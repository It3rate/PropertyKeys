using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public abstract class Store
    {
        protected static Random Rnd = new Random();
        protected static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        protected static readonly int[] DefaultStrides = new int[] { 0 }; // zero means repeating, so this is a regular one row array
        
        // move to primitive class that can be float or int arrays.
        public abstract float[] GetFloatValues { get; }
        public abstract int[] GetIntValues { get; }
        public float CurrentT { get; set; } = 0;
        public int VectorSize { get; } = 1;
        public int ElementCount { get; set; } = 1;
        public abstract int InternalDataCount { get; }
        public virtual float[] MinBounds { get; protected set; }
        public virtual float[] MaxBounds { get; protected set; }
        public virtual float[] Size
        {
            get
            {
                float[] result = FloatStore.GetZeroArray(VectorSize);
                for (int i = 0; i < VectorSize; i++)
                {
                    result[i] = MaxBounds[i] - MinBounds[i];
                }
                return result;
            }
        }
        public EasingType[] EasingTypes { get; set; } // move to properties? No, useful for creating virtual data.

        public int[] Strides { get; set; } // move to grid/hex samplers
        protected BaseSampler Sampler { get; set; }
        public abstract GraphicsPath GetPath(); // move to static method on Bezier store


        public Store(int vectorSize, int[] dimensions = null, EasingType[] easingTypes = null)
        {
            VectorSize = vectorSize;
            Strides = dimensions ?? DefaultStrides;
            EasingTypes = easingTypes ?? DefaultEasing;
        }

        // these should be Vector GetValueAtIndex/T - Vector being convertible to float or int arrays.
        public abstract float[] GetFloatArrayAtIndex(int index);
        public abstract float[] GetFloatArrayAtT(float t);
        public abstract int[] GetIntArrayAtIndex(int index);
        public abstract int[] GetIntArrayAtT(float t);

        public abstract float[] GetInterpolatededValueAtT(float t);
        public abstract void ReplaceSamplerWithData();

        protected virtual bool BoundsDataReady() => false;
        protected void CalculateBounds(float[] values)
        {
            if (BoundsDataReady())
            {
                MinBounds = FloatStore.GetMaxArray(VectorSize);
                MaxBounds = FloatStore.GetMinArray(VectorSize);

                for (int i = 0; i < values.Length; i += VectorSize)
                {
                    for (int j = 0; j < VectorSize; j++)
                    {
                        if (values[i + j] < MinBounds[j])
                        {
                            MinBounds[j] = values[i + j];
                        }

                        if (values[i + j] > MaxBounds[j])
                        {
                            MaxBounds[j] = values[i + j];
                        }
                    }
                }
            }
        }
        public float[] GetZeroArray() { return DataUtils.GetFloatZeroArray(VectorSize); }
        public float[] GetMinArray() { return DataUtils.GetFloatMinArray(VectorSize); }
        public float[] GetMaxArray() { return DataUtils.GetFloatMaxArray(VectorSize); }

        public static float[] GetZeroArray(int size) { return DataUtils.GetFloatZeroArray(size); }
        public static float[] GetMinArray(int size) { return DataUtils.GetFloatMinArray(size); }
        public static float[] GetMaxArray(int size) { return DataUtils.GetFloatMaxArray(size); }
    }
}
