﻿using System;
using System.Collections.Generic;
using Motive.Graphic;
using Motive.SeriesData;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.Components.Transitions
{
    public class BlendTransition : Container
    {
        private readonly Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();

        public ITimeable Runner { get; }
        public IStore Easing { get; set; }
        public IContainer Start { get; set; }
        public IContainer End { get; set; }

        public int[] StartStrides { get; }
        public int[] EndStrides { get; }

        public BlendTransition(IContainer start, IContainer end, ITimeable runner, Store easing = null)
        {
			Runner = runner;
            Motive.Runner.GetRunnerById(0).ActivateComposite(Runner.Id);

			Easing = easing;
            Start = start;
	        End = end;
            StartStrides = start.ChildCounts; // todo:  include own items
            EndStrides = end.ChildCounts;
            GenerateBlends(); // eventually immutable after creation, so no new blends
        }

        public override int NestedItemCount
        {
            get
            {
                var startCount = Start?.NestedItemCount ?? 1;
                var endCount = End?.NestedItemCount ?? 1;
                return (int)Math.Max(startCount, endCount);
            }
        }
        public override int NestedItemCountAtT(float t)
        {
            var startCount = Start?.NestedItemCount ?? 1;
            var endCount = End?.NestedItemCount ?? 1;
            return (int)(startCount + (endCount - startCount) * t);
        }
        
        public void GenerateBlends()
        {
            // todo: render children maps from subcollection to subcollection as needed.
            // to do this, don't precompute blends as the composite target may change as t drifts over the child elements.
            Renderer = (Start is IDrawable) ? ((IDrawable)Start).Renderer : null;

            _blends.Clear();
            HashSet<PropertyId> commonKeys = new HashSet<PropertyId>();
            HashSet<PropertyId> endKeys = new HashSet<PropertyId>();
            Start?.GetDefinedStores(commonKeys);
            End?.GetDefinedStores(endKeys);
            commonKeys.IntersectWith(endKeys);
            foreach (var key in commonKeys)
            {
	            if (Start?.GetStore(key).Id != End?.GetStore(key).Id)
	            {
					_blends[key] = (BlendStore)GetStore(key);
	            }
            }
        }

        public override void StartUpdate(double currentTime, double deltaTime)
        {
            base.StartUpdate(currentTime, deltaTime);

            Start?.Update(currentTime, deltaTime);
            End?.Update(currentTime, deltaTime);
            foreach (var item in _blends.Values)
            {
                item.Update(currentTime, Runner.InterpolationT);
            }
        }

        public override IStore GetStore(PropertyId propertyId)
        {
	        IStore result;
	        if (_blends.ContainsKey(propertyId))
	        {
		        result = _blends[propertyId];
	        }
	        else
	        {
		        result = Start?.GetStore(propertyId);
		        IStore end = End?.GetStore(propertyId);
		        if (result == null)
		        {
			        result = end;
		        }
		        else if (end != null)
		        {
			        result = new BlendStore(result, end);
		        }
	        }
	        return result;
        }
        public override void GetDefinedStores(HashSet<PropertyId> ids)
        {
	        base.GetDefinedStores(ids);
	        Start?.GetDefinedStores(ids);
	        End?.GetDefinedStores(ids);
        }
		
        public override ISeries GetSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries)
        {
            var startDict = new Dictionary<PropertyId, ISeries>() { { propertyId, null } };
            Start?.QueryPropertiesAtT(startDict, t, false);
            ISeries result = startDict[propertyId];
            if (_blends.ContainsKey(propertyId))
            {
                var endDict = new Dictionary<PropertyId, ISeries>() { { propertyId, null } };
                End?.QueryPropertiesAtT(endDict, t, false);

                float indexT = t + Runner.InterpolationT; // delay per element.

                float easedT = Easing?.GetValuesAtT(Runner.InterpolationT * indexT).X ?? Runner.InterpolationT;
                result.InterpolateInto(endDict[propertyId], easedT);
            }
            else if(result == null)
            {
                result = End?.GetNestedSeriesAtT(propertyId, t, parentSeries) ?? SeriesUtils.CreateSeriesOfType(SeriesType.Float, 1, 1, 0f);
            }
            return result;
        }
        public override IRenderable QueryPropertiesAtT(Dictionary<PropertyId, ISeries> data, float t, bool addLocalProperties)
        {
            var endDict = new Dictionary<PropertyId, ISeries>(data);
            IRenderable result = Start?.QueryPropertiesAtT(data, t, true);
            result = End?.QueryPropertiesAtT(endDict, t, true) ?? result;

            float indexT = t + Runner.InterpolationT; // delay per element.
            float easedT = Easing?.GetValuesAtT(Runner.InterpolationT * indexT).X ?? Runner.InterpolationT;
            foreach (var key in endDict.Keys)
            {
                if (data.TryGetValue(key, out ISeries value))
                {
                    value.InterpolateInto(endDict[key], easedT);
                }
                else
                {
                    data[key] = endDict[key];
                }
            }

            return result;
        }

    }
}
