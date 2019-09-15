using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Mutators
{
	public class RandomMutator : Mutator
	{
		private RandomSeries RandomSeries { get; }

		public RandomMutator(int vectorSize, SeriesType type, int virtualCount, float min, float max, int seed = 0) :
			base(new Store(new RandomSeries(vectorSize, type, virtualCount, min, max, seed)))
		{
			RandomSeries = (RandomSeries) Store.GetSeries(0);
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