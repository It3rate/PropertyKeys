using DataArcs.Series;

namespace DataArcs.Samplers
{
    public class BezierSampler : Sampler
    {
        public BezierSeries Series;

        public BezierSampler()
        {
        }

        public BezierSampler(BezierSeries series)
        {
            Series = series;
        }

        public override Series.Series GetValueAtIndex(Series.Series series, int index, int virtualCount = -1)
        {
            // todo: check if this virtualCount assignment should happen in beziers at this point or pass through.
            virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
            series = series ?? Series;
            var t = index / (float) virtualCount;
            return GetValueAtT(series, t, virtualCount);
        }

        public override Series.Series GetValueAtT(Series.Series series, float t, int virtualCount = -1)
        {
            series = series ?? Series;
            var moves = series is BezierSeries bezierSeries ? bezierSeries.Moves : new[] {BezierMove.LineTo};
            return GetValueAtT(series, moves, t, virtualCount);
        }

        public override float GetTAtT(float t)
        {
            return Series != null ? GetValueAtT(Series, Series.Moves, t)[0] : t;
        }

        public static Series.Series GetValueAtT(Series.Series series, BezierMove[] moves, float t,
            int virtualCount = -1)
        {
            if (virtualCount > -1)
            {
                t *= series.VirtualCount / (float) virtualCount;
            }

            virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
            SeriesUtils.GetScaledT(t, virtualCount, out var vT, out var startIndex, out var endIndex);
            var a = series.GetSeriesAtIndex(startIndex).FloatData; // GetFloatArrayAtIndex(startIndex);
            var b = series.GetSeriesAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
            var p0Index = startIndex == endIndex ? 0 : a.Length - 2; // start from last point unless at start or end.
            var p2Index = b.Length - 2;
            var moveType = startIndex < moves.Length ? moves[startIndex] : BezierMove.LineTo;
            float[] result = {0, 0};
            var it = 1f - t;
            switch (moveType)
            {
                case BezierMove.MoveTo:
                case BezierMove.LineTo:
                    result[0] = a[p0Index] + (b[p2Index] - a[p0Index]) * t;
                    result[1] = a[p0Index + 1] + (b[p2Index + 1] - a[p0Index + 1]) * t;
                    break;
                case BezierMove.QuadTo:
                    result[0] = it * it * a[p0Index] + 2 * it * t * b[0] + t * t * b[p2Index];
                    result[1] = it * it * a[p0Index + 1] + 2 * it * t * b[1] + t * t * b[p2Index + 1];
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