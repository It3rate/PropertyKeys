﻿using System;
using System.Collections;
using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
    public abstract class StoreBase : IStore
    {
	    private static int _idCounter = 1;
	    public int StoreId { get; }

        public virtual CombineFunction CombineFunction { get; set; }
        public virtual Sampler Sampler { get; set; }
        protected Series Series { get; set; }
        public virtual int Capacity => Sampler.SliceCount;
        public virtual bool ShouldInterpolate { get; set; } = false; // linear vs nearest

        protected StoreBase(Series series = null)
        {
            StoreId = _idCounter++;
            Series = series;
        }
        //protected StoreBase(IStore store)
        //{
        //    StoreId = _idCounter++;
        //    Sampler = store.Sampler;
        //    CombineFunction = store.CombineFunction;
        //    CombineTarget = store.CombineTarget;
        //}

        public abstract Series GetSeriesRef();
        public abstract void SetFullSeries(Series value);

        public abstract Series GetValuesAtIndex(int index);

        public abstract Series GetValuesAtT(float t);
        
        public abstract ParametricSeries GetSampledTs(ParametricSeries seriesT);
        public virtual Series GetNeighbors(int index, bool wrapEdges = true)
        {
	        return Sampler.GetNeighbors(GetSeriesRef(), index, wrapEdges);
        }
		

        public abstract void Update(float deltaTime);

        public abstract void ResetData();

        public abstract void BakeData();

        public abstract IStore Clone();
        public abstract void CopySeriesDataInto(IStore target);

        public IEnumerator GetEnumerator()
        {
	        return new IStoreEnumerator(this);
        }
    }
}
