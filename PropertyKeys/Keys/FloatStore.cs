using PropertyKeys.Samplers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public class FloatStore : BaseValueStore
    {
        private Random rnd = new Random();

        private static readonly float[] Empty = new float[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        private int _vectorSize = 1;
        public override int VectorSize => _vectorSize;

        private readonly float[] Values;

        public override int ElementCount { get; set; }
        //public EasingType[] EasingTypes; // per dimension
        //public int[] Strides;
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'Dimension' elements

        public float[] MinBounds { get; private set; }
        public float[] MaxBounds { get; private set; }
        public override float[] Size
        {
            get
            {
                List<float> result = new List<float>();
                for (int i = 0; i < VectorSize; i++)
                {
                    result.Add(MaxBounds[i] - MinBounds[i]);
                }
                return result.ToArray();
            }
        }

        private float[] zeroArray;
        private float[] maxArray;
        private float[] minArray;

        public FloatStore(int vectorSize, float[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            bool isDiscrete = false, bool isRepeating = false, SampleType sampleType = SampleType.Default)
        {
            _vectorSize = vectorSize;
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount; // can be larger or smaller based on sampling
            Strides = (dimensions == null) ? DefaultDimensions : dimensions;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            IsDiscrete = isDiscrete;
            IsRepeating = isRepeating;

            if (sampleType == SampleType.Ring)
            {
                Sampler = new RingSampler();
            }
            else if (sampleType == SampleType.Grid)
            {
                Sampler = new GridSampler();
            }
            else if (sampleType == SampleType.Line)
            {
                Sampler = new LineSampler();
            }
            else if (sampleType == SampleType.Hexagon)
            {
                Sampler = new HexagonSampler();
            }

            zeroArray = GetSizedArray(0);
            maxArray = GetSizedArray(float.MaxValue);
            minArray = GetSizedArray(float.MinValue);

            CalculateBounds();
        }
        public override float[] this[int index]
        {
            get { return GetSizedValuesAt(index); }
        }

        public override void NudgeValuesBy(float nudge)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] += (float)rnd.NextDouble() * nudge - nudge / 2f;
            }
        }

        private void CalculateBounds()
        {
            MinBounds = (float[])maxArray.Clone();
            MaxBounds = (float[])minArray.Clone();

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

        public override float[] BlendValueAtIndex(BaseValueStore end, int index, float t)
        {
            float[] result = GetFloatArrayAtIndex(index);
            if (end != null)
            {
                float[] endAr = end.GetFloatArrayAtIndex(index);
                InterpolateIntoA(result, endAr, t);
            }
            return result;
        }


        public override float[] GetFloatArrayAtIndex(int index)
        {
            return GetValueAtIndex(index, ElementCount);
        }
        public override float GetFloatAtIndex(int index)
        {
            float[] result = GetFloatArrayAtIndex(index);
            return result[0];
        }
        public override Vector2 GetVector2AtIndex(int index)
        {
            float[] result = GetFloatArrayAtIndex(index);
            return new Vector2(result[0], result.Length > 1 ? result[1] : 0);
        }
        public override Vector3 GetVector3AtIndex(int index)
        {
            float[] result = GetFloatArrayAtIndex(index);
            return new Vector3(result[0], result.Length > 1 ? result[1] : 0, result.Length > 2 ? result[2] : 0);
        }
        public override Vector4 GetVector4AtIndex(int index)
        {
            float[] result = GetFloatArrayAtIndex(index);
            return new Vector4(result[0], result.Length > 1 ? result[1] : 0, result.Length > 2 ? result[2] : 0, result.Length > 3 ? result[3] : 0);
        }


        public float[] GetValueAtIndex(int index, int elementCount)
        {
            float[] result;
            bool isZeros = Strides.All(o => o == 0);
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

        public override float[] GetValueAt(float t)
        {
            return GetVirtualValue(t);
        }
        public override void GetValueAt(float t, float[] copyInto)
        {
            float[] result = GetVirtualValue(t);
            for (int i = 0; i < copyInto.Length; i++)
            {
                copyInto[i] = result.Length > i ? result[i] : 0;
            }
        }

        private float[] GetVirtualValue(float t)
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
                    InterpolateIntoA(result, end, remainder_t);
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

        private float[] GetSizedArray(float value)
        {
            List<float> result = new List<float>(VectorSize);
            for (int i = 0; i < VectorSize; i++)
            {
                result.Add(value);
            }
            return result.ToArray();
        }

        private float[] GetSizedValuesAt(int index)
        {
            float[] result = (float[])zeroArray.Clone();
            if(index * VectorSize + VectorSize <= Values.Length)
            {
                Array.Copy(Values, index * VectorSize, result, 0, VectorSize);
            }
            return result;
        }

        private void InterpolateIntoA(float[] a, float[] b, float t)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (i < b.Length)
                {
                    a[i] += (b[i] - a[i]) * t;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
