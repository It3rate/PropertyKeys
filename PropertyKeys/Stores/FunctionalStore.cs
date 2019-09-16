using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class FunctionalStore : StoreBase
    {
		public readonly Store[] Stores;

		public override int VirtualCount
		{
			get => GetNonTStore().VirtualCount;
			set => GetNonTStore().VirtualCount = value;
		}
		
		public FunctionalStore(params Store[] stores)
		{
			Stores = stores;
		}

        public Series this[int index] => GetSeriesAtIndex(index);

		public override Series GetSeries(int index) => Stores[index].GetSeries(0);

        public override Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			var series = Stores[0].GetSeriesAtIndex(index, virtualCount);
			for (var i = 1; i < Stores.Length; i++)
			{
				var b = Stores[i].GetSeriesAtIndex(index, virtualCount);
				series.CombineInto(b, Stores[i].CombineFunction);
			}

			return series;
		}

        public override Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			Series series = null;
			//var series = Stores[0].GetSeriesAtT(t, virtualCount);
			foreach (var store in Stores)
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

        public override void Update(float time)
		{
			foreach (var store in Stores)
			{
				store.Update(time);
			}
        }

        public override void ResetData()
		{
			foreach (var store in Stores)
			{
				store.ResetData();
			}
		}

        public override void HardenToData()
		{
			foreach (var store in Stores)
			{
				store.HardenToData();
			}
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
    }
}