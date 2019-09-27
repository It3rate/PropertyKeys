﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataArcs.Adapters;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Composite : DrawableComposite
	{
		private Dictionary<PropertyId, IStore> _stores { get; }

		public Composite(IComposite parent = null)
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

		public void AddProperty(PropertyId id, IStore store)
		{
			_stores[id] = store;
		}
		public void AppendProperty(PropertyId id, IStore store)
		{
			if (_stores.ContainsKey(id))
			{
				IStore curStore = _stores[id];
				if (curStore is FunctionalStore)
				{
					((FunctionalStore) curStore).Add(curStore);
				}
				else
				{
					_stores[id] = new FunctionalStore(curStore, store);
				}
			}
			else
			{
				AddProperty(id, store);
			}
		}

        public void RemoveProperty(PropertyId id, BlendStore store)
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

			float t = CurrentT % 1f;
            if (t <= 0.05f && shouldShuffle)
            {
                SeriesUtils.Shuffle(GetStore(PropertyId.Location).GetFullSeries(0));
            }
            if (t > 0.99 && shouldShuffle)
            {
                Series s = GetStore(PropertyId.Location).GetFullSeries(0);
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }

            CurrentT = deltaTime;
        }

        public override IComposite CreateChild()
        {
            return new Composite(this);
        }

    }

}