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
			bezierSeries.EvenlySpaced = true;
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
            var ct = Math.Max(0, Math.Min(1f, t));
            return GetSeriesSample(series, ct);
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
	        return BezierSeries.GetSeriesAtT(t);
        }
    }
}