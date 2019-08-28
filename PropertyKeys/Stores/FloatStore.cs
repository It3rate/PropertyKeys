using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class FloatStore : Store
    {
        protected float[] Values { get; set; }
        protected BaseSampler Sampler { get; set; }

        public override float[] GetFloatValues => (float[])Values.Clone();
        public override int[] GetIntValues => Values.ToInt();
        public float[] this[int index] => GetSizedValuesAt(index);

        public FloatStore(int vectorSize, float[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            SampleType sampleType = SampleType.Default) : base(vectorSize, dimensions, easingTypes)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount; // can be larger or smaller based on sampling
            Sampler = BaseSampler.CreateSampler(sampleType);
            CalculateBounds(Values);
        }
        protected override bool BoundsDataReady() => Values != null;

        public void NudgeValuesBy(float nudge)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] += (float)rnd.NextDouble() * nudge - nudge / 2f;
            }
        }

        public override float[] GetFloatArrayAtIndex(int index)
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

        public override float[] GetFloatArrayAtT(float t)
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
        public override int[] GetIntArrayAtIndex(int index)
        {
            return GetFloatArrayAtIndex(index).ToInt();
        }
        public override int[] GetIntArrayAtT(float t)
        {
            return GetFloatArrayAtT(t).ToInt();
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


        public static FloatStore CreateLerp(int vectorSize)
        {
            float[] start = DataUtils.GetSizedFloatArray(vectorSize, 0f);
            float[] end = DataUtils.GetSizedFloatArray(vectorSize, 1f);
            float[] values = DataUtils.CombineFloatArrays(start, end);
            return new FloatStore(vectorSize, values, sampleType: SampleType.Line);
        }

        public static FloatStore CreateBox(int vectorSize, int rows, int cols, SampleType sampleType)
        {
            float[] start = DataUtils.GetSizedFloatArray(vectorSize, 0f);
            float[] end = DataUtils.GetSizedFloatArray(vectorSize, 1f);
            float[] values = DataUtils.CombineFloatArrays(start, end);
            return new FloatStore(2, values, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 }, sampleType: sampleType);
        }
        public static FloatStore CreateGrid(int vectorSize, int rows, int cols)
        {
            return CreateBox(vectorSize, rows, cols, SampleType.Grid);
        }
        public static FloatStore CreateHexGrid(int vectorSize, int rows, int cols)
        {
            return CreateBox(vectorSize, rows, cols, SampleType.Hexagon);
        }

        public float[] GetZeroArray() { return DataUtils.GetFloatZeroArray(VectorSize); }
        public float[] GetMinArray() { return DataUtils.GetFloatMinArray(VectorSize); }
        public float[] GetMaxArray() { return DataUtils.GetFloatMaxArray(VectorSize); }

        public static float[] GetZeroArray(int size) { return DataUtils.GetFloatZeroArray(size); }
        public static float[] GetMinArray(int size) { return DataUtils.GetFloatMinArray(size); }
        public static float[] GetMaxArray(int size) { return DataUtils.GetFloatMaxArray(size); }

        public override GraphicsPath GetPath()
        {
            throw new NotImplementedException();
        }
    }
}
