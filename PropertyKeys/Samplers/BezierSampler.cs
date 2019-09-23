using DataArcs.SeriesData;

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
			Capacity = series.Count;
		}

		public override Series GetValueAtIndex(Series series, int index)
		{
			// todo: check if this virtualCount assignment should happen in beziers at this point or pass through.
			series = series ?? Series;
			var t = index / (float) Capacity;
			return GetValueAtT(series, t);
		}

		public override Series GetValueAtT(Series series, float t)
		{
			series = series ?? Series;
			var moves = series is BezierSeries bezierSeries ? bezierSeries.Moves : new[] {BezierMove.LineTo};
			return GetSeriesAtT(series, t);
		}
		
		private Series GetSeriesAtT(Series series, float t)
		{
			SeriesUtils.GetScaledT(t, Series.Moves.Length, out var vT, out var startIndex, out var endIndex);
			var a = series.GetDataAtIndex(startIndex).FloatData; // GetFloatArrayAtIndex(startIndex);
			var b = series.GetDataAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
			var p0Index = startIndex == endIndex ? 0 : a.Length - 2; // start from last point unless at start or end.
			var p2Index = b.Length - 2;
			var moveType = startIndex < Series.Moves.Length ? Series.Moves[startIndex] : BezierMove.LineTo;
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