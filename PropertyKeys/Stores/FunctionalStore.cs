using DataArcs.SeriesData;

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
        public int VirtualCount { get => Stores[0].VirtualCount; set => Stores[0].VirtualCount = value; }


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
			var series = Stores[0].GetSeriesAtT(t, virtualCount);
			for (var i = 1; i < Stores.Length; i++)
			{
				var b = Stores[i].GetSeriesAtT(t, virtualCount);
				series.CombineInto(b, Stores[i].CombineFunction);
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