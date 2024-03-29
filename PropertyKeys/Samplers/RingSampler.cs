﻿using System;
using System.Diagnostics;
using System.Linq;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public class RingSampler : Sampler
	{
        protected int[] RingCounts { get; }
        protected IStore Orientation { get; }
        public float MinRadius { get; set; }

        public RingSampler(int[] ringCounts, IStore orientation = null, Slot[] swizzleMap = null) : base(swizzleMap)
        {
            RingCounts = ringCounts;
            Orientation = orientation;
            SliceCount = ringCounts[0];
            for (int i = 1; i < ringCounts.Length; i++)
            {
                SliceCount += ringCounts[i];
            }

            MinRadius = 0.3f;
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        ParametricSeries result;
	        if (seriesT.VectorSize == 1) // assume if there is more than one vectorSize the params are set
	        {
		        int max = RingCounts.Sum() - 1;
		        int index = (int)Math.Max(0, Math.Min(max, Math.Floor(seriesT.X * max + 0.5f)));
		        result = SamplerUtils.GetSummedJaggedT(RingCounts, index, true);
	        }
	        else
	        {
		        result = seriesT;
	        }
			
	        return Swizzle(result, seriesT);
        }

        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
			float ringIndexT = seriesT.X;
			float ringT = seriesT.Y;
			
            float orientation = 0;
            if (Orientation != null)
            {
                orientation = Orientation.GetValuesAtT(ringT).X;
                orientation -= (int)orientation;
                orientation *= (float)(Math.PI * 2);
            }

            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
			var frame = series.Frame.FloatDataRef; // x0,y0...n0, x1,y1..n1
			var size = series.Size.FloatDataRef; // s0,s1...sn

            var centerX = size[0] / 2.0f;
            var radiusX = centerX - ringIndexT * ((size[0] / 2.0f) * (1f - MinRadius));
			result[0] = (float) (Math.Sin(ringT * 2.0f * Math.PI + Math.PI + orientation) * radiusX + frame[0] + centerX);
            var centerY = size[1] / 2.0f;
            var radiusY = centerY - ringIndexT * ((size[1] / 2.0f) * (1f - MinRadius));
            result[1] = (float) (Math.Cos(ringT * 2.0f * Math.PI + Math.PI + orientation) * radiusY + frame[1] + centerY);

            return SeriesUtils.CreateSeriesOfType(series, result);
		}
	}
}