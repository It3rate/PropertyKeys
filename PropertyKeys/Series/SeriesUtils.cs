using System;
using DataArcs.Stores;

namespace DataArcs.Series
{
    public class SeriesUtils
    {
        public static void Shuffle(Series series)
        {
            int len = series.DataSize;
            for (int i = 0; i < series.DataSize; i++)
            {
                int a = DataUtils.Random.Next(len);
                int b = DataUtils.Random.Next(len);
                Series sa = series.GetSeriesAtIndex(a);
                Series sb = series.GetSeriesAtIndex(b);
                series.SetSeriesAtIndex(a, sb);
                series.SetSeriesAtIndex(b, sa);
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
            bool result = true;
            if (a.GetType() == b.GetType() && a.VirtualCount == b.VirtualCount && a.VectorSize == b.VectorSize)
            {
                for (int i = 0; i < a.VirtualCount; i++)
                {
                    if (a.Type == SeriesType.Float)
                    {
                        float delta = 0.0001f;
                        float[] ar = a.GetValueAtVirtualIndex(i).FloatData;
                        float[] br = b.GetValueAtVirtualIndex(i).FloatData;
                        for (int j = 0; j < ar.Length; j++)
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
                        int[] ar = a.GetValueAtVirtualIndex(i).IntData;
                        int[] br = b.GetValueAtVirtualIndex(i).IntData;
                        for (int j = 0; j < ar.Length; j++)
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

        public static FloatSeries GetZeroFloatSeries(int vectorSize, int elementCount) { return new FloatSeries(vectorSize, DataUtils.GetFloatZeroArray(vectorSize * elementCount)); }
        public static IntSeries GetZeroIntSeries(int vectorSize, int elementCount) { return new IntSeries(vectorSize, DataUtils.GetIntZeroArray(vectorSize * elementCount)); }
    }
}
