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
        public int ElementCount { get; set; } = 1;
        public int[] Strides { get; set; }
        public EasingType[] EasingTypes { get; set; }
        public BaseSampler Sampler { get; set; }
        public abstract float[] Size { get; }
        public abstract float[] this[int index] { get; }
        

        public BaseValueStore(int vectorSize)
        {
            VectorSize = vectorSize;
            zeroArray = GetSizedArray(0);
            minArray = GetSizedArray(float.MinValue);
            maxArray = GetSizedArray(float.MaxValue);
        }

        public abstract float[] GetFloatArrayAtIndex(int index);
        public abstract float[] GetFloatArrayAtT(float t);
        public abstract float[] GetUnsampledValueAtT(float t);
        
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

        public static void InterpolateInto(float[] result, float[] b, float t)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i < b.Length)
                {
                    result[i] += (b[i] - result[i]) * t;
                }
                else
                {
                    break;
                }
            }
        }

        public float[] GetZeroArray() { return (float[])zeroArray.Clone(); }
        public float[] GetMinArray() { return (float[])minArray.Clone(); }
        public float[] GetMaxArray() { return (float[])maxArray.Clone(); }
    }
}
