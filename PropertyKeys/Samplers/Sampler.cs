using System;
using System.Linq;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
		public Slot[] SwizzleMap { get; set; }
		public int[] Strides { get; protected set; }

		public Sampler(Slot[] swizzleMap = null, int capacity = 1)
		{
			SwizzleMap = swizzleMap;
			Capacity = capacity;
			Strides = new int[Capacity];
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

		// todo: Why is this in sample? Needs to move to series, and a rect vs grid series can use different algorithms to generate values (needed).
        public virtual Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = series.GetVirtualValueAt(seriesT[i]).FloatDataAt(i);
            }
            return SeriesUtils.CreateSeriesOfType(series, result);
        }
        public virtual ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            return Swizzle(seriesT, seriesT);
        }

        public virtual int NeighborCount => 2;
		private int WrappedIndex(int index, int capacity) => index >= capacity ? 0 : index < 0 ? capacity - 1 : index;
        public virtual Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
            var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);
	        result.SetRawDataAt(0, series.GetVirtualValueAt(WrappedIndex(index - 1, Capacity), Capacity));
	        result.SetRawDataAt(1, series.GetVirtualValueAt(WrappedIndex(index + 1, Capacity), Capacity));
            return result;
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