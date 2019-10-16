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

        public static int[] GetPositionsForIndex(int virtualCount, int[] strides, int index)
        {
            var result = new int[strides.Length];
	        var count = Math.Max(0, Math.Min(virtualCount - 1, index));
	        for (var i = strides.Length - 1; i >= 0; i--)
	        {
		        int dimSize = 1;
		        for (int j = 0; j < i; j++)
		        {
			        dimSize *= strides[j];
		        }
		        result[i] = count / dimSize;
		        count -= result[i] * dimSize;
	        }
	        return result;
        }

        public static float[] GetStrideTsForIndex(int virtualCount, int[] strides, int index)
        {
            var indexes = GetPositionsForIndex(virtualCount, strides, index);
            var dSize = 1;
            var maxLen = virtualCount - 1;
            var result = new float[indexes.Length];
            for (var i = 0; i < indexes.Length; i++)
            {
                if (i < strides.Length && strides[i] > 0)
                {
                    result[i] = indexes[i] / (float)(strides[i] - 1);
                    dSize *= strides[i];
                }
                else
                {
                    result[i] = (float)(indexes[i] / Math.Floor(maxLen / (float)dSize));
                    break;
                }
            }

            return result;
        }

        public static void SummedIndexAndRemainder(int stride, int partialIndex, ref int index, out int remainder)
        {
            if(partialIndex > stride)
            {
                remainder = partialIndex - stride;
                index++;
            }
            else
            {
                remainder = partialIndex;
            }
        }
        public static void GetSummedJaggedT(int[] segments, int index, out float indexT, out float remainder)
        {
            indexT = 0;
            remainder = 0;
            int partialIndex = index;
            int refRemainder = 0;
            int refIndex = 0;
            int curSeg = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                curSeg = segments[i];
                SummedIndexAndRemainder(curSeg, partialIndex, ref refIndex, out refRemainder);
                partialIndex = refRemainder;
                if(refRemainder < curSeg)
                {
                    break;
                }
            }
            // the index could overflow the sum of segments, so finish the calculation regardless
            indexT = segments.Length > 1 ? refIndex / (float)(segments.Length - 1f) : 0;
            remainder = curSeg > 0 ? refRemainder / (float)curSeg : 0;
        }

        public static void DividedIndexAndRemainder(int stride, float t, out int index, out float remainder)
        {
            float position = t * (stride - 1f);
            index = (int)(position + .00001f);
            remainder = position - index;
            remainder = (remainder + .00001f) > 1f ? 0 : remainder;
        }
        public static void GetDividedJaggedT(int[] segments, float t, out float indexT, out float remainder)
        {
            float varT = t;
            indexT = 0;
            remainder = t;
            for (int i = 0; i < segments.Length; i++)
            {
                DividedIndexAndRemainder(segments[i], varT, out int index, out remainder);
                indexT = segments[i] > 0 ? index / (float)segments[i] : 0;
                varT = remainder;
            }
            GetJaggedTx(segments, t, out float rowT, out float segT);
        }

        public static void GetJaggedTx(int[] segments, float t, out float rowIndex, out float segmentT)
        {
            float capacity = (float)segments.Sum();

            int index = (int)Math.Round(t * (capacity - 1f));
            float step = 1f / segments[0];
            int nextRow = segments[0];
            rowIndex = 0;
            segmentT = 0;
            for (int i = 0; i <= index; i++)
            {
                if(i >= nextRow)
                {
                    rowIndex += 1;
                    // accounting for zero length segments used for spacing
                    while(rowIndex < segments.Length - 1 && segments[(int)rowIndex] == 0)
                    {
                        rowIndex += 1;
                    }
                    // accounting for potential overflow
                    int segIndex = Math.Min(segments.Length - 1, (int)rowIndex);
                    nextRow += segments[segIndex];
                    step = segments[segIndex] == 0 ? step : 1f / segments[segIndex];
                    segmentT = 0;
                }
                else if(i > 0)
                {
                    segmentT += step;
                }
            }
            rowIndex = segments.Length > 2 ? rowIndex / (segments.Length - 1f) : rowIndex;
        }

        public static float[] GetStrideTsForT(int virtualCount, int[] strides, float t)
        {
            var index = (int)(t * (virtualCount - 1) + 0.5f); // Need an index for a strided object, so discard remainder.
            return GetStrideTsForIndex(virtualCount, strides, index);
        }
    }
}
