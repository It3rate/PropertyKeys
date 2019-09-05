using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public class Store
    {
        protected static Random Rnd = new Random();
        protected static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        protected static readonly int[] DefaultStrides = new int[] { 0 }; // zero means repeating, so this is a regular one row array

        protected Series Series { get; set; }

        // move to primitive class that can be float or int arrays.
        //public int VectorSize { get; } = 1;
        //public int ElementCount { get; set; } = 1;
        //public abstract int InternalDataCount { get; }
        //public virtual float[] MinBounds { get; protected set; }
        //public virtual float[] MaxBounds { get; protected set; }
        //public virtual float[] Size
        //{
        //    get
        //    {
        //        float[] result = FloatStore.GetZeroArray(VectorSize);
        //        for (int i = 0; i < VectorSize; i++)
        //        {
        //            result[i] = MaxBounds[i] - MinBounds[i];
        //        }
        //        return result;
        //    }
        //}
        //public int[] Strides { get; set; } // move to grid/hex samplers
        
        public EasingType[] EasingTypes { get; set; } // move to properties? No, useful for creating virtual data.
        protected BaseSampler Sampler { get; set; }
        public virtual GraphicsPath GetPath(){return null;} // move to static method on Bezier store
        public int VirtualCount
        {
            get => Series.VirtualCount;
            set => Series.VirtualCount = value;
        }

        public Store(Series series, BaseSampler sampler = null, EasingType[] easingTypes = null)
        {
            Series = series;
            Sampler = sampler ?? new LineSampler();
            EasingTypes = easingTypes ?? DefaultEasing;
        }
        public Store(int[] data, BaseSampler sampler = null, EasingType[] easingTypes = null) : this(new IntSeries(1, data), sampler, easingTypes) {}
        public Store(float[] data, BaseSampler sampler = null, EasingType[] easingTypes = null) : this(new FloatSeries(1, data), sampler, easingTypes) {}

        public Series GetValueAtIndex(int index)
        {
            Series result;
            if (Sampler != null)
            {
                result = Sampler.GetValueAtIndex(Series, index);
            }
            else // direct sample of data
            {
                result = Series.GetValueAtIndex(index);
            }
            return result;
        }

        public Series GetValueAtT(float t)
        {
            Series result;
            if (Sampler != null)
            {
                result = Sampler.GetValueAtT(Series, t);
            }
            else // direct sample of data
            {
                result = GetValueAtT(t);
            }
            return result;
        }

        public void HardenToData()
        {
        }


        //public Store(int vectorSize, int[] dimensions = null, EasingType[] easingTypes = null)
        //{
        //    VectorSize = vectorSize;
        //    Strides = dimensions ?? DefaultStrides;
        //    EasingTypes = easingTypes ?? DefaultEasing;
        //}

        // these should be Vector GetValueAtIndex/T - Vector being convertible to float or int arrays.
        //public abstract float[] GetFloatArrayAtIndex(int index);
        //public abstract float[] GetFloatArrayAtT(float t);
        //public abstract int[] GetIntArrayAtIndex(int index);
        //public abstract int[] GetIntArrayAtT(float t);

        //public abstract float[] GetInterpolatededValueAtT(float t);

        //protected virtual bool BoundsDataReady() => false;
        //protected void CalculateBounds(float[] values)
        //{
        //    if (BoundsDataReady())
        //    {
        //        MinBounds = FloatStore.GetMaxArray(VectorSize);
        //        MaxBounds = FloatStore.GetMinArray(VectorSize);

        //        for (int i = 0; i < values.Length; i += VectorSize)
        //        {
        //            for (int j = 0; j < VectorSize; j++)
        //            {
        //                if (values[i + j] < MinBounds[j])
        //                {
        //                    MinBounds[j] = values[i + j];
        //                }

        //                if (values[i + j] > MaxBounds[j])
        //                {
        //                    MaxBounds[j] = values[i + j];
        //                }
        //            }
        //        }
        //    }
        //}
        //public float[] GetZeroArray() { return DataUtils.GetFloatZeroArray(VectorSize); }
        //public float[] GetMinArray() { return DataUtils.GetFloatMinArray(VectorSize); }
        //public float[] GetMaxArray() { return DataUtils.GetFloatMaxArray(VectorSize); }

        //public static float[] GetZeroArray(int size) { return DataUtils.GetFloatZeroArray(size); }
        //public static float[] GetMinArray(int size) { return DataUtils.GetFloatMinArray(size); }
        //public static float[] GetMaxArray(int size) { return DataUtils.GetFloatMaxArray(size); }
    }
}
