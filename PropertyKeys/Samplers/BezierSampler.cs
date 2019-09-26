using DataArcs.SeriesData;
using System;

namespace DataArcs.Samplers
{
    // Samples passed series based on the 'easing' encoded in the internal normalized bezier curve.
	public class BezierSampler : Sampler
	{
		private BezierSeries BezierSeries;

		public BezierSampler(BezierSeries bezierSeries)
		{
            bezierSeries.Normalize();
			BezierSeries = bezierSeries;
		}

		public override Series GetValueAtIndex(Series series, int index)
        {
            float t = Capacity != 1 ? index / (Capacity - 1f) : 0;
            return GetValueAtT(series, t);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            t = Math.Max(0, Math.Min(1f, t));
            return GetSeriesSample(series, t);
        }
      
        private Series GetSeriesSample(Series series, float t)
        {
            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
            var strideTs = GetSeriesAtT(t).FloatData;

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (i < strideTs.Length) ? series.GetValueAtT(strideTs[i]).FloatDataAt(i) : 0;
            }

            return SeriesUtils.Create(series, result);
        }

        private Series GetSeriesAtT(float t)
        {
            SeriesUtils.GetScaledT(t, Capacity, out var vT, out var startIndex, out var endIndex);
            var aSeries = BezierSeries.GetSeriesAtIndex(startIndex);
            var a = aSeries.GetValueAtVirtualIndex(startIndex == endIndex ? 0 : aSeries.Count - 1, aSeries.Count).FloatData;
            var b = BezierSeries.GetSeriesAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
            var moveType = startIndex < BezierSeries.Moves.Length ? BezierSeries.Moves[startIndex] : BezierMove.LineTo;

            var p2Index = b.Length - 2;
            float[] result = { 0, 0 };
            var it = 1f - vT;
            switch (moveType)
            {
                case BezierMove.MoveTo:
                case BezierMove.LineTo:
                    result[0] = a[0] + (b[p2Index] - a[0]) * vT;
                    result[1] = a[1] + (b[p2Index + 1] - a[1]) * vT;
                    break;
                case BezierMove.QuadTo:
                    result[0] = it * it * a[0] + 2 * it * vT * b[0] + vT * vT * b[p2Index];
                    result[1] = it * it * a[1] + 2 * it * vT * b[1] + vT * vT * b[p2Index + 1];
                    break;
                case BezierMove.CubeTo:
                    // todo: cubic bezier calc
                    break;
                case BezierMove.End:
                default:
                    result = b;
                    break;
            }

            return new FloatSeries(2, result);
        }
    }
}