using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class RingSampler : Sampler
	{
        protected int[] RingCounts { get; }
        private int Capacity { get; }

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
        public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? Capacity : virtualCount;
			var indexT = index / (virtualCount - 1f); // full circle
			return GetSeriesSample(series, indexT, virtualCount);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
        {
            virtualCount = virtualCount == -1 ? Capacity : virtualCount;
            return GetSeriesSample(series, t, virtualCount);
		}
		

		public Series GetSeriesSample(Series series, float t, int virtualCount = -1)
		{
            int ring = 0;
            float ratio = RingCounts[0] / (float)Capacity;
            while(t - ratio > 0)
            {
                t -= ratio;
                ring++;
                ratio = RingCounts[ring] / (float)Capacity;
            }
            var ringT = t / ratio;
			var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var frame = series.Frame.FloatData; // x0,y0...n0, x1,y1..n1
			var size = series.Size.FloatData; // s0,s1...sn

            var centerX = size[0] / 2.0f;
			var radiusX = centerX - ring * (size[0] / 8.0f);
			result[0] = (float) (Math.Sin(ringT * 2.0f * Math.PI + Math.PI) * radiusX + frame[0] + centerX);
            var centerY = size[1] / 2.0f;
            var radiusY = centerY - ring * (size[1] / 8.0f);
			result[1] = (float) (Math.Cos(ringT * 2.0f * Math.PI + Math.PI) * radiusY + frame[1] + centerY);
			return SeriesUtils.Create(series, result);
		}
	}
}