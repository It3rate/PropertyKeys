using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class BezierSampler : Sampler
	{
		private BezierSeries BezierSeries;

		public BezierSampler(BezierSeries bezierSeries)
		{
			BezierSeries = bezierSeries;
		}

		public override Series GetValueAtIndex(Series series, int index)
		{
			// not Bezier series by overloads
            float capacity = (series as BezierSeries)?.Moves.Length ?? series.Count;
			var t = index / capacity;
			return GetValueAtT(series, t);
		}

        public override Series GetValueAtT(Series series, float t)
        {
	        return GetSeriesAtT(t);
            // not Bezier series by overloads
         //   SeriesUtils.GetScaledT(t, series.Count, out var vT, out var startIndex, out var endIndex);
	        //var aSeries = series.GetDataAtIndex(startIndex);
	        //var a = aSeries.GetValueAtVirtualIndex(startIndex == endIndex ? 0 : aSeries.Count - 1, aSeries.Count).FloatData;
	        //var b = series.GetDataAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
	        //var moveType = b.Length > 2 ? BezierMove.CubeTo : BezierMove.LineTo;// startIndex < series.Moves.Length ? series.Moves[startIndex] : BezierMove.LineTo;
	        //return GetSeriesAtT(a, b, moveType, vT);
        }

		public Series GetValueAtIndex(BezierSeries series, int index)
		{
			var t = index / Capacity;
			return GetValueAtT(series, t);
		}
        public Series GetValueAtT(BezierSeries series, float t)
        {
	        return GetSeriesAtT(t);
      //      SeriesUtils.GetScaledT(t, Capacity, out var vT, out var startIndex, out var endIndex);
	     //   var aSeries = series.GetDataAtIndex(startIndex);
		    //var a = aSeries.GetValueAtVirtualIndex(startIndex == endIndex ? 0 : aSeries.Count - 1, aSeries.Count).FloatData;
	     //   var b = series.GetDataAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
	     //   var moveType = startIndex < series.Moves.Length ? series.Moves[startIndex] : BezierMove.LineTo;
	     //   return GetSeriesAtT(a, b, moveType, vT);
        }

        private Series GetSeriesAtT(float t)
        {
	        SeriesUtils.GetScaledT(t, Capacity, out var vT, out var startIndex, out var endIndex);
	        var aSeries = BezierSeries.GetDataAtIndex(startIndex);
	        var a = aSeries.GetValueAtVirtualIndex(startIndex == endIndex ? 0 : aSeries.Count - 1, aSeries.Count).FloatData;
	        var b = BezierSeries.GetDataAtIndex(endIndex).FloatData; // GetFloatArrayAtIndex(endIndex);
	        var moveType = startIndex < BezierSeries.Moves.Length ? BezierSeries.Moves[startIndex] : BezierMove.LineTo;

            var p2Index = b.Length - 2;
			float[] result = {0, 0};
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