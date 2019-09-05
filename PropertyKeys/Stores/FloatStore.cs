using DataArcs.Samplers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    // todo: public UI access comes from a store property only (via inspection if needed). Add onChanged etc events.
    public class FloatStore : Store, IEnumerable<float>
    {
        protected float[] Values { get; set; }

        public override float[] GetFloatValues => (float[])Values.Clone();
        public override int[] GetIntValues => Values.ToInt();

        public override int InternalDataCount => Values.Length;

        public float[] this[int index] => GetSizedValuesAt(index);

        public FloatStore(int vectorSize, float[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            SampleType sampleType = SampleType.Default) : base(vectorSize, dimensions, easingTypes)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount; // can be larger or smaller based on sampling
            Sampler = BaseSampler.CreateSampler(sampleType, dimensions);
            CalculateBounds(Values);
        }
        public FloatStore(int vectorSize, params float[] values) : base(vectorSize, null, null)
        {
            Values = values;
            ElementCount = values.Length / VectorSize;
            Sampler = BaseSampler.CreateSampler(SampleType.Default);
            CalculateBounds(Values);
        }
        protected override bool BoundsDataReady() => Values != null;

        public void NudgeValuesBy(float nudge)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] += (float)Rnd.NextDouble() * nudge - nudge / 2f;
            }
        }

        public override float[] GetFloatArrayAtIndex(int index)
        {
            float[] result;
            if (Sampler != null)
            {
                result = Sampler.GetFloatSample(this, index);
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
                result = Sampler.GetFloatSample(this, t);
            }
            else // direct sample of data
            {
                result = GetInterpolatededValueAtT(t);
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

        public override float[] GetInterpolatededValueAtT(float t)
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
                    float remainderT = pos - startIndex;
                    result = GetSizedValuesAt(startIndex);
                    float[] end = GetSizedValuesAt(startIndex + 1);
                    DataUtils.InterpolateInto(result, end, remainderT);
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

        public override void HardenToData()
        {
            float[] concreteValues = new float[ElementCount * VectorSize];
            int index = 0;
            for (int i = 0; i < concreteValues.Length; i += VectorSize)
            {
                float[] vals = GetFloatArrayAtIndex(index);
                for (int j = 0; j < VectorSize; j++)
                {
                    concreteValues[i + j] = vals[j];
                }
                index++;
            }

            Values = concreteValues;
            Sampler = null;
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


        public override GraphicsPath GetPath()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<float> GetEnumerator()
        {
            return ((IEnumerable<float>)Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<float>)Values).GetEnumerator();
        }
    }
}
