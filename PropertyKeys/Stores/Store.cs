using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
		private Series _series;
		
        public override int VirtualCount
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

		public Series this[int index] => GetSeriesAtIndex(index);

        public override Series GetSeries(int index)
        {
            return _series;
        }

		public override Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
            virtualCount = (virtualCount == -1) ? VirtualCount : virtualCount;
			return Sampler != null
				? Sampler.GetValueAtIndex(_series, index, virtualCount)
				: _series.GetValueAtVirtualIndex(index);
		}

		public override Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			return Sampler?.GetValueAtT(_series, t, virtualCount) ?? _series.GetValueAtT(t);
		}

		public override void Update(float deltaTime)
		{
			_series.Update(deltaTime);
		}

        public override void ResetData()
		{
			_series.ResetData();
		}

		public override void HardenToData()
		{
			_series = _series.HardenToData(this);
			Sampler = null;
		}


		
    }
}