using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class LineSampler : BaseSampler
    {
        public override float[] GetSample(ValueKey valueKey, int index)
        {
            float index_t = (valueKey.ElementCount > 1) ?  index / (valueKey.ElementCount - 1f) : 0f;
            return valueKey.GetVirtualValue(index_t);
        }
    }
}
