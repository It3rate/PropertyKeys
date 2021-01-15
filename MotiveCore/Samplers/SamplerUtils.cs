using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Motive.SeriesData;

namespace Motive.Samplers
{
	public enum ClampType
	{
		None = 0, // -1..0..1..2
		Wrap, // 0..1->0..1..
		WrapRight, // 1..0->1..0->1...
        Mirror, // 0..1..0..1..
		ClampAtZero, // 0..0..1..2
		ClampAtOne, // -1..0..1..1
		Clamp, // 0..0..1..1..1..
	}
	public enum AlignmentType
	{
		Left = 0,
		Right,
		Centered,
		Justified,
	}
	public enum GrowthType
	{
		Product = 0,
		Widest,
		Sum,
	}

    public class SamplerUtils
    {
        public const float TOLERANCE = 0.00001f;

		// todo: reconcile indexFromT with TFromIndex -- one returns 0-1 inclusive, the second exclusive.
	    public static int IndexFromT(int capacity, float t)
	    {
		    return Math.Max(0, Math.Min(capacity - 1, (int)Math.Round(t * (capacity - 1f))));
	    }
	    public static float TFromIndex(int capacity, int index)
	    {
		    return index / (capacity - 1f);
	    }

	    public static ParametricSeries DistributeTBySummedSampler(ParametricSeries seriesT, Sampler sampler, out int[] positions)
	    {
			// This is inherently 2D because is defines the row sizes.
		    var result = new float[2];
		    positions = new int[2];
		    int maxStride = sampler.Strides.Max();
		    int strideSum = sampler.Strides.Sum();
            var rows = sampler.Strides.Length;
            float minSampleSize = 1f / strideSum;
            int rowLength;
		    bool hasMultipleT = seriesT.Count > 1;
		    if (hasMultipleT)
		    {
			    positions[1] = (int)Math.Floor(seriesT[1] * rows);
			    rowLength = sampler.Strides[positions[1]];
			    positions[0] = (int)Math.Floor(seriesT[0] * rowLength);
		    }
		    else
		    {
			    float total = 0;
			    for (int i = 0; i < rows; i++)
			    {
				    rowLength = sampler.Strides[i];
				    if (total + minSampleSize * rowLength < seriesT[0])
				    {
					    total += minSampleSize * rowLength;
				    }
				    else
				    {
					    float rem = seriesT[0] - total;

					    positions[0] = (int)Math.Floor(rem * strideSum);
					    positions[1] = i;
					    result[0] = positions[0] / (float)maxStride;
						result[1] = i / (rows - 1f);
					    break;
				    }
			    }
		    }
			// only makes sense to clamp element one when rows are manually defined.
            if (sampler.ClampTypes?.Length > 0)
            {
                switch (sampler.ClampTypes[0])
                {
                    case ClampType.WrapRight:
                        positions[0] = (maxStride - 1) - positions[0];
                        break;
                    case ClampType.Mirror:
                        positions[0] = (positions[1] & 1) == 0 ? positions[0] : (maxStride - 1) - positions[0];
                        break;
                    case ClampType.ClampAtZero:
                        positions[0] = (positions[0] < 0) ? 0 : positions[0];
                        break;
                    case ClampType.ClampAtOne:
                        positions[0] = (positions[0] > 1) ? 1 : positions[0];
                        break;
                    case ClampType.Clamp:
                        positions[0] = (positions[0] < 0) ? 0 : (positions[0] > 1) ? 1 : positions[0];
                        break;
                }
                result[0] = positions[0] / (float)maxStride;
            }
            return new ParametricSeries(2, result);
	    }
        public static ParametricSeries DistributeTBySampler(ParametricSeries seriesT, Sampler sampler, out int[] positions)
	    {
		    var len = sampler.Strides.Length;
		    var result = new float[len];
		    positions = new int[len];
			bool hasMultipleT = seriesT.Count > 1;
            float minSegSize = 1f / (sampler.SampleCount - 1);
		    float offsetT = seriesT[0] + minSegSize / 2f;
		    for (int i = 0; i < len; i++)
		    {
				// allow input of multiple parameters - use position per stride in this case.
				// if mismatched lengths, just use last value available (basically error condition).
			    if (hasMultipleT && seriesT.Count > i)
			    {
				    minSegSize = 1f / (sampler.Strides[i] - 1);
				    offsetT = seriesT[i] + minSegSize / 2f;
                }
			    int curStride = sampler.Strides[i];
			    float div = offsetT / minSegSize;
			    int pos = (int)Math.Floor(div);
			    int index = pos % curStride;
			    if (sampler.ClampTypes.Length > i)
			    {
				    switch (sampler.ClampTypes[i])
				    {
					    case ClampType.None:
						    break;
					    case ClampType.Wrap:
						    pos = index;
						    break;
					    case ClampType.WrapRight:
						    pos = curStride - index;
						    break;
					    case ClampType.Mirror:
						    pos = ((index / curStride) & 1) == 0 ? index : curStride - index;
						    break;
					    case ClampType.ClampAtZero:
						    pos = (pos < 0) ? 0 : index;
						    break;
					    case ClampType.ClampAtOne:
						    pos = (pos > 1) ? 1 : index;
						    break;
					    case ClampType.Clamp:
						    pos = (pos < 0) ? 0 : (pos > 1) ? 1 : index;
						    break;
				    }
			    }
			    positions[i] = pos;
			    result[i] = pos / (float)(curStride - 1);
			    minSegSize *= curStride;
            }
		    return new ParametricSeries(len, result);
	    }

