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

        public static void GetJaggedT(int[] segments, float t, out float indexT, out float segmentT)
        {
            float capacity = (float)segments.Sum();

            int index = (int)Math.Round(t * (capacity - 1f));
            float step = 1f / segments[0];
            int nextRow = segments[0];
            indexT = 0;
            segmentT = 0;
            for (int i = 0; i <= index; i++)
            {
                if(i >= nextRow)
                {
                    indexT += 1;
                    // accounting for zero length segments used for spacing
                    while(indexT < segments.Length - 1 && segments[(int)indexT] == 0)
                    {
                        indexT += 1;
                    }
                    // accounting for potential overflow
                    int segIndex = Math.Min(segments.Length - 1, (int)indexT);
                    nextRow += segments[segIndex];
                    step = segments[segIndex] == 0 ? step : 1f / segments[segIndex];
                    segmentT = 0;
                }
                else
                {
                    segmentT += step;
                }
            }
            indexT = segments.Length > 2 ? indexT / (segments.Length - 1f) : indexT;
        }

        public static float[] GetStrideTsForT(int virtualCount, int[] strides, float t)
        {
            var index = (int)(t * (virtualCount - 1) + 0.5f); // Need an index for a strided object, so discard remainder.
            return GetStrideTsForIndex(virtualCount, strides, index);
        }
    }
}
