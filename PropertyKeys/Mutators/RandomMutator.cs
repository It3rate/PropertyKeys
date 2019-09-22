using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Mutators
{
	public class RandomMutator : Mutator
	{
		private RandomSeries RandomSeries { get; }

		public RandomMutator(int vectorSize, SeriesType type, int virtualCount, Series minMax, int seed = 0) :
			base(new Store(new RandomSeries(vectorSize, type, virtualCount, minMax, seed)))
		{
			RandomSeries = (RandomSeries) Store.GetFullSeries(0);
		}

		public override void Update(float time)
		{
			RandomSeries.Update(time);
		}

		public override void ResetData()
		{
			RandomSeries.ResetData();
		}
	}
}