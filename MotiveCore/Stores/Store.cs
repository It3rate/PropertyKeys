using System;
using System.Collections;
using Motive.Samplers.Utils;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Stores
{
	public class Store : StoreBase
    {
	    protected Store() { }
        public Store(ISeries series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Replace) : base(series)
		{
			Sampler = sampler ?? new LinearSampler(series.Count);
			CombineFunction = combineFunction;
        }

        public Store(int seriesId, int sampleId, CombineFunction combineFunction) : base(seriesId, sampleId, combineFunction)
        {
        }

		public override ISeries GetSeriesRef()
		{
			return Series;
        }

		public override ISeries GetValuesAtT(float t)
		{
            // GetValuesAtT checks if it was baked, this implies the 't' maps to the baked series.
            return IsBaked ? Series.GetSeriesAt(t) : Sampler.GetValuesAtT(Series, t);
		}
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        return Sampler.GetSampledTs(seriesT);
        }


        public override void Update(double currentTime, double deltaTime)
		{
			Series.Update(currentTime, deltaTime);
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
	            ISeries result = SeriesUtils.CreateSeriesOfType(Series.Type, Series.VectorSize, Capacity, 0f);
	            
                for (var i = 0; i < Capacity; i++)
                {
	                float t = i / (float) (Capacity - 1);
                    result.SetSeriesAt(i, GetValuesAtT(t));
                }
                Series = result;
            }

            IsBaked = true;//Series.Type != SeriesType.Int;
		}

		public override IStore Clone()
		{
			return new Store((ISeries)Series.Copy(), Sampler, CombineFunction);
		}
		public override void CopySeriesDataInto(IStore target)
		{
			ISeries targetSeries = target.GetSeriesRef();
			
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
				target.SetFullSeries((ISeries)Series.Copy());
            }
		}

        public static Store CreateItemStore(int count)
		{
            // creates a series from 0 to count-1, these are the actual indexes, but will be interpolated from 0-1 to include all values.
            // From the outside, this should be identical to the case where every value is baked.
            // Question of whether this should go to n or n-1, probably n?
			IntSeries series = new IntSeries(1, 0, count - 1);
			var result = series.CreateLinearStore(count);
			return result;
		}
        private static readonly IStore _emptyItemStore = IntSeries.Empty.Store();
        public static IStore EmptyItemStore => _emptyItemStore;
    }
}