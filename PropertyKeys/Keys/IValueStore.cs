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
        float[] GetValueAt(float t);
        float[] BlendValueAtIndex(BaseValueStore endKey, int index, float t);
    }
}
