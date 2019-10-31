using DataArcs.SeriesData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
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
