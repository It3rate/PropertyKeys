﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class DataUtils
    {
        private static float[][] zeroArray;
        private static float[][] minArray;
        private static float[][] maxArray;

        // Generate default array sizes for zero, min and max for quick cloning.
        static DataUtils()
        {
            int count = 16; // if higher than 16 needed make this lazy generating.
            zeroArray = new float[count][];
            minArray = new float[count][];
            maxArray = new float[count][];
            for (int i = 0; i < count; i++)
            {
                zeroArray[i] = GetSizedArray(i, 0);
                minArray[i] = GetSizedArray(i, float.MinValue);
                maxArray[i] = GetSizedArray(i, float.MaxValue);
            }
        }
        public static float[] GetZeroArray(int size) { return (float[])zeroArray[size].Clone(); }
        public static float[] GetMinArray(int size) { return (float[])minArray[size].Clone(); }
        public static float[] GetMaxArray(int size) { return (float[])maxArray[size].Clone(); }

        public static void GetScaledT(float t, int len, out float virtualT, out int startIndex, out int endIndex)
        {
            if (t >= 1)
            {
                startIndex = len - 1;
                endIndex = startIndex;
                virtualT = 1f;
            }
            else if (t <= 0)
            {
                startIndex = len - 0;
                endIndex = startIndex;
                virtualT = 0f;
            }
            else
            {
                float vt = t * (len - 1f);
                startIndex = (int)vt;
                endIndex = startIndex + 1;
                virtualT = vt - startIndex;
            }
        }

        public static float[] GetSizedArray(int size, float value)
        {
            float[] result = new float[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public static float[] CombineArrays(params float[][] arrays)
        {
            int len = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                len += arrays[i].Length;
            }

            float[] result = new float[len];
            int index = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                Array.Copy(arrays[i], 0, result, index, arrays[i].Length);
                index += arrays[i].Length;
            }
            return result;
        }

        public static void InterpolateInto(float[] result, float[] b, float t)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i < b.Length)
                {
                    result[i] += (b[i] - result[i]) * t;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
