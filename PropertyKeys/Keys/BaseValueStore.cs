using PropertyKeys.Samplers;
using System.Drawing;
using System.Numerics;

namespace PropertyKeys.Keys
{
    public abstract class BaseValueStore : IValueStore
    {
        private float[] zeroArray;
        private float[] maxArray;
        private float[] minArray;
        public int VectorSize { get; } = 1;

        public BaseValueStore(int vectorSize)
        {
            VectorSize = vectorSize;
            zeroArray = GetSizedArray(0);
            minArray = GetSizedArray(float.MinValue);
            maxArray = GetSizedArray(float.MaxValue);
        }

        public int[] Strides { get; set; }
        public EasingType[] EasingTypes { get; set; }

        public BaseSampler Sampler { get; set; }

        public abstract float[] Size { get; }

        public abstract float[] this[int index] { get; }

        // todo: elementCount probably needs to come from the parent, at least optionally. Or repeat/loop? Or param (another t vs index?)
        // Eg does color count need to equal positions count?
        public abstract int ElementCount { get; set; } 
        public abstract float[] GetFloatArrayAtIndex(int index);
        public abstract float[] GetFloatArrayAtT(float t);
        public abstract float[] GetUnsampledValueAtT(float t);
        public abstract float[] BlendValueAtIndex(IValueStore end, int index, float t);
        public abstract float[] BlendValueAtT(IValueStore end, float index_t, float t);

        public abstract void GetUnsampledValueAt(float t, float[] copyInto);
        
        public abstract void NudgeValuesBy(float nudge);


        private float[] GetSizedArray(float value)
        {
            float[] result = new float[VectorSize];
            for (int i = 0; i < VectorSize; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public float[] GetZeroArray() { return (float[])zeroArray.Clone(); }
        public float[] GetMinArray() { return (float[])minArray.Clone(); }
        public float[] GetMaxArray() { return (float[])maxArray.Clone(); }
    }
}
