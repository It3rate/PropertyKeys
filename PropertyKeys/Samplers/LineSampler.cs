using PropertyKeys.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class LineSampler : BaseSampler
    {
        public override float[] GetSample(BaseValueStore valueStore, int index)
        {
            float index_t = (valueStore.ElementCount > 1) ?  index / (valueStore.ElementCount - 1f) : 0f;
            return valueStore.GetValueAt(index_t);
        }
        public override float[] GetSample(BaseValueStore valueStore, float t)
        {
            return valueStore.GetValueAt(t);
        }
    }
}
