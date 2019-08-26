﻿using DataArcs.Samplers;
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

        public int VectorSize { get; } = 1;
        public int ElementCount { get; set; } = 1;
        public int[] Strides { get; set; }
        public EasingType[] EasingTypes { get; set; }
        public BaseSampler Sampler { get; set; }

        private readonly float[] Values;

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

        public FloatStore(int vectorSize, float[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            SampleType sampleType = SampleType.Default)
        {
            VectorSize = vectorSize;
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount; // can be larger or smaller based on sampling
            Strides = (dimensions == null) ? DefaultDimensions : dimensions;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            Sampler = BaseSampler.CreateSampler(sampleType);

            CalculateBounds();
        }
        public float[] this[int index]
        {
            get { return GetSizedValuesAt(index); }
        }

        public void NudgeValuesBy(float nudge)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] += (float)rnd.NextDouble() * nudge - nudge / 2f;
            }
        }

        public float[] GetFloatArrayAtIndex(int index)
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

        public float[] GetFloatArrayAtT(float t)
        {
            throw new NotImplementedException();
        }

        public float[] GetUnsampledValueAtT(float t)
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
        
        private float[] GetSizedValuesAt(int index)
        {
            float[] result = GetZeroArray();
            if(index * VectorSize + VectorSize <= Values.Length)
            {
                Array.Copy(Values, index * VectorSize, result, 0, VectorSize);
            }
            return result;
        }


        private void CalculateBounds()
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

        public float[] GetZeroArray() { return DataUtils.GetZeroArray(VectorSize); }
        public float[] GetMinArray() { return DataUtils.GetMinArray(VectorSize); }
        public float[] GetMaxArray() { return DataUtils.GetMaxArray(VectorSize); }
    }
}