	    public static IntSeries DistributeBySampler(IntSeries series, Sampler sampler, out int[] positions)
	    {
		    var len = sampler.Strides.Length;
		    positions = new int[len];
		    bool hasMultiple = series.Count > 1;
		    int minSegSize = sampler.SampleCount - 1;
		    int offset = series.IntDataAt(0);
		    for (int i = 0; i < len; i++)
		    {
			    // allow input of multiple parameters - use position per stride in this case.
			    // if mismatched lengths, just use last value available (basically error condition).
			    if (hasMultiple && series.Count > i)
			    {
				    minSegSize = sampler.Strides[i] - 1;
				    offset = series.IntDataAt(i);
			    }
			    int curStride = sampler.Strides[i];
			    int pos = offset / minSegSize;
			    int index = pos % curStride;
			    if (sampler.ClampTypes.Length > i)
			    {
				    switch (sampler.ClampTypes[i])
				    {
					    case ClampType.None:
						    break;
					    case ClampType.Wrap:
						    pos = index;
						    break;
					    case ClampType.WrapRight:
						    pos = curStride - index;
						    break;
					    case ClampType.Mirror:
						    pos = ((index / curStride) & 1) == 0 ? index : curStride - index;
						    break;
					    case ClampType.ClampAtZero:
						    pos = (pos < 0) ? 0 : index;
						    break;
					    case ClampType.ClampAtOne:
						    pos = (pos > 1) ? 1 : index;
						    break;
					    case ClampType.Clamp:
						    pos = (pos < 0) ? 0 : (pos > 1) ? 1 : index;
						    break;
				    }
			    }
			    positions[i] = pos;
			    minSegSize *= curStride;
		    }
		    return new IntSeries(len, positions);
	    }
        /// <summary>
        /// Returns normalized indexes into segmented array based on index and size. Passed size can be virtual (larger than implied segments total).
        /// </summary>
        /// <param name="segments">The segments (e.g. row/cols) for the container.</param>
        /// <param name="capacity">Virtual size of element container.</param>
        /// <param name="index">The index into the segments.</param>
        /// <returns></returns>
        public static ParametricSeries GetMultipliedJaggedT(int[] segments, int capacity, int index)
        {
            var indexes = GetPositionsForIndex(segments, capacity, index);
            var dSize = 1;
            var maxLen = capacity - 1;
            var result = new float[indexes.Length];
            for (var i = 0; i < indexes.Length; i++)
            {
                if (i < segments.Length && segments[i] > 0)
                {
                    result[i] = indexes[i] / (float)(segments[i] - 1); // needs to be 0-1, recursive like jagged sampler
                    dSize *= segments[i];
                }
                else
                {
                    result[i] = (float)(indexes[i] / Math.Floor(maxLen / (float)dSize));
                    break;
                }
            }

            return new ParametricSeries(indexes.Length, result);
        }

