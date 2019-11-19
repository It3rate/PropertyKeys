using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;

namespace DataArcs.SeriesData.Utils
{
	public delegate float FloatEquation(float a, float b);

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
                    floats[i] = SlotUtils.ComputeOnElement(value, swizzleMap[i]);
		        }
		        result = CreateSeriesOfType(series, floats, floats.Length);
            }
	        else
	        {
		        result = series;
	        }

	        return result;
	    }
		public static Series CreateSeriesOfType(Series series, int[] values, int vectorSize = -1)
		{
			Series result;
			vectorSize = (vectorSize == -1) ? series.VectorSize : vectorSize;
            if (series.Type == SeriesType.Int)
			{
				result = new IntSeries(vectorSize, values);
			}
			else if (series.Type == SeriesType.Parametric)
			{
				result = new ParametricSeries(vectorSize, values.ToFloat());
			}
            else
			{
				result = new FloatSeries(vectorSize, values.ToFloat());
			}

			return result;
		}

		public static Series CreateSeriesOfType(Series series, float[] values, int vectorSize = -1)
		{
			Series result;
			vectorSize = (vectorSize == -1) ? series.VectorSize : vectorSize;

			switch (series.Type)
			{
				case SeriesType.Int:
					result = new IntSeries(vectorSize, values.ToInt());
					break;
				// don't use RectF here as a sample of a rect should mean a float array. Probably RectF should not be so special to have it's own series type.
				//case SeriesType.RectF:
				//	result = new RectFSeries(values);
				//	break;
				case SeriesType.Parametric:
					result = new ParametricSeries(vectorSize, values);
                    break;
				case SeriesType.Float:
				default:
					result = new FloatSeries(vectorSize, values);
                    break;
			}
			return result;
		}

		public static Series CreateSeriesOfType(SeriesType seriesType, int vectorSize, int elementCount, float defaultValue = 0)
		{
			Series result;
			switch (seriesType)
			{
				case SeriesType.Int:
					int[] intValues = ArrayExtension.GetSizedIntArray(elementCount, (int)defaultValue);
					result = new IntSeries(vectorSize, intValues);
					break;
				//case SeriesType.RectF:
				//	result = new RectFSeries(defaultValue, defaultValue, defaultValue + 1f, defaultValue + 1f);
				//	break;
				case SeriesType.Parametric:
					float[] paramValues = ArrayExtension.GetSizedFloatArray(elementCount, defaultValue);
					result = new ParametricSeries(vectorSize, paramValues);
					break;
				case SeriesType.Float:
                default:
					float[] floatValues = ArrayExtension.GetSizedFloatArray(elementCount, defaultValue);
					result = new FloatSeries(vectorSize, floatValues);
					break;
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

        public static Series MergeSeriesElements(params Series[] seriesArray)
        {
			Debug.Assert(seriesArray.Length > 0);
	        var totalElements = 0;
	        var firstSeries = seriesArray[0];
	        foreach (var series in seriesArray)
	        {
		        totalElements += series.VectorSize;
	        }
			var values = new float[totalElements * firstSeries.Count];
			int len = 0;
			for (int seriesIndex = 0; seriesIndex < seriesArray.Length; seriesIndex++)
			{
				Series series = seriesArray[seriesIndex];
                for (int index = 0; index < series.Count; index++)
                {
	                var svals = series.GetRawDataAt(index).FloatDataRef;
	                Array.Copy(svals, 0, values, index * totalElements + len, series.VectorSize);
				}
                len += series.VectorSize;
			}

	        var result = SeriesUtils.CreateSeriesOfType(firstSeries, values, totalElements);
	        return result;
        }


        private static Series SlotsFunction(FloatEquation equation, float defaultValue, Series source, params Slot[] slots)
        {
	        bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
	        var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetSizedFloatArray(slotSize, defaultValue);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (useAllSlots)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[j] = equation(result[j], svals[j]);
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        if (slots[j] < Slot.Combinatorial)
				        {
					        result[j] = equation(result[j], svals[(int)slots[j]]);
                        }
				        else
				        {
					        result[j] = equation(result[j], SlotUtils.ComputeOnElement(source, slots[j], i));
				        }
			        }
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series SumSlots(Series source, params Slot[] slots)
        {
            return SlotsFunction((a, b) => a + b, 0, source, slots);
        }
        public static Series MultiplySlots(Series source, params Slot[] slots)
        {
            return SlotsFunction((a, b) => a * b, 1, source, slots);
        }
        public static Series ScaleSlots(Series source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => a * b, 0, source, slots);
        }
        public static Series MaxSlots(Series source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => b > a ? b : a, float.MinValue, source, slots);
        }
        public static Series MinSlots(Series source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => b < a ? b : a, float.MaxValue, source, slots);
        }
        public static Series ClampTo01Slots(Series source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => Math.Max(0, Math.Min(1, b)), 0, source, slots);
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
            bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
            var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var max = ArrayExtension.GetFloatMinArray(slotSize);
            var min = ArrayExtension.GetFloatMaxArray(slotSize);
            for (int i = 0; i < source.Count; i++)
            {
                var svals = source.GetRawDataAt(i).FloatDataRef;
                if (useAllSlots)
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
		                if (slots[j] < Slot.Combinatorial)
		                {
			                var index = (int)slots[j];
			                max[j] = svals[index] > max[j] ? svals[index] : max[j];
			                min[j] = svals[index] < min[j] ? svals[index] : min[j];
                        }
		                else
		                {
			                var calc = SlotUtils.ComputeOnElement(source, slots[j], i);
			                max[j] = calc > max[j] ? calc : max[j];
			                min[j] = calc < min[j] ? calc : min[j];
		                }
	                }

                }
            }

            for (int i = 0; i < max.Length; i++)
            {
                max[i] -= min[1];
            }
            return CreateSeriesOfType(source, max);
        }

        // Note: For simple cases it may be easier to use: float SlotUtils.ComputeOnElement(series, Slot.Sum, index)
        private static Series PerElementFunction(FloatEquation equation, float defaultValue, Series source, params Slot[] slots)
        {
	        bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
	        var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetSizedFloatArray(source.Count, defaultValue);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetRawDataAt(i).FloatDataRef;
		        if (useAllSlots)
		        {
			        for (int j = 0; j < svals.Length; j++)
			        {
				        result[i] = equation(result[i], svals[j]);
			        }
		        }
		        else
		        {
			        for (int j = 0; j < slots.Length; j++)
			        {
				        if (slots[j] < Slot.Combinatorial)
				        {
					        result[i] = equation(result[i], svals[(int) slots[j]]);
				        }
				        else
				        {
					        result[i] = equation(result[i], SlotUtils.ComputeOnElement(source, slots[j], i));
				        }
			        }
		        }
	        }
	        return CreateSeriesOfType(source, result, 1);
        }
        public static Series SumPerElement(Series source, params Slot[] slots)
        {
            return PerElementFunction((a, b) => a + b, 0, source, slots);
        }
        public static Series MultiplyPerElement(Series source, params Slot[] slots)
        {
            return PerElementFunction((a, b) => a * b, 1, source, slots);
        }
        public static Series ScalePerElement(Series source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => a * b, 0, source, slots);
        }
        public static Series MaxPerElement(Series source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => b > a ? b : a, float.MinValue, source, slots);
        }
        public static Series MinPerElement(Series source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => b < a ? b : a, float.MaxValue, source, slots);
        }


        public static void ShuffleElements(Series series)
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

    }
}