﻿using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class RingSampler : Sampler
    {
        public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
        {
            virtualCount = (virtualCount == -1) ? series.VirtualCount : virtualCount;
            float indexT = index / (virtualCount - 1f); // full circle
            return GetSeriesSample(series, indexT, virtualCount);
        }

        public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
        {
            virtualCount = (virtualCount == -1) ? series.VirtualCount : virtualCount;
            if (virtualCount > -1)
            {
                t *= series.VirtualCount / (float)virtualCount;
            }
            return GetSeriesSample(series, t, virtualCount);
        }

        public override float GetTAtT(float t)
        {
            return (float) (Math.Sin(t * 2.0f * Utils.M_PIx2) / Math.PI);
        }


        public static Series GetSeriesSample(Series series, float t, int virtualCount = -1)
        {
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            float[] frame = series.Frame.FloatData; // x0,y0...n0, x1,y1..n1
            float[] size = series.Size.FloatData; // s0,s1...sn

            float radiusX = size[0] / 2.0f;
            result[0] = (float)(Math.Sin(t * 2.0f * Math.PI + Math.PI) * radiusX + frame[0] + radiusX);
            float radiusY = size[1] / 2.0f;
            result[1] = (float)(Math.Cos(t * 2.0f * Math.PI + Math.PI) * radiusY + frame[1] + radiusY);
            return SeriesUtils.Create(series, result);
        }
        
    }
}