        public static ParametricSeries GetMultipliedJaggedTFromT(int[] segments, int capacity, float t)
        {
	        var index = IndexFromT(capacity, t); // (int)Math.Round(t * (capacity - 1f));
            //var index = (int)(t * (capacity - 1) + 0.5f); // Need an index for a strided object, so discard remainder.
            return GetMultipliedJaggedT(segments, capacity, index);
        }

        public static int[] GetPositionsForIndex(int[] segments, int capacity, int index)
        {
            var result = new int[segments.Length];
            var count = Math.Max(0, Math.Min(capacity - 1, index));
            for (var i = segments.Length - 1; i >= 0; i--)
            {
                int dimSize = 1;
                for (int j = 0; j < i; j++)
                {
                    dimSize *= segments[j];
                }
                result[i] = count / dimSize;
                count -= result[i] * dimSize;
            }
            return result;
        }

        public static int GetClampedValue(int index, int count, ClampType clampType)
        {
	        int result = index;
	        if (index < 0 || index >= count)
	        {
		        int mod = index % count;
		        switch (clampType)
		        {
			        case ClampType.None:
				        break;
			        case ClampType.Wrap:
				        result = mod;
				        break;
			        case ClampType.WrapRight:
				        result = count - mod;
				        break;
			        case ClampType.Mirror:
				        result = ((index / count) & 1) == 0 ? mod : (count - 1) - mod;
				        break;
			        case ClampType.ClampAtZero:
				        result = (result < 0) ? 0 : index;
				        break;
			        case ClampType.ClampAtOne:
				        result = (result > 1) ? 1 : index;
				        break;
			        case ClampType.Clamp:
				        result = (index < 0) ? 0 : (index > 1) ? 1 : index;
				        break;
		        }
	        }

	        return result;
        }

		/// <summary>
        /// Returns normalized indexes into a jagged set of elements at the passed index.
        /// Normally the last parameter will be 0-1 in order to pass to children,
        /// but in the case of eg grid sampling it can return a normalized index based on the count of the last segment.
        /// </summary>
        /// <param name="segments">The lengths of elements in a jagged array.</param>
        /// <param name="index">The index to sample.</param>
        /// <param name="isLastSegmentDiscrete">Decides if the last parameter is 0-1 (default), or discrete based on the count of the last sampled segment.</param>
        /// <returns></returns>
        public static ParametricSeries GetSummedJaggedT(int[] segments, int index, bool isLastSegmentDiscrete = false)
        {
            int partialIndex = index;
            int refRemainder = index;
            int refIndex = 0;
            int lastSegment = 0;
            foreach (var seg in segments)
            {
	            if(refRemainder < seg)
	            {
					lastSegment = seg;
		            break;
	            }
	            SummedIndexAndRemainder(seg, partialIndex, ref refIndex, out refRemainder);
	            partialIndex = refRemainder;
            }
            // the index could overflow the sum of segments, so finish the calculation regardless
            float indexT = segments.Length > 0 ? refIndex / (segments.Length - 0f) : 0;
            float discreteAdjust = isLastSegmentDiscrete ? 0f : 1f;
            float remainder = lastSegment > 1 ? refRemainder / (lastSegment - discreteAdjust) : 0;
            return new ParametricSeries(2, indexT, remainder);
        }
        public static void SummedIndexAndRemainder(int stride, int partialIndex, ref int index, out int remainder)
        {
            if(partialIndex >= stride)
            {
                remainder = partialIndex - stride;

                index++;
            }
            else
            {
                remainder = partialIndex;
            }
        }

		/// <summary>
        /// Gets interplolated position along a given length, the start index and how far into it to sample.
        /// Assures index is greater than zero and less than len.
        /// Remainder may be greater than one if t is greater than one.
        /// </summary>
        /// <param name="len">Total length to consider.</param>
        /// <param name="t">Interpolation point.</param>
        /// <param name="startIndex">Returned index into the length.</param>
        /// <param name="remainder">Returned remainder into the length.</param>
        public static void InterpolatedIndexAndRemainder(int len, float t, out int startIndex, out float remainder)
        {
	        var dist = t * (len - 1f);
	        dist = Math.Max(dist, 0);
	        startIndex = (int)Math.Max(0, Math.Min(dist, len - 2));
	        remainder = dist - startIndex;
	        remainder = (remainder < TOLERANCE) ? 0 : remainder;
        }
    }
}
