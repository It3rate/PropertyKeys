﻿using DataArcs.SeriesData;

namespace DataArcs.Stores
{
	public class FunctionalStore : IStore
    {
		public readonly Store[] Stores;
		
		public FunctionalStore(params Store[] stores)
		{
			Stores = stores;
		}

		public Series GetSeries(int index) => Stores[index].GetSeries(0);

		public CombineFunction CombineFunction { get; set; }
		public CombineTarget CombineTarget { get; set; }
		public int VirtualCount
		{
			get => GetNonTStore().VirtualCount;
			set => GetNonTStore().VirtualCount = value;
		}

		private Store GetNonTStore()
		{
			Store result = Stores[0];
			int index = 1;
			while (result.CombineTarget == CombineTarget.T && index < Stores.Length)
			{
				result = Stores[index];
			}
			return result;
		}

		public Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			var series = Stores[0].GetSeriesAtIndex(index, virtualCount);
			for (var i = 1; i < Stores.Length; i++)
			{
				var b = Stores[i].GetSeriesAtIndex(index, virtualCount);
				series.CombineInto(b, Stores[i].CombineFunction);
			}

			return series;
		}

		public Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			Series series = null;
			//var series = Stores[0].GetSeriesAtT(t, virtualCount);
			foreach (var store in Stores)
			{
				if (store.CombineTarget == CombineTarget.T)
				{
					t = store.GetSeriesAtT(t)[0];
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

        public float GetTatT(float t)
        {
            throw new System.NotImplementedException();
        }

        public void HardenToData()
		{
			foreach (var store in Stores)
			{
				store.HardenToData();
			}
		}

		public void Reset()
		{
			foreach (var store in Stores)
			{
				store.Reset();
			}
		}

		public void Update(float time)
		{
			foreach (var store in Stores)
			{
				store.Update(time);
			}
		}
	}
}