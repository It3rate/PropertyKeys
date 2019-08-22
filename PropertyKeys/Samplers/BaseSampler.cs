using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public abstract class BaseSampler
    {
        public abstract float[] GetSample(ValueKey valueKey, int index, float t, int elementCount);
    }
}
