using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class RingSampler : BaseSampler
    {
        public override float[] GetSample(IValueStore valueStore, int index)
        {
            float index_t = index / (valueStore.ElementCount - 1f); // full circle
            return GetSample(valueStore, index_t);
        }

        public override float[] GetSample(IValueStore valueStore, float t)
        {
            float[] result;
            float[] tl = valueStore.GetUnsampledValueAtT(0f);
            float[] br = valueStore.GetUnsampledValueAtT(1f);

            float dx = (br[0] - tl[0]) / 2.0f;
            float dy = (br[1] - tl[1]) / 2.0f;
            result = new float[] {
                (float)(Math.Sin(t * 2.0f * Math.PI + Math.PI) * dx + tl[0] + dx),
                (float)(Math.Cos(t * 2.0f * Math.PI + Math.PI) * dy + tl[1] + dy) };
            return result;
        }
    }
}
