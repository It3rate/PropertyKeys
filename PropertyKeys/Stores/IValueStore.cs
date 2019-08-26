using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public interface IValueStore
    {
        int VectorSize { get; }
        int ElementCount { get; set; } 
        int[] Strides { get; set; }
        EasingType[] EasingTypes { get; set; }
        float[] Size { get; }

        float[] GetFloatArrayAtIndex(int index);
        float[] GetFloatArrayAtT(float t);
        float[] GetUnsampledValueAtT(float t);

        void NudgeValuesBy(float nudge);

        float[] GetZeroArray();
        float[] GetMinArray();
        float[] GetMaxArray();
    }
}
