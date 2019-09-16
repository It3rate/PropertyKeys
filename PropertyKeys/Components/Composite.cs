﻿using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Composite : CompositeBase
	{
		private Dictionary<PropertyId, IStore> _stores { get; }

        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        public Composite Parent { get; set; }

		public Composite(Composite parent = null)
		{
			Parent = parent;
			_stores = new Dictionary<PropertyId, IStore>();
		}
        
		public override IStore GetStore(PropertyId propertyId)
		{
			_stores.TryGetValue(propertyId, out var result);
			if (result == null && Parent != null)
			{
				result = Parent.GetStore(propertyId);
			}

			return result;
		}
        
		public void AddProperty(PropertyId id, BlendStore propertyStore)
		{
			_stores.Add(id, propertyStore);
		}

		public void RemoveProperty(PropertyId id, BlendStore propertyStore)
		{
			_stores.Remove(id);
		}
		
        public override void GetDefinedStores(HashSet<PropertyId> ids)
        {
            foreach (var item in _stores.Keys)
            {
                ids.Add(item);
            }
            if(Parent != null)
            {
                Parent.GetDefinedStores(ids);
            }
        }

		public bool shouldShuffle; // basis for switching to events

		public override void Update(float currentTime, float deltaTime)
		{
			foreach (var store in _stores.Values)
			{
				store.Update(deltaTime);
			}

            if (deltaTime <= 0.05f && shouldShuffle)
            {
                SeriesUtils.Shuffle(((BlendStore)GetStore(PropertyId.Location))[1].GetSeries(0));
            }
            if (deltaTime > 0.99 && shouldShuffle)
            {
                Series s = ((BlendStore)GetStore(PropertyId.Location))[0].GetSeries(0);
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }

            CurrentT = deltaTime;
        }

        public override CompositeBase CreateChild()
        {
            return new Composite(this);
        }

    }

}