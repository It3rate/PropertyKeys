﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class SamplerUtils
    {

        public static int[] GetDimsForIndex(int virtualCount, int[] strides, int index)
        {
            var count = Math.Max(0, Math.Min(virtualCount - 1, index));
            var slot = 0;
            var dSize = 1;
            for (var i = 0; i < strides.Length; i++)
            {
                if (strides[i] > 0)
                {
                    dSize *= strides[i];
                    slot++;
                }
            }

            var result = new int[slot + 1];
            for (var i = slot; i >= 0; i--)
            {
                result[i] = count / dSize;
                count -= result[i] * dSize;
                if (i > 0)
                {
                    dSize /= strides[i - 1];
                }
            }

            return result;
        }

        public static void GetJaggedT(int[] segments, float t, out float indexT, out float segmentT)
        {
            int index = 0;
            float capacity = (float)segments.Sum();
            //float capacity = (float)segments[0];
            //for (int i = 1; i < segments.Length; i++) capacity += segments[i];

            float ratio = segments[0] / capacity;
            while (t - ratio > 0 && index < segments.Length - 1)
            {
                t -= ratio;
                index++;
                ratio = segments[index] / capacity;
            }
            indexT = index / (float)segments.Length;
            segmentT = t / ratio;
        }

        public static float[] GetStrideTsForIndex(int virtualCount, int[] strides, int index)
        {
            var indexes = GetDimsForIndex(virtualCount, strides, index);
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

        public static float[] GetStrideTsForT(int virtualCount, int[] strides, float t)
        {
            var index = (int)(t * (virtualCount - 1) + 0.5f); // Need an index for a strided object, so discard remainder.
            return GetStrideTsForIndex(virtualCount, strides, index);
        }

        public static Sampler CreateSampler(SampleType sampleType, int[] strides = null)
        {
            Sampler result;
            switch (sampleType)
            {
                case SampleType.Line:
                    result = new LineSampler();
                    break;
                case SampleType.Grid:
                    result = new GridSampler(strides);
                    break;
                case SampleType.Ring:
                    result = new RingSampler(strides);
                    break;
                case SampleType.Hexagon:
                    result = new HexagonSampler(strides);
                    break;
                default:
                    result = new LineSampler();
                    break;
            }

            return result;
        }
    }
}
