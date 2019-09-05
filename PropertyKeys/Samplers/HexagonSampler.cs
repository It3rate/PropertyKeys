using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class HexagonSampler : BaseSampler
    {
        protected int[] Strides { get; }

        public HexagonSampler(int[] strides)
        {
            Strides = strides;
        }

        public override Series GetValueAtIndex(Series series, int index)
        {
            float indexT = index / (series.DataCount - 1f); // full circle
            return GetSeriesSample(series, Strides, index);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            return GetSeriesSample(series, Strides, (int)(t * series.VirtualCount));
        }

        public static Series GetSeriesSample(Series series, int[] strides, int index)
        {
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            float[] frame = series.Frame.FloatValuesCopy; // x0,y0...n0, x1,y1..n1
            float[] size = series.Size.FloatValuesCopy; // s0,s1...sn

            float[] strideTs = GetStrideTsForT(series, strides, index);

            for (int i = 0; i < result.Length; i++)
            {
                float temp = series.GetValueAtT(strideTs[i]).FloatValuesCopy[i];
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

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = series.GetValueAtT(strideTs[i]).FloatValuesCopy[i];
            }

            return Series.Create(series, result);
        }



        public override float[] GetFloatSample(Store valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForIndex(valueStore, index);
            for (int i = 0; i < result.Length; i++)
            {
                float temp = valueStore.GetInterpolatededValueAtT(strideTs[i])[i];
                int curRow = (int)((float)index / valueStore.Strides[0]);
                if(i == 0 && ((curRow & 1) == 1) && valueStore.Strides[0] > 0)
                {
                    result[i] = temp + (valueStore.Size[0] / (valueStore.Strides[0] - 1f) * 0.5f);
                }
                else
                {
                    result[i] = temp;
                }
            }
            return result;
        }
        public override float[] GetFloatSample(Store valueStore, float t)
        {
            return valueStore.GetInterpolatededValueAtT(t);
        }

        public override int[] GetIntSample(Store valueStore, int index)
        {
            return GetFloatSample(valueStore, index).ToInt();
        }

        public override int[] GetIntSample(Store valueStore, float t)
        {
            return GetFloatSample(valueStore, t).ToInt();
        }
    }
}
