using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PropertyKeys
{
    public abstract class Values
    {
        public abstract void ApplyValues(float t, ValueKey valueKey);

        public abstract bool GetBoolAt(float t);
        public abstract int GetIntAt(float t);
        public abstract float GetFloatAt(float t);
        public abstract Vector2 GetVector2At(float t, bool interpolate); // 2D, grid, circular
        public abstract Vector3 GetVector3At(float t); // color, 3D
        public abstract Vector4 GetVector4At(float t);

        public virtual Values ShallowCopy()
        {
           return (Values)MemberwiseClone();
        }
        public abstract Values DeepCopy();
    }
}
