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

        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping,
            int[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) : base(data, sampler, combineFunction)
        {
            CompositeId = compositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Player.GetPlayerById(0);
        }

        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping,
            float[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) : base(data, sampler, combineFunction)
        {
            CompositeId = compositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Player.GetPlayerById(0);
        }

        private IStore GetLinkedStore()
        {
            return _player[CompositeId]?.GetStore(PropertyId);
        }

        public override Series GetValuesAtIndex(int index)
        {
            Series result = base.GetValuesAtIndex(index);
            Series link = GetLinkedStore()?.GetValuesAtIndex(index);
            if(link != null)
            {
                Series mappedValues = SeriesUtils.GetSubseries(SlotMapping, link);
                result.CombineInto(mappedValues, CombineFunction);
            }
            return result;
        }

        public override Series GetValuesAtT(float t)
        {
            Series result;
            // todo: probably linking store always runs on t, the first case
            if (PropertyId == PropertyId.T || PropertyId == PropertyId.Easing)
            {
                float newT = GetLinkedStore()?.GetValuesAtT(t).X ?? t;
                result = base.GetValuesAtT(newT);
            }
            else
            {
                result = base.GetValuesAtT(t);
                Series link = GetLinkedStore()?.GetValuesAtT(t);
                if (link != null)
                {
                    Series mappedValues = SeriesUtils.GetSubseries(SlotMapping, link);
                    result.CombineInto(mappedValues, CombineFunction);
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
