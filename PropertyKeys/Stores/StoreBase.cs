using System;
using System.Collections;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
    public abstract class StoreBase : IStore
    {
	    public string Name { get; set; }
        public int Id { get; private set; }

        private CombineFunction _combineFunction;
        public virtual CombineFunction CombineFunction { get => _combineFunction; set => _combineFunction = value; }

        protected int _samplerId;
        public virtual Sampler Sampler
        {
	        get => Player.CurrentSamplers[_samplerId];
	        set => _samplerId = value.Id;
        }

        protected int _seriesId;
        public Series Series
        {
	        get => _seriesId > 0 ? Player.CurrentSeries[_seriesId] : null;
	        set
	        {
		        if (value != null)
		        {
		            _seriesId = Player.CurrentSeries.AddToLibrary(value);
		        }
	        }
        }

        public virtual int Capacity => Sampler.SampleCount;
        public virtual bool ShouldInterpolate { get; set; } = false; // linear vs nearest

        protected StoreBase(Series series = null)
        {
	        Series = series;
	        Player.CurrentStores.AddToLibrary(this);
        }
        protected StoreBase(int seriesId, int sampleId, CombineFunction combineFunction)
        {
	        _seriesId = seriesId;
	        _samplerId = sampleId;
	        _combineFunction = combineFunction;
	        Player.CurrentStores.AddToLibrary(this);
        }

        public bool AssignIdIfUnset(int id)
        {
	        bool result = false;
	        if (Id == 0 && id > 0)
	        {
		        Id = id;
		        result = true;
	        }
	        return result;
        }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }

        public abstract Series GetSeriesRef();
        public abstract void SetFullSeries(Series value);

        public abstract Series GetValuesAtT(float t);
        
        public abstract ParametricSeries GetSampledTs(ParametricSeries seriesT);
        public virtual Series GetNeighbors(int index, bool wrapEdges = true)
        {
	        return Sampler.GetNeighbors(GetSeriesRef(), index, wrapEdges);
        }
		

        public abstract void Update(double currentTime, double deltaTime);

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
