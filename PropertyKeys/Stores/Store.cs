using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
		protected Series _series;

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

		public Series this[int index] => GetValuesAtIndex(index);

        public override Series GetFullSeries(int index)
        {
            return _series;
        }

		public override Series GetValuesAtIndex(int index)
		{
			return Sampler.GetValueAtIndex(_series, index);
		}

		public override Series GetValuesAtT(float t)
		{
			return Sampler.GetValuesAtT(_series, t);
		}
        
        public override ParametricSeries GetSampledTs(float t)
        {
            return Sampler.GetSampledTs(t);
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
                Series result = _series.GetZeroSeries(Capacity);
                for (var i = 0; i < Capacity; i++)
                {
	                float t = i / (float) (Capacity - 1);
                    result.SetSeriesAtIndex(i, GetValuesAtT(t));
                }
                _series = result;
            }
			Sampler = new LineSampler(Sampler.Capacity);
		}
		
		public static Store CreateItemStore(int count)
		{
			IntSeries result = new IntSeries(1, 0, count - 1);
			return result.CreateLinearStore(count);
		}
    }
}