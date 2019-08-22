using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class RingSampler : BaseSampler
    {
        public override float[] GetSample(ValueKey valueKey, int index, float t, int elementCount)
        {
            float[] result;
            float index_t = index / (float)(elementCount - 1f); // full circle

            float[] tl = valueKey.GetVirtualValue(0f);
            float[] br = valueKey.GetVirtualValue(1f);

            float dx = (br[0] - tl[0]) / 2.0f;
            float dy = (br[1] - tl[1]) / 2.0f;
            result = new float[] {
                (float)(Math.Sin(index_t * 2.0f * Math.PI + Math.PI) * dx + tl[0] + dx),
                (float)(Math.Cos(index_t * 2.0f * Math.PI + Math.PI) * dy + tl[1] + dy) };
            return result;
        }
    }
}
