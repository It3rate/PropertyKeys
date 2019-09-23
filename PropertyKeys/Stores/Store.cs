using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
		private Series _series;

		public Store(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Add, 
            CombineTarget combineTarget = CombineTarget.Destination)
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

        public override Series GetFullSeries(int index)
        {
            return _series;
        }

		public override Series GetSeriesAtIndex(int index)
		{
			return Sampler.GetValueAtIndex(_series, index);
		}

		public override Series GetSeriesAtT(float t)
		{
			return Sampler.GetValueAtT(_series, t);
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
            var len = Capacity * _series.VectorSize;
            if (_series.DataSize != len)
            {
                Series result = _series.GetZeroSeries(len);
                for (var i = 0; i < Capacity; i++)
                {
                    result.SetDataAtIndex(i, GetSeriesAtIndex(i));
                }
                _series = result;
            }
			Sampler = new LineSampler(Sampler.Capacity);
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