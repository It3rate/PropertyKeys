﻿using System;
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
        private IStore _mixStore;

        private IStore MaskedStore => _mixStore ?? _player[CompositeId]?.GetStore(PropertyId);
        public override CombineFunction CombineFunction { get => MaskedStore.CombineFunction; set => MaskedStore.CombineFunction = value; }
        public override CombineTarget CombineTarget { get => MaskedStore.CombineTarget; set => MaskedStore.CombineTarget = value; }
        public override Sampler Sampler { get => MaskedStore.Sampler; set => MaskedStore.Sampler = value; }
        public override int Capacity => MaskedStore.Capacity;
        
        public LinkingStore(int compositeId, PropertyId propertyId, Slot[] slotMapping, IStore store)
        {
            CompositeId = compositeId;
            PropertyId = propertyId;
            SlotMapping = slotMapping;
            _player = Player.GetPlayerById(0);
            _mixStore = store;
            //_series = store?.GetFullSeries(0);
        }
        

        private IStore GetLinkedStore()
        {
            return _player[CompositeId]?.GetStore(PropertyId);
        }
        private IStore GetMixStore()
        {
            return _mixStore;
        }

        public override Series GetValuesAtIndex(int index)
        {
            return GetValuesAtT(index / (Capacity - 1f));
        }

        public override Series GetValuesAtT(float t)
        {
            Series result = null;
            float curT = _player[CompositeId]?.InputT ?? 0;
            if (PropertyIdSet.IsTSampling(PropertyId))
            {
                Series link = GetLinkedStore()?.GetValuesAtT(t);
                if (link != null)
                {
                    var curTSeries = SeriesUtils.GetSubseries(SlotMapping, link);
                    if(PropertyIdSet.IsTCombining(PropertyId))
                    {
                        curTSeries.CombineInto(new FloatSeries(1, t), CombineFunction, t);
                    }
                    result = GetMixStore()?.GetValuesAtT(curTSeries.X);
                }
            }
            else
            {
                result = GetMixStore()?.GetValuesAtT(t);
                if (result != null)
                {
                    Series link = GetLinkedStore()?.GetValuesAtT(curT);
                    if (link != null)
                    {
                        Series linkMapped = SeriesUtils.GetSubseries(SlotMapping, link);
                        result.CombineInto(linkMapped, CombineFunction, curT);
                    }
                }
                else
                {
                    result = GetLinkedStore()?.GetValuesAtT(curT);
                }
            }
            return result;
        }

        public override ParametricSeries GetSampledTs(float t)
        {
            ParametricSeries result = _mixStore.GetSampledTs(t);
            ParametricSeries link = GetLinkedStore()?.GetSampledTs(t);
            if (link != null)
            {
                Series mappedValues = SeriesUtils.GetSubseries(SlotMapping, link);
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
