using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Mutators
{
	public abstract class Mutator
	{
		protected Store Store { get; set; }

		public Mutator(Store store)
		{
			Store = store;
			Store.BakeData();
		}

		public virtual ISeries GetValueAtT(ISeries series, float t)
		{
			return Store.GetValuesAtT(t);
		}

		public abstract void Update(double currentTime, double deltaTime);
		public abstract void ResetData();
	}
}