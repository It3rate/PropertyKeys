﻿using DataArcs.Stores;
using DataArcs.SeriesData;

namespace DataArcs.Mutators
{
	public abstract class Mutator
	{
		protected Store Store { get; set; }

		public Mutator(Store store)
		{
			Store = store;
			Store.HardenToData();
		}

		public virtual Series GetValueAtIndex(Series series, int index)
		{
			return Store.GetSeriesAtIndex(index);
		}

		public virtual Series GetValueAtT(Series series, float t)
		{
			return Store.GetSeriesAtT(t);
		}

		public virtual float GetTAtT(float t)
		{
			return Store.GetTatT(t);
		}

		public abstract void Update(float t);
		public abstract void ResetData();
	}
}