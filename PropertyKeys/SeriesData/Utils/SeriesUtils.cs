using System;

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
		        Series value = series.GetSeriesAtIndex(0);
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
                        var ar = a.GetSeriesAtIndex(i).IntDataRef;
                        var br = b.GetSeriesAtIndex(i).IntDataRef;
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
                        var ar = a.GetSeriesAtIndex(i).FloatDataRef;
                        var br = b.GetSeriesAtIndex(i).FloatDataRef;
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
	        destination.SetSeriesAtIndex(index, value);
	        return destination;
        }
        public static void Shuffle(Series series)
		{
			var len = series.DataSize;
			for (var i = 0; i < series.DataSize; i++)
			{
				var a = Random.Next(len);
				var b = Random.Next(len);
				var sa = series.GetSeriesAtIndex(a);
				var sb = series.GetSeriesAtIndex(b);
				series.SetSeriesAtIndex(a, sb);
				series.SetSeriesAtIndex(b, sa);
			}
		}

        public static Series Sum(Series source)
        {
	        var result = new float[source.VectorSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAtIndex(i).FloatDataRef;
		        for (int j = 0; j < svals.Length; j++)
		        {
			        result[j] += svals[j];
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series Scale(Series source)
        {
	        var result = new float[source.VectorSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAtIndex(i).FloatDataRef;
		        for (int j = 0; j < svals.Length; j++)
		        {
			        result[j] *= svals[j];
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series Average(Series source)
        {
	        var result = Sum(source).FloatDataRef; // already a copy
	        float len = source.Count;
	        for (int j = 0; j < result.Length; j++)
	        {
		        result[j] /= len;
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series Max(Series source)
        {
	        var result = new float[source.VectorSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAtIndex(i).FloatDataRef;
		        for (int j = 0; j < svals.Length; j++)
		        {
			        result[j] = svals[j] > result[j] ? svals[j] : result[j];
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series Min(Series source)
        {
	        var result = new float[source.VectorSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAtIndex(i).FloatDataRef;
		        for (int j = 0; j < svals.Length; j++)
		        {
			        result[j] = svals[j] < result[j] ? svals[j] : result[j];
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
        public static Series ClampTo01(Series source)
        {
	        var result = new float[source.VectorSize];
	        for (int i = 0; i < source.Count; i++)
	        {
		        var svals = source.GetSeriesAtIndex(i).FloatDataRef;
		        for (int j = 0; j < svals.Length; j++)
		        {
			        result[j] = Math.Max(0, Math.Min(1, svals[j]));
		        }
	        }
	        return CreateSeriesOfType(source, result);
        }
    }
}