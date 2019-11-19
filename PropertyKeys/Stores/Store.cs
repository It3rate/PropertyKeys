using System;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {
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
			Sampler = sampler ?? new LineSampler(series.Count);
			CombineFunction = combineFunction;
			CombineTarget = combineTarget;
        }

		public Series this[int index] => GetValuesAtIndex(index);

		public override Series GetSeriesRef()
		{
			return _series;
        }
		public override void SetFullSeries(Series value)
		{
			_series = value;
		}

        public override Series GetValuesAtIndex(int index)
		{
			return ShouldIterpolate ? _series.GetRawDataAt(index) : Sampler.GetValueAtIndex(_series, index);
		}

		public override Series GetValuesAtT(float t)
		{
			// GetValuesAtT checks if it was baked, this implies the 't' maps to the baked series.
            return ShouldIterpolate ? _series.GetRawDataAt(t) : Sampler.GetValuesAtT(_series, t);
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
	            Series result = SeriesUtils.CreateSeriesOfType(_series.Type, _series.VectorSize, Capacity, 0f);
	            
                for (var i = 0; i < Capacity; i++)
                {
	                float t = i / (float) (Capacity - 1);
                    result.SetRawDataAt(i, GetValuesAtT(t));
                }
                _series = result;
            }
            ShouldIterpolate = _series.Type != SeriesType.Int;
		}

		public override IStore Clone()
		{
			return new Store(_series.Copy(), Sampler, CombineFunction, CombineTarget);
		}
		public override void CopySeriesDataInto(IStore target)
		{
			Series targetSeries = target.GetSeriesRef();
			
            if (_series.Type == targetSeries.Type && _series.DataSize == targetSeries.DataSize)
			{
				if (_series.Type == SeriesType.Float)
				{
					Array.Copy(_series.FloatDataRef, target.GetSeriesRef().FloatDataRef, _series.DataSize);
				}
				else if (_series.Type == SeriesType.Int)
				{
					Array.Copy(_series.IntDataRef, target.GetSeriesRef().IntDataRef, _series.DataSize);
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