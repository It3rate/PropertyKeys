using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class GridSampler : Sampler
	{
		protected int[] Strides { get; }

        public GridSampler(int[] strides, Slot[] swizzleMap = null) : base(swizzleMap)
        {
			Strides = strides;
            Capacity = strides[0];
            for (int i = 1; i < strides.Length; i++)
            {
                if (strides[i] != 0)
                {
                    Capacity *= strides[i];
                }
                else
                {
                    break;
                }
            }
        }

		public override Series GetValueAtIndex(Series series, int index)
        {
            // access special case directly to avoid index>t>index>t conversions
            var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, Capacity, index);
            return GetSeriesSample(series, seriesT);
		}
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        var result = seriesT.VectorSize == 1 ? SamplerUtils.GetMultipliedJaggedTFromT(Strides, Capacity, seriesT.X) : seriesT;
            return Swizzle(result);
        }
        
        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < Strides.Length) ? series.GetValueAtT(seriesT[i]).FloatDataAt(i) : 0;
			}

			return SeriesUtils.CreateSeriesOfType(series, result);
		}
	}
}