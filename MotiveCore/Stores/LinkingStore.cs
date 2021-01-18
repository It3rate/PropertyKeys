using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.Samplers;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Stores
{
    public class LinkingStore : Store
    {
        public int LinkedCompositeId { get; }
        public PropertyId PropertyId { get; }
        public Slot[] SlotMapping { get; }
        private Runner _player;
        private IStore _mixStore;

        private IStore MixStore => _mixStore;// ?? _player[LinkedCompositeId]?.GetStore(PropertyId);
		// todo: consider implications of having own samplers and combines here. Or copy masked store into this.
        public override CombineFunction CombineFunction { get => MixStore.CombineFunction; set => MixStore.CombineFunction = value; }
        public override Sampler Sampler
        {
	        get => MixStore?.Sampler ?? Runner.CurrentComposites[LinkedCompositeId]?.GetStore(PropertyId)?.Sampler;
	        set{ if (MixStore != null) MixStore.Sampler = value; }

        }
        // todo: this can cause a cycle if this is a link for a locationProperty (capacity will use location recursively)
        public override int Capacity => MixStore?.Capacity ?? Runner.CurrentComposites[LinkedCompositeId]?.Capacity ?? 1;
        
        public LinkingStore(int linkedCompositeId, PropertyId propertyId, Slot[] slotMapping, IStore store)
        {
            LinkedCompositeId = linkedCompositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Runner.GetRunnerById(0);
            _mixStore = store;
        }

        public override ISeries GetValuesAtT(float t)
        {
            ISeries result = null;
            if (PropertyIdSet.IsTSampling(PropertyId))
            {
                ISeries link = Runner.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                if (link != null)
                {
                    var slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
                    if(PropertyIdSet.IsTCombining(PropertyId))
                    {
                        slotMapped.CombineInto(new FloatSeries(1, t), CombineFunction, t);
                    }

                    result = _mixStore?.GetValuesAtT(t) ?? SeriesUtils.CreateSeriesOfType(slotMapped.Type, slotMapped.VectorSize, 1, 0f);
                    float[] resultArray = new float[result.VectorSize];
                    for (int i = 0; i < result.VectorSize; i++)
                    {
	                    resultArray[i] = _mixStore?.GetValuesAtT(slotMapped.FloatValueAt(i)).FloatValueAt(i) ?? result.FloatValueAt(i);
                    }
					result.SetSeriesAt(0, new FloatSeries(slotMapped.VectorSize, resultArray));
                    //result = _mixStore?.GetValuesAtT(slotMapped.X);
                }
            }
            else
            {
                result = _mixStore?.GetValuesAtT(t);
                if (result != null)
                {
					// This is two step in order to use slot mapping, probably can sensibly combine this.
	                ISeries link = Runner.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
	                ISeries slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
	                result.CombineInto(slotMapped, CombineFunction, t);
                }
                else
                {
                    result = Runner.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                    result = SeriesUtils.SwizzleSeries(SlotMapping, result);
                }
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            ParametricSeries result = _mixStore.GetSampledTs(seriesT);
            ParametricSeries link = Runner.CurrentComposites[LinkedCompositeId]?.GetNormalizedPropertyAtT(PropertyId, seriesT);
            if (link != null)
            {
                ISeries mappedValues = SeriesUtils.SwizzleSeries(SlotMapping, link);
                result.CombineInto(mappedValues, CombineFunction);
            }
            return result;
        }


        public override void Update(double currentTime, double deltaTime)
        {
            _mixStore?.Update(currentTime, deltaTime);
        }

        public override void ResetData()
        {
            _mixStore?.ResetData();
        }

        public override void BakeData()
        {
            _mixStore?.BakeData();
        }

        public override IStore Clone()
        {
	        return new LinkingStore(LinkedCompositeId, PropertyId, SlotMapping, _mixStore?.Clone());
		}
    }
}
