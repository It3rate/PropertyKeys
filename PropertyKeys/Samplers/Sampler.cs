using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
		public Slot[] SwizzleMap { get; set; }

		public Sampler(Slot[] swizzleMap = null, int capacity = 1)
		{
			SwizzleMap = swizzleMap;
			Capacity = capacity;
        }

        public virtual Series GetValueAtIndex(Series series, int index)
        {
            var indexT = index / (Capacity - 1f);
            return GetValuesAtT(series, indexT);
        }

        public virtual Series GetValuesAtT(Series series, float t)
        {
            var seriesT = GetSampledTs(new ParametricSeries(1, t));
            return GetSeriesSample(series, seriesT);
        }
        public virtual Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = series.GetValueAtT(seriesT[i]).FloatDataAt(i);
            }
            return SeriesUtils.CreateSeriesOfType(series, result);
        }
        public virtual ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            return Swizzle(seriesT);
        }

        public ParametricSeries Swizzle(ParametricSeries series)
        {
	        ParametricSeries result;
	        if (SwizzleMap != null)
	        {
		        int len = SwizzleMap.Length;
		        result = new ParametricSeries(len, new float[len]);
		        for (int i = 0; i < len; i++)
		        {
			        int index = (int)SwizzleMap[i];
			        index = Math.Max(0, Math.Min(len - 1, index));
			        result[i] = series[index];
		        }
	        }
	        else
	        {
		        result = series;
	        }

	        return result;
        }

    }
}