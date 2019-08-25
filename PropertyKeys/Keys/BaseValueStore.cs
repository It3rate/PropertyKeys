using PropertyKeys.Samplers;
using System.Drawing;
using System.Numerics;

namespace PropertyKeys.Keys
{
    public abstract class BaseValueStore : IValueStore
    {
        public int[] Strides { get; set; }
        public EasingType[] EasingTypes { get; set; }
        public abstract int VectorSize{ get; }

        public BaseSampler Sampler { get; set; }

        public abstract float[] Size { get; }

        public abstract float[] this[int index] { get; }

        // todo: elementCount probably needs to come from the parent, at least optionally. Or repeat/loop? Or param (another t vs index?)
        // Eg does color count need to equal positions count?
        public abstract int ElementCount { get; set; } 
        public abstract float[] GetFloatArrayAtIndex(int index);
        public abstract float[] GetValueAt(float t);
        public abstract float[] BlendValueAtIndex(BaseValueStore end, int index, float t);

        public abstract void GetValueAt(float t, float[] copyInto);

        public abstract float GetFloatAtIndex(int index);
        public abstract Vector2 GetVector2AtIndex(int index);
        public abstract Vector3 GetVector3AtIndex(int index);
        public abstract Vector4 GetVector4AtIndex(int index);
        
        public abstract void NudgeValuesBy(float nudge);

    }
}
