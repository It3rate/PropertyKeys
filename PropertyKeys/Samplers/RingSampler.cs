using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class RingSampler : Sampler
	{
        protected int[] RingCounts { get; }

        public RingSampler(int[] ringCounts)
        {
            RingCounts = ringCounts;
            Capacity = ringCounts[0];
            for (int i = 1; i < ringCounts.Length; i++)
            {
                if (ringCounts[i] != 0)
                {
                    Capacity += ringCounts[i];
                }
                else
                {
                    break;
                }
            }
        }
        public override Series GetValueAtIndex(Series series, int index)
		{
			var indexT = index / (Capacity - 1f); // full circle
			return GetSeriesSample(series, indexT);
		}

		public override Series GetValueAtT(Series series, float t)
        {
            return GetSeriesSample(series, t);
		}

        public override ParametricSeries GetSampledT(float t)
        {
            SamplerUtils.GetJaggedT(RingCounts, t, out var ringIndexT, out var ringT);
            return new ParametricSeries(2, ringIndexT, ringT);
        }

        private Series GetSeriesSample(Series series, float t)
		{
			SamplerUtils.GetJaggedT(RingCounts, t, out var ringIndexT, out var ringT);

            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var frame = series.Frame.FloatData; // x0,y0...n0, x1,y1..n1
			var size = series.Size.FloatData; // s0,s1...sn

            var centerX = size[0] / 2.0f;
			var radiusX = centerX - ringIndexT * (size[0] / 2.0f);
			result[0] = (float) (Math.Sin(ringT * 2.0f * Math.PI + Math.PI) * radiusX + frame[0] + centerX);
            var centerY = size[1] / 2.0f;
            var radiusY = centerY - ringIndexT * (size[1] / 2.0f);
			result[1] = (float) (Math.Cos(ringT * 2.0f * Math.PI + Math.PI) * radiusY + frame[1] + centerY);
			return SeriesUtils.Create(series, result);
		}
	}
}