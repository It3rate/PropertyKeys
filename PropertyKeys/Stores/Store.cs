using System;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
	    public bool IsBaked { get; set; } = false;
        protected Series _series;

        protected Store() { }

        //public Store(Store store):base(store)
        //{
        //    _series = store?._series;
        //    Sampler = store?.Sampler ?? new LineSampler();
        //    CombineFunction = store?.CombineFunction ?? CombineFunction.Add;
        //    CombineTarget = store?.CombineTarget ?? CombineTarget.Destination;
        //}

        public Store(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Replace, 
            CombineTarget combineTarget = CombineTarget.Destination)
		{
			_series = series;
			Sampler = sampler ?? new LineSampler();
			CombineFunction = combineFunction;
			CombineTarget = combineTarget;
        }

		public Series this[int index] => GetValuesAtIndex(index);

		public override Series GetFullSeries()
		{
			return _series;
        }
		public override void SetFullSeries(Series value)
		{
			_series = value;
		}

        public override Series GetValuesAtIndex(int index)
		{
			return IsBaked ? _series.GetValueAtVirtualIndex(index, Capacity) : Sampler.GetValueAtIndex(_series, index);
		}

		public override Series GetValuesAtT(float t)
		{
			// GetValuesAtT checks if it was baked, this implies the 't' maps to the baked series.
            return IsBaked ? _series.GetValueAtT(t) : Sampler.GetValuesAtT(_series, t);
		}
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        return Sampler.GetSampledTs(seriesT);
        }


        public override void Update(float deltaTime)
		{
			_series.Update(deltaTime);
		}

        public override void ResetData()
		{
			_series.ResetData();
		}

		public override void BakeData()
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
                IsBaked = true;
            }
		}

		public override IStore Clone()
		{
			return new Store(_series.Copy(), Sampler, CombineFunction, CombineTarget);
		}
		public override void CopySeriesDataInto(IStore target)
		{
			Series targetSeries = target.GetFullSeries();
			
            if (_series.Type == targetSeries.Type && _series.DataSize == targetSeries.DataSize)
			{
				if (_series.Type == SeriesType.Float)
				{
					Array.Copy(_series.FloatDataRef, target.GetFullSeries().FloatDataRef, _series.DataSize);
				}
				else if (_series.Type == SeriesType.Int)
				{
					Array.Copy(_series.IntDataRef, target.GetFullSeries().IntDataRef, _series.DataSize);
                }
			}
            else
            {
				target.SetFullSeries(_series.Copy());
            }
		}

        public static Store CreateItemStore(int count)
		{
			IntSeries result = new IntSeries(1, 0, count - 1);
			return result.CreateLinearStore(count);
        }
        private static readonly IStore _emptyItemStore = IntSeries.Empty.Store;
        public static IStore EmptyItemStore => _emptyItemStore;
    }
}