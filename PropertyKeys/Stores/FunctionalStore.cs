using System;
using DataArcs.SeriesData;
using System.Collections;
using System.Collections.Generic;

namespace DataArcs.Stores
{
	public class FunctionalStore : StoreBase
    {
		private readonly List<IStore> _stores;

		public override int VirtualCount
		{
			get => GetStartDataStore().VirtualCount;
			set => GetStartDataStore().VirtualCount = value;
		}
		
		public FunctionalStore(params IStore[] stores)
		{
			_stores = new List<IStore>(stores);
		}

        public Series this[int index] => GetSeriesAtIndex(index);

		public override Series GetFullSeries(int index) => _stores[index].GetFullSeries(0);

        public override Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			var series = _stores[0].GetSeriesAtIndex(index, virtualCount);
			for (var i = 1; i < _stores.Count; i++)
			{
				var b = _stores[i].GetSeriesAtIndex(index, virtualCount);
				series.CombineInto(b, _stores[i].CombineFunction);
			}

			return series;
		}

        public override Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			Series series = null;
			//var series = _stores[0].GetSeriesAtT(t, virtualCount);
			foreach (var store in _stores)
			{
				if (store.CombineTarget == CombineTarget.T)
				{
					t = store.GetSeriesAtT(t).FloatDataAt(0);
				}
				else if(series != null)
				{
					var b = store.GetSeriesAtT(t, virtualCount);
					series.CombineInto(b, store.CombineFunction);
                }
				else
				{
					series = store.GetSeriesAtT(t, virtualCount);
                }
			}
			return series;
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