using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class FloatStore
    {
        private Random rnd = new Random();
        
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        protected float[] Values { get; private set; }

        private int VectorSize { get; } = 1;
        public int ElementCount { get; set; } = 1;
        public int[] Strides { get; set; }
        public EasingType[] EasingTypes { get; set; }
        public BaseSampler Sampler { get; set; }
        public float[] MinBounds { get; private set; }
        public float[] MaxBounds { get; private set; }
        public float[] Size
        {
            get
            {
                float[] result = GetZeroArray();
                for (int i = 0; i < VectorSize; i++)
                {
                    result[i] = MaxBounds[i] - MinBounds[i];
                }
                return result;
            }
        }
        public float[] this[int index]
        {
            get { return GetSizedValuesAt(index); }
        }

        public FloatStore(int vectorSize, float[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            SampleType sampleType = SampleType.Default)
        {
            VectorSize = vectorSize;
            Strides = dimensions ?? DefaultDimensions;
            EasingTypes = easingTypes ?? DefaultEasing;

            Initialize(values, elementCount, sampleType);
        }

        protected virtual void Initialize(float[] values, int elementCount, SampleType sampleType)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount; // can be larger or smaller based on sampling
            Sampler = BaseSampler.CreateSampler(sampleType);
            CalculateBounds();
        }

        public void NudgeValuesBy(float nudge)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] += (float)rnd.NextDouble() * nudge - nudge / 2f;
            }
        }

        public virtual float[] GetFloatArrayAtIndex(int index)
        {
            float[] result;
            if (Sampler != null)
            {
                result = Sampler.GetSample(this, index);
            }
            else // direct sample of data
            {
                int len = Values.Length / VectorSize;
                int startIndex = Math.Min(len - 1, Math.Max(0, index));
                result = GetSizedValuesAt(startIndex);
            }
            return result;
        }

        public virtual float[] GetFloatArrayAtT(float t)
        {
            float[] result;
            if (Sampler != null)
            {
                result = Sampler.GetSample(this, t);
            }
            else // direct sample of data
            {
                result = GetUnsampledValueAtT(t);
            }
            return result;
        }

        public virtual float[] GetUnsampledValueAtT(float t)
        {
            float[] result;
            int len = Values.Length / VectorSize;
            if (len > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (len - 1);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(len - 1, Math.Max(0, startIndex));
                if (pos < len - 1)
                {
                    float remainder_t = pos - startIndex;
                    result = GetSizedValuesAt(startIndex);
                    float[] end = GetSizedValuesAt(startIndex + 1);
                    DataUtils.InterpolateInto(result, end, remainder_t);
                }
                else
                {
                    result = GetSizedValuesAt(startIndex);
                }
            }
            else
            {
                result = GetSizedValuesAt(0);
            }
            return result;
        }

        protected virtual float[] GetSizedValuesAt(int index)
        {
            float[] result = GetZeroArray();
            if(index * VectorSize + VectorSize <= Values.Length)
            {
                Array.Copy(Values, index * VectorSize, result, 0, VectorSize);
            }
            return result;
        }


        protected virtual void CalculateBounds()
        {
            MinBounds = GetMaxArray();
            MaxBounds = GetMinArray();

            for (int i = 0; i < Values.Length; i += VectorSize)
            {
                for (int j = 0; j < VectorSize; j++)
                {
                    if (Values[i + j] < MinBounds[j])
                    {
                        MinBounds[j] = Values[i + j];
                    }

                    if (Values[i + j] > MaxBounds[j])
                    {
                        MaxBounds[j] = Values[i + j];
                    }
                }
            }
        }

        public static FloatStore CreateLerp(int vectorSize)
        {
            float[] start = DataUtils.GetSizedArray(vectorSize, 0f);
            float[] end = DataUtils.GetSizedArray(vectorSize, 1f);
            float[] values = DataUtils.CombineArrays(start, end);
            return new FloatStore(vectorSize, values, sampleType: SampleType.Line);
        }
        public static FloatStore CreateGrid(int vectorSize, int rows, int cols)
        {
            float[] start = DataUtils.GetSizedArray(vectorSize, 0f);
            float[] end = DataUtils.GetSizedArray(vectorSize, 1f);
            float[] values = DataUtils.CombineArrays(start, end);
            return new FloatStore(2, values, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Grid);
        }

        public static FloatStore CreateHexGrid(int vectorSize, int rows, int cols)
        {
            float[] start = DataUtils.GetSizedArray(vectorSize, 0f);
            float[] end = DataUtils.GetSizedArray(vectorSize, 1f);
            float[] values = DataUtils.CombineArrays(start, end);
            return new FloatStore(2, values, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Hexagon);
        }
        public float[] GetZeroArray() { return DataUtils.GetZeroArray(VectorSize); }
        public float[] GetMinArray() { return DataUtils.GetMinArray(VectorSize); }
        public float[] GetMaxArray() { return DataUtils.GetMaxArray(VectorSize); }
    }
}
