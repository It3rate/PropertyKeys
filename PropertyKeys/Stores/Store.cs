using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : IStore
    {
		private Series _series;

		public CombineFunction CombineFunction { get; set; }
		public CombineTarget CombineTarget { get; set; }
        public int VirtualCount
		{
			get => _series.VirtualCount;
			set => _series.VirtualCount = value;
		}

		protected Sampler Sampler { get; set; }

		public Store(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, CombineTarget combineTarget = CombineTarget.Destination)
		{
			_series = series;
			Sampler = sampler ?? new LineSampler();
			CombineFunction = combineFunction;
			CombineTarget = combineTarget;
        }

		public Store(int[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) :
			this(new IntSeries(1, data), sampler, combineFunction)

		{
		}
		public Store(float[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add) :
			this(new FloatSeries(1, data), sampler, combineFunction)

		{
		}
        public Series GetSeries(int index) => _series;

		public virtual void ResetData()
		{
			_series.ResetData();
		}

		public virtual void Update(float time)
		{
			_series.Update(time);
		}

		public virtual void HardenToData()
		{
			_series = _series.HardenToData(this);
			Sampler = null;
		}

		public virtual Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			return Sampler != null
				? Sampler.GetValueAtIndex(_series, index, virtualCount)
				: _series.GetDataAtIndex(index);
		}

		public virtual Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			return Sampler?.GetValueAtT(_series, t, virtualCount) ?? _series.GetValueAtT(t);
		}

		public virtual float GetTatT(float t)
		{
			return Sampler?.GetTAtT(t) ?? _series.GetValueAtT(t).FloatDataAt(0);
        }

        #region Enumeration
        public Series this[int index] => GetSeriesAtIndex(index);
        private int _position;
        public bool MoveNext()
        {
            _position++;
            return (_position < VirtualCount);
        }

        public object Current => this[_position];

        public void Reset()
        {
            _position = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        #endregion
    }
}