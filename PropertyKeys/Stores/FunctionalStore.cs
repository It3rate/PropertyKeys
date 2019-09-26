using System;
using DataArcs.SeriesData;
using System.Collections;
using System.Collections.Generic;

namespace DataArcs.Stores
{
	public class FunctionalStore : StoreBase
    {
		private readonly List<IStore> _stores;

		public override int Capacity
		{
			get => GetStartDataStore().Capacity; // todo: use combine math to get functional store virtual count. Rename to capacity.
		}
		
		public FunctionalStore(params IStore[] stores)
		{
			_stores = new List<IStore>(stores);
		}

        public Series this[int index] => GetValuesAtIndex(index);

		public override Series GetFullSeries(int index) => _stores[index].GetFullSeries(0);

        public override Series GetValuesAtIndex(int index)
		{
			var series = _stores[0].GetValuesAtIndex(index);
			for (var i = 1; i < _stores.Count; i++)
			{
				var b = _stores[i].GetValuesAtIndex(index);
				series.CombineInto(b, _stores[i].CombineFunction);
			}

			return series;
		}

        public override Series GetValuesAtT(float t)
		{
			Series series = null;
			foreach (var store in _stores)
			{
				if (store.CombineTarget == CombineTarget.T)
				{
					t = store.GetValuesAtT(t).X;
				}
				else if(series != null)
				{
					var b = store.GetValuesAtT(t);
					series.CombineInto(b, store.CombineFunction);
                }
				else
				{
					series = store.GetValuesAtT(t);
                }
			}
			return series;
        }

        public override ParametricSeries GetSampledTs(float t)
        {
            ParametricSeries result = null;

            foreach (var store in _stores)
            {
                if (store.CombineTarget == CombineTarget.T)
                {
                    t = store.GetSampledTs(t).X;
                }
                else if (result != null)
                {
                    var b = store.GetSampledTs(t);
                    result.CombineInto(b, store.CombineFunction);
                }
                else
                {
                    result = store.GetSampledTs(t);
                }
            }
            
            return result;
        }

        public override void Update(float deltaTime)
		{
			foreach (var store in _stores)
			{
				store.Update(deltaTime);
			}
        }

        public override void ResetData()
		{
			foreach (var store in _stores)
			{
				store.ResetData();
			}
		}

        public override void HardenToData()
		{
			foreach (var store in _stores)
			{
				store.HardenToData();
			}
		}

		private IStore GetStartDataStore()
		{
			IStore result = _stores[0];
			int index = 1;
			while (result.CombineTarget == CombineTarget.T && index < _stores.Count)
			{
				result = _stores[index];
			}
			return result;
		}

		
		public IStore GetStoreAt(int index) => _stores[Math.Max(0, Math.Min(_stores.Count - 1, index))];
		public void Add(IStore item) => _stores.Add(item);
		public void Insert(int index, IStore item) => _stores.Insert(Math.Max(0, Math.Min(_stores.Count - 1, index)), item);
		public bool Remove(IStore item) => _stores.Remove(item);
		public void RemoveAt(int index)
		{
			if (index >= 0 && index < _stores.Count)
			{
				_stores.RemoveAt(index);
			}
		}
		public bool RemoveById(int id)
		{
			bool result = false;
			int index = _stores.FindIndex(s => s.StoreId == id);
			if (index > -1)
			{
				_stores.RemoveAt(index);
				result = true;
			}
			return result;
		}

    }
}