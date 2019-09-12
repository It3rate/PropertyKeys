using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
	public class Store : IStore
    {
		private Series _series;

		public CombineFunction CombineFunction { get; set; }
		public int VirtualCount
		{
			get => _series.VirtualCount;
			set => _series.VirtualCount = value;
		}

		protected Sampler Sampler { get; set; }

		public EasingType[]
			EasingTypes { get; protected set; } // move to properties? May be useful for creating virtual data. Change to t sampler.


		protected Store(EasingType[] easingTypes = null, CombineFunction combineFunction = CombineFunction.Add)
		{
		}

		public Store(Series series, Sampler sampler = null, EasingType[] easingTypes = null,
			CombineFunction combineFunction = CombineFunction.Add)
		{
			_series = series;
			Sampler = sampler ?? new LineSampler();
			EasingTypes = easingTypes ?? new []{EasingType.Linear};
			CombineFunction = combineFunction;
		}

		public Store(int[] data, Sampler sampler = null, EasingType[] easingTypes = null,
			CombineFunction combineFunction = CombineFunction.Add) : this(new IntSeries(1, data), sampler, easingTypes,
			combineFunction)
		{
		}

		public Store(float[] data, Sampler sampler = null, EasingType[] easingTypes = null,
			CombineFunction combineFunction = CombineFunction.Add) : this(new FloatSeries(1, data), sampler,
			easingTypes, combineFunction)
		{
		}

		public Series GetSeries(int index) => _series;

		public virtual void Reset()
		{
			_series.Reset();
		}

		public virtual void Update(float time)
		{
			_series.Update(time);
		}

		public virtual void HardenToData()
		{
			_series = _series.HardenToData(this);
			Sampler = null;
			EasingTypes = null;
		}

		public virtual Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
			return Sampler != null
				? Sampler.GetValueAtIndex(_series, index, virtualCount)
				: _series.GetSeriesAtIndex(index);
		}

		public virtual Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			return Sampler?.GetValueAtT(_series, t, virtualCount) ?? _series.GetValueAtT(t);
		}

		public virtual float GetTatT(float t)
		{
			return Sampler?.GetTAtT(t) ?? _series.GetValueAtT(t)[0];
		}
	}
}