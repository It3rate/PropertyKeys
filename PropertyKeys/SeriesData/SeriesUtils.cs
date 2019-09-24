﻿using DataArcs.Stores;
using System;

namespace DataArcs.SeriesData
{
	public class SeriesUtils
	{
		public static void InterpolateInto(float[] result, float[] b, float t)
		{
			for (var i = 0; i < result.Length; i++)
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

		public static void Shuffle(Series series)
		{
			var len = series.DataSize;
			for (var i = 0; i < series.DataSize; i++)
			{
				var a = Random.Next(len);
				var b = Random.Next(len);
				var sa = series.GetDataAtIndex(a);
				var sb = series.GetDataAtIndex(b);
				series.SetDataAtIndex(a, sb);
				series.SetDataAtIndex(b, sa);
			}
		}

		public static Series Create(Series series, int[] values)
		{
			Series result;
			if (series.Type == SeriesType.Int)
			{
				result = new IntSeries(series.VectorSize, values);
			}
			else
			{
				result = new FloatSeries(series.VectorSize, values.ToFloat());
			}

			return result;
		}

		public static Series Create(Series series, float[] values)
		{
			Series result;
			if (series.Type == SeriesType.Int)
			{
				result = new IntSeries(series.VectorSize, values.ToInt());
			}
			else
			{
				result = new FloatSeries(series.VectorSize, values);
			}

			return result;
		}

		public static bool IsEqual(Series a, Series b)
		{
			var result = true;
			if (a.GetType() == b.GetType() && a.Count == b.Count && a.VectorSize == b.VectorSize)
			{
                if (a.Type == SeriesType.Float)
                {
                    for (var i = 0; i < a.DataSize; i++)
                    {
                        var delta = 0.0001f;
                        var ar = a.GetDataAtIndex(i).FloatData;
                        var br = b.GetDataAtIndex(i).FloatData;
                        for (var j = 0; j < ar.Length; j++)
                        {
                            if (Math.Abs(ar[j] - br[j]) > delta)
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < a.DataSize; i++)
                    {
                        var ar = a.GetDataAtIndex(i).IntData;
                        var br = b.GetDataAtIndex(i).IntData;
                        for (var j = 0; j < ar.Length; j++)
                        {
                            if (ar[j] != br[j])
                            {
                                result = false;
                                break;
                            }
                        }
                    }
				}
			}
			else
			{
				result = false;
			}

			return result;
		}

		public static FloatSeries GetZeroFloatSeries(int vectorSize, int elementCount)
		{
			return new FloatSeries(vectorSize, GetFloatZeroArray(vectorSize * elementCount));
		}

		public static IntSeries GetZeroIntSeries(int vectorSize, int elementCount)
		{
			return new IntSeries(vectorSize, GetIntZeroArray(vectorSize * elementCount));
		}


		public const float TOLERANCE = 0.00001f;
		public static readonly Random Random = new Random();
        
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
				var vt = t * (len - 1f);
				startIndex = (int) vt;
				endIndex = Math.Min(len - 1, startIndex + 1);
				virtualT = vt - startIndex;
			}
		}

		public static float[] CombineFloatArrays(params float[][] arrays)
		{
			var len = 0;
			for (var i = 0; i < arrays.Length; i++)
			{
				len += arrays[i].Length;
			}

			var result = new float[len];
			var index = 0;
			for (var i = 0; i < arrays.Length; i++)
			{
				Array.Copy(arrays[i], 0, result, index, arrays[i].Length);
				index += arrays[i].Length;
			}

			return result;
		}
		public static int[] CombineIntArrays(params int[][] arrays)
		{
			var len = 0;
			for (var i = 0; i < arrays.Length; i++)
			{
				len += arrays[i].Length;
			}

			var result = new int[len];
			var index = 0;
			for (var i = 0; i < arrays.Length; i++)
			{
				Array.Copy(arrays[i], 0, result, index, arrays[i].Length);
				index += arrays[i].Length;
			}

			return result;
		}

		public static void MultiplyFloatArrayBy(float[] result, float b)
		{
			for (var i = 0; i < result.Length; i++)
			{
				result[i] *= b;
			}
		}
		public static void SubtractFloatArrayFrom(float[] result, float[] b)
		{
			for (var i = 0; i < result.Length; i++)
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
			for (var i = 0; i < result.Length; i++)
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

        public static float[] GetSizedFloatArray(int size, float value)
        {
            var result = new float[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = value;
            }

            return result;
        }
        public static int[] GetSizedIntArray(int size, int value)
        {
            var result = new int[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = value;
            }

            return result;
        }

        public static float[] GetFloatZeroArray(int size)
        {
            return new float[size];
        }
        public static float[] GetFloatMinArray(int size)
        {
            return GetSizedFloatArray(size, float.MinValue);
            //return Enumerable.Repeat<float>(float.MinValue, size).ToArray();
        }
        public static float[] GetFloatMaxArray(int size)
        {
            return GetSizedFloatArray(size, float.MaxValue);
            //return Enumerable.Repeat<float>(float.MaxValue, size).ToArray();
        }
        public static int[] GetIntZeroArray(int size)
        {
            return new int[size];
        }
        public static int[] GetIntMinArray(int size)
        {
            return GetSizedIntArray(size, int.MinValue);
            //return Enumerable.Repeat<int>(int.MinValue, size).ToArray();
        }
        public static int[] GetIntMaxArray(int size)
        {
            return GetSizedIntArray(size, int.MaxValue);
            //return Enumerable.Repeat<int>(int.MaxValue, size).ToArray();
        }
    }

    public static class ArrayExtension
	{
		public static float[] ToFloat(this int[] values)
		{
			return Array.ConvertAll(values, x => (float) x);
        }
        public static Series ToSeries(this int[] values)
        {
            return new IntSeries(1, values);
        }
        public static Store ToStore(this int[] values)
        {
            return new Store(values.ToSeries());
        }

        public static int[] ToInt(this float[] values)
		{
			return Array.ConvertAll(values, x => (int) x);
        }

        public static Series ToSeries(this float[] values)
        {
            return new FloatSeries(1, values);
        }
        public static Store ToStore(this float[] values)
        {
            return new Store(values.ToSeries());
        }
    }
}