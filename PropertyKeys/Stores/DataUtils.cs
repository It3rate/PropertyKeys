using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class DataUtils
    {
        public const float TOLERANCE = 0.00001f;
        public static readonly Random Random = new Random();

        private static readonly float[][] zeroFloatArray;
        private static readonly float[][] minFloatArray;
        private static readonly float[][] maxFloatArray;
        private static readonly int[][] zeroIntArray;
        private static readonly int[][] minIntArray;
        private static readonly int[][] maxIntArray;

        // Generate default array sizes for zero, min and max for quick cloning.
        static DataUtils()
        {
            int count = 16; // if higher than 16 needed make this lazy generating.
            zeroFloatArray = new float[count][];
            minFloatArray = new float[count][];
            maxFloatArray = new float[count][];
            zeroIntArray = new int[count][];
            minIntArray = new int[count][];
            maxIntArray = new int[count][];
            for (int i = 0; i < count; i++)
            {
                zeroFloatArray[i] = GetSizedFloatArray(i, 0);
                minFloatArray[i] = GetSizedFloatArray(i, float.MinValue);
                maxFloatArray[i] = GetSizedFloatArray(i, float.MaxValue);
                zeroIntArray[i] = GetSizedIntArray(i, 0);
                minIntArray[i] = GetSizedIntArray(i, int.MinValue);
                maxIntArray[i] = GetSizedIntArray(i, int.MaxValue);
            }
        }
        public static float[] GetFloatZeroArray(int size) { return (float[])zeroFloatArray[size].Clone(); }
        public static float[] GetFloatMinArray(int size) { return (float[])minFloatArray[size].Clone(); }
        public static float[] GetFloatMaxArray(int size) { return (float[])maxFloatArray[size].Clone(); }
        public static int[] GetIntZeroArray(int size) { return (int[])zeroIntArray[size].Clone(); }
        public static int[] GetIntMinArray(int size) { return (int[])minIntArray[size].Clone(); }
        public static int[] GetIntMaxArray(int size) { return (int[])maxIntArray[size].Clone(); }

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
                startIndex = 0;
                endIndex = startIndex;
                virtualT = 0f;
            }
            else
            {
                float vt = t * (len - 1f);
                startIndex = (int)vt;
                endIndex = Math.Min(len - 1, startIndex + 1);
                virtualT = vt - startIndex;
            }
        }

        public static float[] GetSizedFloatArray(int size, float value)
        {
            float[] result = new float[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = value;
            }
            return result;
        }
        public static int[] GetSizedIntArray(int size, int value)
        {
            int[] result = new int[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public static float[] CombineFloatArrays(params float[][] arrays)
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

        public static int[] CombineIntArrays(params int[][] arrays)
        {
            int len = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                len += arrays[i].Length;
            }

            int[] result = new int[len];
            int index = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                Array.Copy(arrays[i], 0, result, index, arrays[i].Length);
                index += arrays[i].Length;
            }
            return result;
        }

        public static void SubtractFloatArrayFrom(float[] result, float[] b)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i < b.Length)
                {
                    result[i] -= b[i];
                }
                else
                {
                    break;
                }
            }
        }
        public static void SubtractIntArrayFrom(int[] result, int[] b)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i < b.Length)
                {
                    result[i] -= b[i];
                }
                else
                {
                    break;
                }
            }
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

    public static class ArrayExtension
    {
        public static float[] ToFloat(this int[] values) => Array.ConvertAll(values, x => (float)x);
        public static int[] ToInt(this float[] values) => Array.ConvertAll(values, x => (int)x);
    }
}
