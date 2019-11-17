using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;

namespace DataArcs.SeriesData.Utils
{
	public class SeriesUtils
    {
	    public static Series SwizzleSeries(Slot[] swizzleMap, Series series)
	    {
		    Series result;
	        if (swizzleMap != null)
	        {
		        float[] floats = new float[swizzleMap.Length];
		        Series value = series.GetRawDataAt(0);
		        for (int i = 0; i < swizzleMap.Length; i++)
		        {
                    floats[i] = SlotUtils.GetFloatAt(value, swizzleMap[i]);
		        }
		        result = CreateSeriesOfType(series, floats);
            }
	        else
	        {
		        result = series;
	        }

	        return result;
	    }
		public static Series CreateSeriesOfType(Series series, int[] values)
		{
			Series result;
			if (series.Type == SeriesType.Int)
			{
				result = new IntSeries(series.VectorSize, values);
			}
			else if (series.Type == SeriesType.Parametric)
			{
				result = new ParametricSeries(series.VectorSize, values.ToFloat());
			}
            else
			{
				result = new FloatSeries(series.VectorSize, values.ToFloat());
			}

			return result;
		}

		public static Series CreateSeriesOfType(Series series, float[] values)
		{
			Series result;
			if (series.Type == SeriesType.Int)
			{
				result = new IntSeries(series.VectorSize, values.ToInt());
			}
			else if (series.Type == SeriesType.Parametric)
			{
				result = new ParametricSeries(series.VectorSize, values);
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
                if (a.Type == SeriesType.Int)
                {
                    for (var i = 0; i < a.DataSize; i++)
                    {
                        var ar = a.GetRawDataAt(i).IntDataRef;
                        var br = b.GetRawDataAt(i).IntDataRef;
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
                else
                {
                    for (var i = 0; i < a.DataSize; i++)
                    {
                        var delta = 0.0001f;
                        var ar = a.GetRawDataAt(i).FloatDataRef;
                        var br = b.GetRawDataAt(i).FloatDataRef;
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
			}
			else
			{
				result = false;
			}

			return result;
		}

		public static FloatSeries GetZeroFloatSeries(int vectorSize, int elementCount)
		{
			return new FloatSeries(vectorSize, ArrayExtension.GetFloatZeroArray(vectorSize * elementCount));
		}
		public static ParametricSeries GetZeroParametricSeries(int vectorSize, int elementCount)
		{
			return new ParametricSeries(vectorSize, ArrayExtension.GetFloatZeroArray(vectorSize * elementCount));
		}

        public static IntSeries GetZeroIntSeries(int vectorSize, int elementCount)
		{
			return new IntSeries(vectorSize, ArrayExtension.GetIntZeroArray(vectorSize * elementCount));
		}

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


        public static Series InterpolateInto(Series source, Series target, float t)
        {
	        source.InterpolateInto(target, t);
	        return source;
        }
        public static Series InterpolateInto(Series source, Series target, ParametricSeries seriesT)
        {
	        source.InterpolateInto(target, seriesT);
	        return source;
        }
        public static Series SetSeriesAtIndex(Series destination, int index, Series value)
        {
	        destination.SetRawDataAt(index, value);
	        return destination;
        }
        public static void Shuffle(Series series)
		{
			var len = series.Count;
            for (var i = 0; i < len; i++)
			{
				var a = Random.Next(len);
				var b = Random.Next(len);
					var sa = series.GetRawDataAt(a);
					series.SetRawDataAt(a, series.GetRawDataAt(b));
					series.SetRawDataAt(b, sa);
			}
        }

        public static Series SumSlots(Series source, params Slot[] slots)
        {
			var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
	        var result = new float[slotSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (slots.Length == 0)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] += svals[j];
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        result[j] += svals[(int)slots[j]];
			        }

                }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series ScaleSlots(Series source, params Slot[] slots)
        {
	        var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
            var result = new float[slotSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (slots.Length == 0)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] *= svals[j];
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        result[j] *= svals[(int)slots[j]];
			        }
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series AverageSlots(Series source, params Slot[] slots)
        {
            var result = SumSlots(source, slots).FloatDataRef; // now a copy due to SumSlots
	        float len = source.Count;
	        for (int j = 0; j < result.Length; j++)
	        {
		        result[j] /= len;
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series MaxDiffSlots(Series source, params Slot[] slots)
        {
	        var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
	        var max = ArrayExtension.GetFloatMinArray(slotSize);
            var min = ArrayExtension.GetFloatMaxArray(slotSize);
            for (int i = 0; i < source.Count; i++)
            {
                var svals = source.GetRawDataAt(i).FloatDataRef;
                if (slots.Length == 0)
                {
	                for (int j = 0; j < svals.Length; j++)
	                {
		                max[j] = svals[j] > max[j] ? svals[j] : max[j];
		                min[j] = svals[j] < min[j] ? svals[j] : min[j];
                    }
                }
                else
                {
	                for (int j = 0; j < slots.Length; j++)
	                {
		                var index = (int) slots[j];
		                max[j] = svals[index] > max[j] ? svals[index] : max[j];
		                min[j] = svals[index] < min[j] ? svals[index] : min[j];
	                }

                }
            }

            for (int i = 0; i < max.Length; i++)
            {
                max[i] -= min[1];
            }
            return CreateSeriesOfType(source, max);
        }
        public static Series MaxSlots(Series source, params Slot[] slots)
        {
	        var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetFloatMinArray(slotSize);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (slots.Length == 0)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] = svals[j] > result[j] ? svals[j] : result[j];
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        var index = (int)slots[j];
				        result[j] = svals[index] > result[j] ? svals[index] : result[j];
			        }
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series MinSlots(Series source, params Slot[] slots)
        {
	        var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetFloatMaxArray(slotSize);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (slots.Length == 0)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] = svals[j] < result[j] ? svals[j] : result[j];
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        var index = (int)slots[j];
				        result[j] = svals[index] < result[j] ? svals[index] : result[j];
			        }
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }

        public static Series ClampTo01Slots(Series source, params Slot[] slots)
        {
	        var slotSize = slots.Length == 0 ? source.VectorSize : slots.Length;
	        var result = new float[slotSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (slots.Length == 0)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] = Math.Max(0, Math.Min(1, svals[j]));
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        var index = (int)slots[j];
				        result[j] = Math.Max(0, Math.Min(1, svals[(int)slots[j]]));
			        }
                }
	        }

	        return CreateSeriesOfType(source, result);
        }
    }
}