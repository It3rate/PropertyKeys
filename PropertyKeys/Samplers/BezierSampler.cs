using DataArcs.SeriesData;
using System;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
    // Samples passed series based on the 'easing' encoded in the internal normalized bezier curve.
	public class BezierSampler : Sampler
	{
		private BezierSeries BezierSeries;

		public BezierSampler(BezierSeries bezierSeries, Slot[] swizzleMap = null, int sampleCount = 1) : base(swizzleMap, sampleCount)
        {
            //bezierSeries.Normalize();
			BezierSeries = bezierSeries;
		}

		public override Series GetValueAtIndex(Series series, int index)
		{
			float t = SampleCount != 1 ? SamplerUtils.TFromIndex(SampleCount, index) : 0; // index / (SampleCount - 1f) : 0;
            return GetValuesAtT(series, t);
        }

		//public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
		//{
		//	var t = Math.Max(0, Math.Min(1f, seriesT[0]));
  //          return base.GetSampledTs(seriesT);
		//}

        public override Series GetValuesAtT(Series series, float t)
        {
            t = Math.Max(0, Math.Min(1f, t));
            return GetSeriesSample(series, t);
        }

        private Series GetSeriesSample(Series series, float t)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            var strideTs = GetSeriesAtT(t).FloatDataRef;

            //for (var i = 0; i < result.Length; i++)
            //{
            //    result[i] = (i < strideTs.Length) ? series.GetVirtualValueAt(strideTs[i]).FloatDataAt(i) : 0;
            //}

            return SeriesUtils.CreateSeriesOfType(series, strideTs);
        }

        private Series GetSeriesAtT(float t)
        {
	        BezierSeries.GetSegmentFromT(t, out var vT, out var startIndex, out var endIndex);
            //SeriesUtils.GetScaledT(t, SampleCount, out var vT, out var startIndex, out var endIndex);
            var aSeries = BezierSeries.GetRawDataAt(startIndex);
            //var a = aSeries.GetVirtualValueAt(startIndex == endIndex ? 0 : aSeries.Count - 1, aSeries.Count).FloatDataRef;
            var a = new[] { aSeries.FloatDataRef[aSeries.DataSize - 2], aSeries.FloatDataRef[aSeries.DataSize - 1]};
            var b = BezierSeries.GetRawDataAt(endIndex).FloatDataRef; // GetFloatArrayAtIndex(endIndex);
            var moveType = endIndex < BezierSeries.Moves.Length ? BezierSeries.Moves[endIndex] : BezierMove.End;

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
                case BezierMove.End: // special case when t == 1
	                result[0] = a[a.Length - 2];
	                result[1] = a[a.Length - 1];
	                break;
                default:
                    result = b;
                    break;
            }

            return new FloatSeries(2, result);
        }
    }
}