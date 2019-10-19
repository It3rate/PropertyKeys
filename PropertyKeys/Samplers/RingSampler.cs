using System;
using System.Diagnostics;
using System.Linq;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public class RingSampler : Sampler
	{
        protected int[] RingCounts { get; }
        protected IStore Orientation { get; }

        public RingSampler(int[] ringCounts, IStore orientation = null)
        {
            RingCounts = ringCounts;
            Orientation = orientation;
            Capacity = ringCounts[0];
            for (int i = 1; i < ringCounts.Length; i++)
            {
                Capacity += ringCounts[i];
            }
        }
        public override Series GetValueAtIndex(Series series, int index)
		{
			var indexT = index / (Capacity - 1f); // full circle
			return GetSeriesSample(series, indexT);
		}

		public override Series GetValuesAtT(Series series, float t)
        {
            return GetSeriesSample(series, t);
		}

        public override ParametricSeries GetSampledTs(float t)
        {
            int index = (int)Math.Floor(t * (RingCounts.Sum() - 1) + 0.5f);
            return SamplerUtils.GetSummedJaggedT(RingCounts, index, false);
        }

        public override IntSeries GetSampledIndexes(float t)
        {
	        var sample = GetSampledTs(t);
	        int ringIndex = (int)(sample.X * RingCounts.Length);
            return new IntSeries(2, ringIndex, (int)(sample.Y * RingCounts[ringIndex]));
        }

        private Series GetSeriesSample(Series series, float t)
        {
	        var sample = GetSampledTs(t);

			float ringIndexT = sample.X;
			float ringT = sample.Y;

            //Debug.WriteLine(ringIndexT + " : " + ringT + " :: " + RingCounts[0]);
            float orientation = 0;
            if (Orientation != null)
            {
                orientation = Orientation.GetValuesAtT(ringT).X;
                orientation -= (int)orientation;
                orientation *= (float)(Math.PI * 2);
            }

            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var frame = series.Frame.FloatData; // x0,y0...n0, x1,y1..n1
			var size = series.Size.FloatData; // s0,s1...sn
            float minRadius = 0.3f; // todo: make class property

            var centerX = size[0] / 2.0f;
            var radiusX = centerX - ringIndexT * ((size[0] / 2.0f) * (1f - minRadius));
			result[0] = (float) (Math.Sin(ringT * 2.0f * Math.PI + Math.PI + orientation) * radiusX + frame[0] + centerX);
            var centerY = size[1] / 2.0f;
            var radiusY = centerY - ringIndexT * ((size[1] / 2.0f) * (1f - minRadius));
            result[1] = (float) (Math.Cos(ringT * 2.0f * Math.PI + Math.PI + orientation) * radiusY + frame[1] + centerY);

            return SeriesUtils.Create(series, result);
		}
	}
}