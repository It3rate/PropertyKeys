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
	        var indexT = SamplerUtils.TFromIndex(Capacity, index); // index / (Capacity - 1f);
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
            return Swizzle(seriesT, seriesT);
        }

        /// <summary>
        /// Generate a result with values mapped according the internal SwizzleMap.
        /// Extra values can be preserved if the extra series is longer than the source, and the swizzle map calls for them.
        /// </summary>
        /// <param name="source">The series to be mapped using the SwizzleMap.</param>
        /// <param name="extra">Extra values that could be used if the SwizzleMap asks for slots out of the source range.</param>
        /// <returns></returns>
        public ParametricSeries Swizzle(ParametricSeries source, ParametricSeries extra)
        {
	        ParametricSeries result = source;
	        if (SwizzleMap != null)
            {
                int len = SwizzleMap.Length;
                result = new ParametricSeries(len, new float[len]);
		        for (int i = 0; i < len; i++)
		        {
			        int index = (int)SwizzleMap[i];
			        result[i] = index < source.VectorSize ? source[index] : index < extra.VectorSize ? extra[index] : source[len - 1];
		        }
	        }
            else if(source.VectorSize < extra.VectorSize)
            {
                // No slots, but don't destroy data in the extra source if it isn't overwritten
                result = (ParametricSeries)extra.Copy();
                for (int i = 0; i < source.VectorSize; i++)
                {
                    result[i] = source[i];
                }
            }

	        return result;
        }

    }
}