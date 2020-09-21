using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Stores
{
    public class LinkingStore : Store
    {
        public int LinkedCompositeId { get; }
        public PropertyId PropertyId { get; }
        public Slot[] SlotMapping { get; }
        private Player _player;
        private IStore _mixStore;

        private IStore MixStore => _mixStore;// ?? _player[LinkedCompositeId]?.GetStore(PropertyId);
		// todo: consider implications of having own samplers and combines here. Or copy masked store into this.
        public override CombineFunction MergeFunction { get => MixStore.MergeFunction; set => MixStore.MergeFunction = value; }
        public override Sampler Sampler
        {
	        get => MixStore?.Sampler ?? Player.CurrentComposites[LinkedCompositeId]?.GetStore(PropertyId)?.Sampler;
	        set{ if (MixStore != null) MixStore.Sampler = value; }

        }
        // todo: this can cause a cycle if this is a link for a locationProperty (capacity will use location recursively)
        public override int Capacity => MixStore?.Capacity ?? Player.CurrentComposites[LinkedCompositeId]?.Capacity ?? 1;
        
        public LinkingStore(int linkedCompositeId, PropertyId propertyId, Slot[] slotMapping, IStore store)
        {
            LinkedCompositeId = linkedCompositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Player.GetPlayerById(0);
            _mixStore = store;
        }
        
        public override Series GetValuesAtIndex(int index)
        { 
            // TODO: can't just pass this in because capacity may mean row capacity.
            // t needs to be a series always, and can get multi dim t from sampler.
            return GetValuesAtT(SamplerUtils.TFromIndex(Capacity, index)); //   index / (SampleCount - 1f));
        }

        public override Series GetValuesAtT(float t)
        {
            Series result = null;
            if (PropertyIdSet.IsTSampling(PropertyId))
            {
                Series link = Player.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                if (link != null)
                {
                    var slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
                    if(PropertyIdSet.IsTCombining(PropertyId))
                    {
                        slotMapped.CombineInto(new FloatSeries(1, t), MergeFunction, t);
                    }

                    result = _mixStore?.GetValuesAtT(t) ?? SeriesUtils.CreateSeriesOfType(slotMapped.Type, slotMapped.VectorSize, 1, 0f);
                    float[] resultArray = new float[result.VectorSize];
                    for (int i = 0; i < result.VectorSize; i++)
                    {
	                    resultArray[i] = _mixStore?.GetValuesAtT(slotMapped.FloatDataAt(i)).FloatDataAt(i) ?? result.FloatDataAt(i);
                    }
					result.SetRawDataAt(0, new FloatSeries(slotMapped.VectorSize, resultArray));
                    //result = _mixStore?.GetValuesAtT(slotMapped.X);
                }
            }
            else
            {
                result = _mixStore?.GetValuesAtT(t);
                if (result != null)
                {
					// This is two step in order to use slot mapping, probably can sensibly combine this.
	                Series link = Player.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
	                Series slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
	                result.CombineInto(slotMapped, MergeFunction, t);
                }
                else
                {
                    result = Player.CurrentComposites[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                    result = SeriesUtils.SwizzleSeries(SlotMapping, result);
                }
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            ParametricSeries result = _mixStore.GetSampledTs(seriesT);
            ParametricSeries link = Player.CurrentComposites[LinkedCompositeId]?.GetSampledTs(PropertyId, seriesT);
            if (link != null)
            {
                Series mappedValues = SeriesUtils.SwizzleSeries(SlotMapping, link);
                result.CombineInto(mappedValues, MergeFunction);
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
