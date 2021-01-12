using System;
using MotiveCore.Stores;

namespace MotiveCore.SeriesData.Utils
{
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
			return GetSizedFloatArray(size, Single.MinValue);
			//return Enumerable.Repeat<float>(float.MinValue, size).ToArray();
		}

		public static float[] GetFloatMaxArray(int size)
		{
			return GetSizedFloatArray(size, Single.MaxValue);
			//return Enumerable.Repeat<float>(float.MaxValue, size).ToArray();
		}

		public static int[] GetIntZeroArray(int size)
		{
			return new int[size];
		}

		public static int[] GetIntMinArray(int size)
		{
			return GetSizedIntArray(size, Int32.MinValue);
			//return Enumerable.Repeat<int>(int.MinValue, size).ToArray();
		}

		public static int[] GetIntMaxArray(int size)
		{
			return GetSizedIntArray(size, Int32.MaxValue);
			//return Enumerable.Repeat<int>(int.MaxValue, size).ToArray();
		}

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
	}
}