namespace DataArcs.Stores
{
	public class FunctionalStore : Store
	{
		public readonly Store[] Stores;
		public override Series.Series Series => Stores[0].Series;

		//public FunctionalStore(Store[] stores)
		//{
		//    Stores = stores;
		//}
		public FunctionalStore(params Store[] stores)
		{
			Stores = stores;
		}

		public override Series.Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			var series = Stores[0].GetSeriesAtIndex(index, virtualCount);
			for (var i = 1; i < Stores.Length; i++)
			{
				var b = Stores[i].GetSeriesAtIndex(index, virtualCount);
				series.CombineInto(b, Stores[i].CombineFunction);
			}

			return series;
		}

		public override Series.Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			var series = Stores[0].GetSeriesAtT(t, virtualCount);
			for (var i = 1; i < Stores.Length; i++)
			{
				var b = Stores[i].GetSeriesAtT(t, virtualCount);
				series.CombineInto(b, Stores[i].CombineFunction);
			}

			return series;
		}

		public override void HardenToData()
		{
			foreach (var store in Stores)
			{
				store.HardenToData();
			}
		}

		public override void Reset()
		{
			foreach (var store in Stores)
			{
				store.Reset();
			}
		}

		public override void Update(float time)
		{
			foreach (var store in Stores)
			{
				store.Update(time);
			}
		}
	}
}