using DataArcs.Stores;
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
			if (a.GetType() == b.GetType() && a.VirtualCount == b.VirtualCount && a.VectorSize == b.VectorSize)
			{
				for (var i = 0; i < a.VirtualCount; i++)
				{
					if (a.Type == SeriesType.Float)
					{
						var delta = 0.0001f;
						var ar = a.GetValueAtVirtualIndex(i).FloatData;
						var br = b.GetValueAtVirtualIndex(i).FloatData;
						for (var j = 0; j < ar.Length; j++)
						{
							if (Math.Abs(ar[j] - br[j]) > delta)
							{
								result = false;
								break;
							}
						}
					}
					else
					{
						var ar = a.GetValueAtVirtualIndex(i).IntData;
						var br = b.GetValueAtVirtualIndex(i).IntData;
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

		private static readonly float[][] zeroFloatArray;
		private static readonly float[][] minFloatArray;
		private static readonly float[][] maxFloatArray;
		private static readonly int[][] zeroIntArray;
		private static readonly int[][] minIntArray;
		private static readonly int[][] maxIntArray;

		// Generate default array sizes for zero, min and max for quick cloning.
		static SeriesUtils()
		{
			var count = 16; // if higher than 16 needed make this lazy generating.
			zeroFloatArray = new float[count][];
			minFloatArray = new float[count][];
			maxFloatArray = new float[count][];
			zeroIntArray = new int[count][];
			minIntArray = new int[count][];
			maxIntArray = new int[count][];
			for (var i = 0; i < count; i++)
			{
				zeroFloatArray[i] = GetSizedFloatArray(i, 0);
				minFloatArray[i] = GetSizedFloatArray(i, float.MinValue);
				maxFloatArray[i] = GetSizedFloatArray(i, float.MaxValue);
				zeroIntArray[i] = GetSizedIntArray(i, 0);
				minIntArray[i] = GetSizedIntArray(i, int.MinValue);
				maxIntArray[i] = GetSizedIntArray(i, int.MaxValue);
			}
		}

		public static float[] GetFloatZeroArray(int size)
		{
			return (float[]) zeroFloatArray[size].Clone();
		}

		public static float[] GetFloatMinArray(int size)
		{
			return (float[]) minFloatArray[size].Clone();
		}

		public static float[] GetFloatMaxArray(int size)
		{
			return (float[]) maxFloatArray[size].Clone();
		}

		public static int[] GetIntZeroArray(int size)
		{
			return (int[]) zeroIntArray[size].Clone();
		}

		public static int[] GetIntMinArray(int size)
		{
			return (int[]) minIntArray[size].Clone();
		}

		public static int[] GetIntMaxArray(int size)
		{
			return (int[]) maxIntArray[size].Clone();
		}

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