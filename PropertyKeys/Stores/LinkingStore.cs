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
        private Player _player;

        public LinkingStore(int compisiteId, PropertyId propertyId, Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, CombineTarget combineTarget = CombineTarget.Destination) : base(series, sampler, combineFunction, combineTarget)
        {
            CompositeId = compisiteId;
            PropertyId = propertyId;
            _player = Player.GetPlayerById(0); // todo: create player versioning.
        }

        public LinkingStore(int compisiteId, PropertyId propertyId, int[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) : base(data, sampler, combineFunction)
        {
            CompositeId = compisiteId;
            PropertyId = propertyId;
            _player = Player.GetPlayerById(0);
        }

        public LinkingStore(int compisiteId, PropertyId propertyId, float[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) : base(data, sampler, combineFunction)
        {
            CompositeId = compisiteId;
            PropertyId = propertyId;
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
                result.CombineInto(link, CombineFunction);
            }
            return result;
        }

        public override Series GetValuesAtT(float t)
        {
            Series result = base.GetValuesAtT(t);
            Series link = GetLinkedStore()?.GetValuesAtT(t);
            if (link != null)
            {
                result.CombineInto(link, CombineFunction);
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(float t)
        {
            ParametricSeries result = base.GetSampledTs(t);
            ParametricSeries link = GetLinkedStore()?.GetSampledTs(t);
            if (link != null)
            {
                result.CombineInto(link, CombineFunction);
            }
            return result;
        }
    }
}
