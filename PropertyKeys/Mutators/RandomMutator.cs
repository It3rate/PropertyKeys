using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Mutators
{
	public class RandomMutator : Mutator
	{
		private RandomSeries RandomSeries { get; }

		public RandomMutator(int vectorSize, SeriesType type, int virtualCount, RectFSeries minMax, int seed = 0) :
			base(new Store(new RandomSeries(vectorSize, type, virtualCount, minMax, seed)))
		{
			RandomSeries = (RandomSeries) Store.GetSeriesRef();
		}

		public override void Update(double time)
		{
			RandomSeries.Update(time);
		}

		public override void ResetData()
		{
			RandomSeries.ResetData();
		}
	}
}