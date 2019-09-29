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
        public int CompositeId { get; }
        public PropertyId PropertyId { get; }
        public Slot[] SlotMapping { get; }
        private Player _player;

        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping) : base(null)
        {
	        CompositeId = compositeId;
	        PropertyId = propertyId;
	        SlotMapping = slotMapping;
	        _player = Player.GetPlayerById(0); 
			_series = _player[compositeId]?.GetStore(propertyId)?.GetFullSeries(0);
        }

        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping, Store store) : base(store)
        {
	        CompositeId = compositeId;
	        PropertyId = propertyId;
	        SlotMapping = slotMapping;
	        _player = Player.GetPlayerById(0); // todo: create player versioning.
        }

        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping,
            Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, 
            CombineTarget combineTarget = CombineTarget.Destination) : base(series, sampler, combineFunction, combineTarget)
        {
            CompositeId = compositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Player.GetPlayerById(0); // todo: create player versioning.
        }

        private IStore GetLinkedStore()
        {
            return _player[CompositeId]?.GetStore(PropertyId);
        }

        public override Series GetValuesAtIndex(int index)
        {
	        return GetValuesAtT(index / (Capacity - 1f));
        }

        public override Series GetValuesAtT(float t)
        {
            Series result;
            if (PropertyId == PropertyId.SampleAtT)
            {
                float newT = GetLinkedStore()?.GetValuesAtT(t).X ?? t;
	            result = base.GetValuesAtT(newT);
            }
            else
            {
                result = base.GetValuesAtT(t);
                float curT = _player[CompositeId]?.InputT ?? 0;
                Series link = GetLinkedStore()?.GetValuesAtT(curT);
                if (link != null)
                {
                    Series mappedValues = SeriesUtils.GetSubseries(SlotMapping, link);
                    result.CombineInto(mappedValues, CombineFunction, curT);
                }
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(float t)
        {
            ParametricSeries result = base.GetSampledTs(t);
            ParametricSeries link = GetLinkedStore()?.GetSampledTs(t);
            if (link != null)
            {
                Series mappedValues = SeriesUtils.GetSubseries(SlotMapping, link);
                result.CombineInto(mappedValues, CombineFunction);
            }
            return result;
        }
    }
}
