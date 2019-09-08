using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class HexagonSampler : Sampler
    {
        protected int[] Strides { get; }

        public HexagonSampler(int[] strides)
        {
            Strides = strides;
        }

        public override Series GetValueAtIndex(Series series, int index)
        {
            float indexT = index / (series.VirtualCount - 1f); 
            return GetSeriesSample(series, Strides, index);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            return GetSeriesSample(series, Strides, (int)(t * (series.VirtualCount )));
        }

        public override float GetTAtT(float t)
        {
            float result;
            if (Strides[0] > 0)
            {
                result = Strides[0] * t;
                int row = (int)result;
                if ((row & 1) == 1)
                {
                    result += 1f / (Strides[0] - 1f) * 0.5f;
                }
                result -= row;
            }
            else
            {
                result = t;
            }
            return result;
        }

        public static Series GetSeriesSample(Series series, int[] strides, int index)
        {
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            //float[] frame = series.Frame.Floats; // x0,y0...n0, x1,y1..n1
            float[] size = series.Size.Floats; // s0,s1...sn

            float[] strideTs = GetStrideTsForIndex(series, strides, index);
            //int[] dims = GetDimsForIndex(series, strides, index);
            //result[0] = series.GetValueAtT(dims[0]/(float)strides[0]).Floats[0];
            //result[0] += ((dims[1]&1) == 1) ? size[0] / (strides[0] - 1) : 0;
            int maxRows = (int)(series.VirtualCount / (float)strides[0]);
            float rowT = 1f / maxRows;
            //result[1] = series.GetValueAtT(dims[1]*strides[0] / (float)maxRows).Floats[1];
            
            for (int i = 0; i < result.Length; i++)
            {
                float temp = series.GetValueAtT(strideTs[i]).Floats[i];
                int curRow = (int)((float)index / strides[0]);
                if (i == 0 && ((curRow & 1) == 1) && strides[0] > 0)
                {
                    result[i] = temp + (size[0] / (strides[0] - 1f) * 0.5f);
                }
                else
                {
                    result[i] = temp;
                }
            }
            
            return Series.Create(series, result);
        }

    }
}
