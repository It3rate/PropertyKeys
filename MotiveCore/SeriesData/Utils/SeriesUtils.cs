using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;

namespace Motive.SeriesData.Utils
{
	public delegate float FloatEquation(float a);
	public delegate float BinaryFloatEquation(float a, float b);

    public class SeriesUtils
    {
	    public static ISeries SwizzleSeries(Slot[] swizzleMap, ISeries series)
	    {
		    ISeries result;
	        if (swizzleMap != null)
	        {
		        float[] floats = new float[swizzleMap.Length];
		        ISeries value = series.GetSeriesAt(0);
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
		public static ISeries CreateSeriesOfType(ISeries series, int[] values, int vectorSize = -1)
		{
			ISeries result;
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

		public static ISeries CreateSeriesOfType(ISeries series, float[] values, int vectorSize = -1)
		{
			ISeries result;
			vectorSize = (vectorSize == -1) ? series.VectorSize : vectorSize;

			switch (series.Type)
			{
				case SeriesType.Int:
					result = new IntSeries(vectorSize, values.ToInt()) { IndexClampMode = series.IndexClampMode };
					break;
				// don't use RectF here as a sample of a rect should mean a float array. Probably RectF should not be so special to have it's own series type.
				//case SeriesType.RectF:
				//	result = new RectFSeries(values);
				//	break;
				case SeriesType.Parametric:
					result = new ParametricSeries(vectorSize, values) { IndexClampMode = series.IndexClampMode };
                    break;
				case SeriesType.Float:
				default:
					result = new FloatSeries(vectorSize, values) { IndexClampMode = series.IndexClampMode };
                    break;
			}
			return result;
		}

		public static ISeries CreateSeriesOfType(SeriesType seriesType, int vectorSize, int elementCount, float defaultValue = 0)
		{
			ISeries result;
			int arraySize = elementCount * vectorSize;
			switch (seriesType)
			{
				case SeriesType.Int:
					int[] intValues = ArrayExtension.GetSizedIntArray(arraySize, (int)defaultValue);
					result = new IntSeries(vectorSize, intValues);
					break;
				//case SeriesType.RectF:
				//	result = new RectFSeries(defaultValue, defaultValue, defaultValue + 1f, defaultValue + 1f);
				//	break;
				case SeriesType.Parametric:
					float[] paramValues = ArrayExtension.GetSizedFloatArray(arraySize, defaultValue);
					result = new ParametricSeries(vectorSize, paramValues);
					break;
				case SeriesType.Float:
				default:
					float[] floatValues = ArrayExtension.GetSizedFloatArray(arraySize, defaultValue);
					result = new FloatSeries(vectorSize, floatValues);
					break;
			}
			return result;
		}

        public static bool IsEqual(ISeries a, ISeries b)
		{
			var result = true;
			if (a.GetType() == b.GetType() && a.Count == b.Count && a.VectorSize == b.VectorSize && a.IndexClampMode == b.IndexClampMode)
			{
                if (a.Type == SeriesType.Int)
                {
                    for (var i = 0; i < a.DataSize; i++)
                    {
                        var ar = a.GetSeriesAt(i).IntDataRef;
                        var br = b.GetSeriesAt(i).IntDataRef;
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
                        var ar = a.GetSeriesAt(i).FloatDataRef;
                        var br = b.GetSeriesAt(i).FloatDataRef;
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

        public static ISeries InterpolateInto(ISeries source, ISeries target, float t)
        {
	        source.InterpolateInto(target, t);
	        return source;
        }
        public static ISeries InterpolateInto(ISeries source, ISeries target, ParametricSeries seriesT)
        {
	        source.InterpolateInto(target, seriesT);
	        return source;
        }
        public static ISeries SetSeriesAtIndex(ISeries destination, int index, ISeries value)
        {
	        destination.SetSeriesAt(index, value);
	        return destination;
        }

        public static ISeries MergeSeriesElements(params ISeries[] seriesArray)
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
				ISeries series = seriesArray[seriesIndex];
                for (int index = 0; index < series.Count; index++)
                {
	                var svals = series.GetSeriesAt(index).FloatDataRef;
	                Array.Copy(svals, 0, values, index * totalElements + len, series.VectorSize);
				}
                len += series.VectorSize;
			}

	        var result = SeriesUtils.CreateSeriesOfType(firstSeries, values, totalElements);
	        return result;
        }


        private static ISeries SlotsFunction(BinaryFloatEquation equation, float defaultValue, ISeries source, params Slot[] slots)
        {
	        bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
	        var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetSizedFloatArray(slotSize, defaultValue);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAt(i).FloatDataRef;
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
        public static ISeries SumSlots(ISeries source, params Slot[] slots)
        {
            return SlotsFunction((a, b) => a + b, 0, source, slots);
        }
        public static ISeries MultiplySlots(ISeries source, params Slot[] slots)
        {
            return SlotsFunction((a, b) => a * b, 1, source, slots);
        }
        public static ISeries ScaleSlots(ISeries source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => a * b, 0, source, slots);
        }
        public static ISeries MaxSlots(ISeries source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => b > a ? b : a, float.MinValue, source, slots);
        }
        public static ISeries MinSlots(ISeries source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => b < a ? b : a, float.MaxValue, source, slots);
        }
        public static ISeries ClampTo01Slots(ISeries source, params Slot[] slots)
        {
	        return SlotsFunction((a, b) => Math.Max(0, Math.Min(1, b)), 0, source, slots);
        }
        public static ISeries AverageSlots(ISeries source, params Slot[] slots)
        {
            var result = SumSlots(source, slots).FloatDataRef; // now a copy due to SumSlots
	        float len = source.Count;
	        for (int j = 0; j < result.Length; j++)
	        {
		        result[j] /= len;
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static ISeries MaxDiffSlots(ISeries source, params Slot[] slots)
        {
            bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
            var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var max = ArrayExtension.GetFloatMinArray(slotSize);
            var min = ArrayExtension.GetFloatMaxArray(slotSize);
            for (int i = 0; i < source.Count; i++)
            {
                var svals = source.GetSeriesAt(i).FloatDataRef;
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

        // Note: For simple cases it may be easier to use: float SlotUtils.ComputeOnElement(series, Slot.Widest, index)
        private static ISeries PerElementFunction(BinaryFloatEquation equation, float defaultValue, ISeries source, params Slot[] slots)
        {
	        bool useAllSlots = slots.Length == 0 || slots[0] == Slot.All;
	        var slotSize = useAllSlots ? source.VectorSize : slots.Length;
	        var result = ArrayExtension.GetSizedFloatArray(source.Count, defaultValue);
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAt(i).FloatDataRef;
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
        public static ISeries SumPerElement(ISeries source, params Slot[] slots)
        {
            return PerElementFunction((a, b) => a + b, 0, source, slots);
        }
        public static ISeries MultiplyPerElement(ISeries source, params Slot[] slots)
        {
            return PerElementFunction((a, b) => a * b, 1, source, slots);
        }
        public static ISeries ScalePerElement(ISeries source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => a * b, 0, source, slots);
        }
        public static ISeries MaxPerElement(ISeries source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => b > a ? b : a, float.MinValue, source, slots);
        }
        public static ISeries MinPerElement(ISeries source, params Slot[] slots)
        {
	        return PerElementFunction((a, b) => b < a ? b : a, float.MaxValue, source, slots);
        }


        public static void ShuffleElements(ISeries series)
        {
	        var len = series.Count;
	        for (var i = 0; i < len; i++)
	        {
		        var a = Random.Next(len);
		        var b = Random.Next(len);
		        var sa = series.GetSeriesAt(a);
		        series.SetSeriesAt(a, series.GetSeriesAt(b));
		        series.SetSeriesAt(b, sa);
	        }
        }

    }
}