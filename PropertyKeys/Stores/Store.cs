using System;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using System.Collections;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Stores
{
	public class Store : StoreBase
    {

        protected Store() { }

        //public Store(Store store):base(store)
        //{
        //    _series = store?._series;
        //    Sampler = store?.Sampler ?? new LineSampler();
        //    CombineFunction = store?.CombineFunction ?? CombineFunction.Add;
        //    CombineTarget = store?.CombineTarget ?? CombineTarget.Destination;
        //}

        public Store(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Replace) : base(series)
		{
			Sampler = sampler ?? new LineSampler(series.Count);
			CombineFunction = combineFunction;
        }

		public Series this[int index] => GetValuesAtIndex(index);

		public override Series GetSeriesRef()
		{
			return Series;
        }
		public override void SetFullSeries(Series value)
		{
			Series = value;
		}

        public override Series GetValuesAtIndex(int index)
		{
			return ShouldInterpolate ? Series.GetRawDataAt(index) : Sampler.GetValueAtIndex(Series, index);
		}

		public override Series GetValuesAtT(float t)
		{
			// GetValuesAtT checks if it was baked, this implies the 't' maps to the baked series.
            return ShouldInterpolate ? Series.GetRawDataAt(t) : Sampler.GetValuesAtT(Series, t);
		}
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        return Sampler.GetSampledTs(seriesT);
        }


        public override void Update(float deltaTime)
		{
			Series.Update(deltaTime);
		}

        public override void ResetData()
		{
			Series.ResetData();
		}

		public override void BakeData()
        {
            var len = Capacity * Series.VectorSize;
            if (Series.DataSize != len)
            {
	            Series result = SeriesUtils.CreateSeriesOfType(Series.Type, Series.VectorSize, Capacity, 0f);
	            
                for (var i = 0; i < Capacity; i++)
                {
	                float t = i / (float) (Capacity - 1);
                    result.SetRawDataAt(i, GetValuesAtT(t));
                }
                Series = result;
            }
            ShouldInterpolate = Series.Type != SeriesType.Int;
		}

		public override IStore Clone()
		{
			return new Store(Series.Copy(), Sampler, CombineFunction);
		}
		public override void CopySeriesDataInto(IStore target)
		{
			Series targetSeries = target.GetSeriesRef();
			
            if (Series.Type == targetSeries.Type && Series.DataSize == targetSeries.DataSize)
			{
				if (Series.Type == SeriesType.Float)
				{
					Array.Copy(Series.FloatDataRef, target.GetSeriesRef().FloatDataRef, Series.DataSize);
				}
				else if (Series.Type == SeriesType.Int)
				{
					Array.Copy(Series.IntDataRef, target.GetSeriesRef().IntDataRef, Series.DataSize);
                }
			}
            else
            {
				target.SetFullSeries(Series.Copy());
            }
		}

        public static Store CreateItemStore(int count)
		{
			IntSeries result = new IntSeries(1, 0, count - 1);
			return result.CreateLinearStore(count);
        }
        private static readonly IStore _emptyItemStore = IntSeries.Empty.Store();
        public static IStore EmptyItemStore => _emptyItemStore;
    }
}