using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
		private Series _series;
		
        public override int VirtualCount { get; set; }

		protected Sampler Sampler { get; set; }

		public Store(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, 
            CombineTarget combineTarget = CombineTarget.Destination, int virtualCount = -1)
		{
			_series = series;
			Sampler = sampler ?? new LineSampler();
			CombineFunction = combineFunction;
			CombineTarget = combineTarget;
            VirtualCount = (virtualCount == -1) ? _series.Count : virtualCount;
        }

		public Store(int[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, int virtualCount = -1) :
			this(new IntSeries(1, data), sampler, combineFunction, virtualCount:virtualCount)

		{
		}

		public Store(float[] data, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, int virtualCount = -1) :
			this(new FloatSeries(1, data), sampler, combineFunction, virtualCount:virtualCount)

		{
		}

		public Series this[int index] => GetSeriesAtIndex(index);

        public override Series GetFullSeries(int index)
        {
            return _series;
        }

		public override Series GetSeriesAtIndex(int index, int virtualCount = -1)
		{
            virtualCount = (virtualCount == -1) ? VirtualCount : virtualCount;
			return Sampler.GetValueAtIndex(_series, index, virtualCount);
		}

		public override Series GetSeriesAtT(float t, int virtualCount = -1)
		{
			return Sampler?.GetValueAtT(_series, t, virtualCount) ?? _series.GetValueAtT(t);
		}
        public override ParametricSeries GetSampledT(float t)
        {
            return Sampler.GetSampledT(t);
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
            var len = VirtualCount * _series.VectorSize;
            if (_series.DataSize != len)
            {
                Series result = _series.GetZeroSeries(len);
                for (var i = 0; i < VirtualCount; i++)
                {
                    result.SetDataAtIndex(i, GetSeriesAtIndex(i, VirtualCount));
                }
                _series = result;
            }
			Sampler = new LineSampler();
		}



		public FunctionalStore CreateFunctionalStore(IStore store)
		{
			return new FunctionalStore(this, store);
		}
		public BlendStore CreateBlendStore(IStore store)
		{
			return new BlendStore(this, store);
		}
    }
}