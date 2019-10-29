using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class HexagonSampler : GridSampler
	{
		public HexagonSampler(int[] strides, Slot[] swizzleMap = null) : base(strides, swizzleMap) { }

        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
		{
			var result = base.GetSeriesSample(series, seriesT);
			var curRow = SamplerUtils.IndexFromT(Strides[1], seriesT.Y);// (int)Math.Round(seriesT.Y * (Strides[1] - 1));
            if ((curRow & 1) == 1)
            {
	            var data = result.FloatData;
	            data[0] += series.Size.X / (Strides[0] - 1f) * 0.5f;
                result = SeriesUtils.CreateSeriesOfType(series, data);
            }
	        return result;
		}
	}
}