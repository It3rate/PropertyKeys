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
        public virtual int Capacity => Sampler.Capacity;

        protected StoreBase()
	    {
		    StoreId = _idCounter++;
	    }

        public abstract Series GetFullSeries(int index);

        public abstract Series GetValuesAtIndex(int index);

        public abstract Series GetValuesAtT(float t);
        
        public abstract ParametricSeries GetSampledTs(float t);

        public abstract void Update(float deltaTime);

        public abstract void ResetData();

        public abstract void HardenToData();

        public IEnumerator GetEnumerator()
        {
	        return new IStoreEnumerator(this);
        }
    }
}
