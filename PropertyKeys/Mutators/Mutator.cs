using DataArcs.Stores;
using DataArcs.SeriesData;

namespace DataArcs.Mutators
{
	public abstract class Mutator
	{
		protected Store Store { get; set; }

		public Mutator(Store store)
		{
			Store = store;
			Store.BakeData();
		}

		public virtual Series GetValueAtT(Series series, float t)
		{
			return Store.GetValuesAtT(t);
		}

		public abstract void Update(double t);
		public abstract void ResetData();
	}
}