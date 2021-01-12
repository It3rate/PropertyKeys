using System;
using MotiveCore.SeriesData;
using MotiveCore.SeriesData.Utils;

namespace MotiveCore.Samplers
{
    // Samples passed series based on the 'easing' encoded in the internal normalized bezier curve.
	public class BezierSampler : Sampler
	{
		private BezierSeries BezierSeries;

		public BezierSampler(BezierSeries bezierSeries, Slot[] swizzleMap = null, int sampleCount = 1) : base(swizzleMap, sampleCount)
        {
            //bezierSeries.Normalize();
			BezierSeries = bezierSeries;
			bezierSeries.EvenlySpaced = true;
		}
		
        public override Series GetValuesAtT(Series series, float t)
        {
            var ct = Math.Max(0, Math.Min(1f, t));
            return GetSeriesSample(series, ct);
        }

        private Series GetSeriesSample(Series series, float t)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            var strideTs = GetSeriesAtT(t).FloatDataRef;
            return SeriesUtils.CreateSeriesOfType(series, strideTs);
        }

        private Series GetSeriesAtT(float t)
        {
	        return BezierSeries.GetSeriesAtT(t);
        }
    }
}