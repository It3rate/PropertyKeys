using System;
using DataArcs.Series;

namespace DataArcs.Samplers
{
    public class RingSampler : Sampler
    {
        public override Series.Series GetValueAtIndex(Series.Series series, int index, int virtualCount = -1)
        {
            virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
            var indexT = index / (virtualCount - 1f); // full circle
            return GetSeriesSample(series, indexT, virtualCount);
        }

        public override Series.Series GetValueAtT(Series.Series series, float t, int virtualCount = -1)
        {
            virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
            if (virtualCount > -1)
            {
                t *= series.VirtualCount / (float) virtualCount;
            }

            return GetSeriesSample(series, t, virtualCount);
        }

        public override float GetTAtT(float t)
        {
            return (float) (Math.Sin(t * 2.0f * Utils.M_PIx2) / Math.PI);
        }


        public static Series.Series GetSeriesSample(Series.Series series, float t, int virtualCount = -1)
        {
            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
            var frame = series.Frame.FloatData; // x0,y0...n0, x1,y1..n1
            var size = series.Size.FloatData; // s0,s1...sn

            var radiusX = size[0] / 2.0f;
            result[0] = (float) (Math.Sin(t * 2.0f * Math.PI + Math.PI) * radiusX + frame[0] + radiusX);
            var radiusY = size[1] / 2.0f;
            result[1] = (float) (Math.Cos(t * 2.0f * Math.PI + Math.PI) * radiusY + frame[1] + radiusY);
            return SeriesUtils.Create(series, result);
        }
    }
}