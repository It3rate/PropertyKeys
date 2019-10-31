using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
    public class LinkingStore : Store
    {
        public int LinkedCompositeId { get; }
        public PropertyId PropertyId { get; }
        public Slot[] SlotMapping { get; }
        private Player _player;
        private IStore _mixStore;

        private IStore MaskedStore => _mixStore;// ?? _player[LinkedCompositeId]?.GetStore(PropertyId);
		// todo: consider implications of having own samplers and combines here. Or copy masked store into this.
        public override CombineFunction CombineFunction { get => MaskedStore.CombineFunction; set => MaskedStore.CombineFunction = value; }
        public override CombineTarget CombineTarget { get => MaskedStore.CombineTarget; set => MaskedStore.CombineTarget = value; }
        public override Sampler Sampler
        {
	        get => MaskedStore?.Sampler ?? _player[LinkedCompositeId]?.GetStore(PropertyId)?.Sampler;
	        set{ if (MaskedStore != null) MaskedStore.Sampler = value; }

        }
        // todo: this can cause a cycle if this is a link for a locationProperty (capacity will use location recursively)
        public override int Capacity => MaskedStore?.Capacity ?? _player[LinkedCompositeId]?.Capacity ?? 1;
        
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
            return GetValuesAtT(SamplerUtils.TFromIndex(Capacity, index)); //   index / (Capacity - 1f));
        }

        public override Series GetValuesAtT(float t)
        {
            Series result = null;
            if (PropertyIdSet.IsTSampling(PropertyId))
            {
                Series link = _player[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                if (link != null)
                {
                    var slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
                    if(PropertyIdSet.IsTCombining(PropertyId))
                    {
                        slotMapped.CombineInto(new FloatSeries(1, t), CombineFunction, t);
                    }

                    result = _mixStore?.GetValuesAtT(t) ?? slotMapped.GetZeroSeries(1);
                    float[] resultArray = new float[result.VectorSize];
                    for (int i = 0; i < result.VectorSize; i++)
                    {
	                    resultArray[i] = _mixStore?.GetValuesAtT(slotMapped.FloatDataAt(i)).FloatDataAt(i) ?? result.FloatDataAt(i);
                    }
					result.SetSeriesAtIndex(0, new FloatSeries(slotMapped.VectorSize, resultArray));
                    //result = _mixStore?.GetValuesAtT(slotMapped.X);
                }
            }
            else
            {
                result = _mixStore?.GetValuesAtT(t);
                if (result != null)
                {
					// This is two step in order to use slot mapping, probably can sensibly combine this.
	                Series link = _player[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
	                Series slotMapped = SeriesUtils.SwizzleSeries(SlotMapping, link);
	                result.CombineInto(slotMapped, CombineFunction, t);
                }
                else
                {
                    result = _player[LinkedCompositeId]?.GetSeriesAtT(PropertyId, t, null);
                }
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            ParametricSeries result = _mixStore.GetSampledTs(seriesT);
            ParametricSeries link = _player[LinkedCompositeId]?.GetSampledTs(PropertyId, seriesT);
            if (link != null)
            {
                Series mappedValues = SeriesUtils.SwizzleSeries(SlotMapping, link);
                result.CombineInto(mappedValues, CombineFunction);
            }
            return result;
        }


        public override void Update(float deltaTime)
        {
            _mixStore?.Update(deltaTime);
        }

        public override void ResetData()
        {
            _mixStore?.ResetData();
        }

        public override void BakeData()
        {
            _mixStore?.BakeData();
        }
    }
}
