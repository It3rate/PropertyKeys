using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public interface IValueStore
    {
        int ElementCount { get; set; }

        float[] GetFloatArrayAtIndex(int index);
        float[] GetFloatArrayAtT(float t);
        float[] GetUnsampledValueAtT(float t);

        void NudgeValuesBy(float nudge);

        float[] GetZeroArray();
        float[] GetMinArray();
        float[] GetMaxArray();
    }
}
