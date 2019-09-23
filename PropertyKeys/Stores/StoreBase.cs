using System;
using System.Collections;
using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
    public abstract class StoreBase : IStore
    {
	    private static int _idCounter = 1;
	    public int StoreId { get; }

        public CombineFunction CombineFunction { get; set; }
        public CombineTarget CombineTarget { get; set; }
        public Sampler Sampler { get; set; }
        public virtual int VirtualCount => Sampler.Capacity;

        protected StoreBase()
	    {
		    StoreId = _idCounter++;
	    }

        public abstract Series GetFullSeries(int index);

        public abstract Series GetSeriesAtIndex(int index, int virtualCount = -1);

        public abstract Series GetSeriesAtT(float t, int virtualCount = -1);

        public abstract ParametricSeries GetSampledT(float t);

        public abstract void Update(float deltaTime);

        public abstract void ResetData();

        public abstract void HardenToData();

        public IEnumerator GetEnumerator()
        {
	        return new IStoreEnumerator(this);
        }
    }
}
