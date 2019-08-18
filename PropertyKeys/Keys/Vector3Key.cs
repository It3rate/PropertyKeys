using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public enum SampleType
    {
        Default,
        Line,
        Grid,
        Ring,
    }
    public class Vector3Key
    {
        private static readonly Vector3[] Empty = new Vector3[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        private const int VectorSize = 3;
        private readonly Vector3[] Values;

        public int ElementCount;
        public EasingType[] EasingTypes; // per dimension
        public int[] Strides;
        public SampleType SampleType;
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'Dimension' elements

        public Vector3Key(Vector3[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            bool isDiscrete = false, bool isRepeating = false, SampleType sampleType = SampleType.Default)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length : elementCount; // can be larger or smaller based on sampling
            Strides = (dimensions == null) ? DefaultDimensions : dimensions;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            IsDiscrete = isDiscrete;
            IsRepeating = isRepeating;
            SampleType = sampleType;
        }

        public void ApplyAt(float t, bool interpolate, Vector3[] outValues, Vector3Key endKey)
        {
            for (int i = 0; i < outValues.Length; i++)
            {
                outValues[i] = GetVector3AtIndex(i, interpolate, t, outValues.Length, endKey);
            }
        }

        public Vector3 GetVector3AtIndex(int index, bool interpolate, float t, Vector3Key end)
        {
            return GetVector3AtIndex(index, interpolate, t, ElementCount, end);
        }
        public Vector3 GetVector3AtIndex(int index, bool interpolate, float t, int elementCount, Vector3Key endKey)
        {
            Vector3 result;
            bool isZeros = Strides.All(o => o == 0);
            if (isZeros || Values.Length != elementCount || endKey.Values.Length != elementCount)
            {
                if (isZeros)
                {
                    float index_t = index / ((float)elementCount - 1f);
                    result = GetVirtualValue(Values, index_t);
                    if (endKey != null && endKey.Values.Length > 0)
                    {
                        Vector3 endV = GetVirtualValue(endKey.Values, index_t);
                        result = Vector3.Lerp(result, endV, t);
                    }
                }
                else
                {
                    if(SampleType == SampleType.Default || SampleType == SampleType.Grid)
                    {
                        result = GridSample(index, interpolate, t, elementCount, endKey);
                    }
                    else
                    {
                        result = RingSample(index, interpolate, t, elementCount, endKey);
                    }
                }
            }
            else
            {
                int startIndex = Math.Min(Values.Length - 1, Math.Max(0, index));
                result = Values[startIndex];
                if (endKey != null && endKey.Values.Length > 0)
                {
                    int endIndex = Math.Min(Values.Length - 1, Math.Max(0, index));
                    result = Vector3.Lerp(result, endKey.Values[endIndex], t);
                }
            }
            return result;
        }

        private Vector3 RingSample(int index, bool interpolate, float t, int elementCount, Vector3Key endKey)
        {
            Vector3 result;
            float index_t = index / (float)(elementCount - 1f);

            Vector3 tls = GetVirtualValue(Values, 0);
            Vector3 brs = GetVirtualValue(Values, 1);
            //Vector3 tle = GetVirtualValue(endKey.Values, 0);
            //Vector3 bre = GetVirtualValue(endKey.Values, 1);
            //Vector3 tl = Vector3.Lerp(tls, tle, t);
            //Vector3 br = Vector3.Lerp(brs, bre, t);

            float dx = (brs.X - tls.X) / 2.0f;
            float dy = (brs.Y - tls.Y) / 2.0f;
            result = new Vector3(
                (float)(Math.Sin(index_t * 2.0f * Math.PI + Math.PI) * dx + tls.X + dx),
                (float)(Math.Cos(index_t * 2.0f * Math.PI + Math.PI) * dy + tls.Y + dy), 0);
            return result;
        }

        private Vector3 GridSample(int index, bool interpolate, float t, int elementCount, Vector3Key endKey)
        {
            endKey = null;
            Vector3 result;
            float dimT = 1f;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = new float[] { 0, 0, 0 };
            float[] start = new float[] { 0, 0, 0 };
            float[] end = new float[] { 0, 0, 0 };
            for (int i = 0; i < VectorSize; i++)
            {
                // first zero results in fill to end
                bool isLast = (Strides.Length - 1 < i) || (Strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(elementCount / (curSize + prevSize));
                }
                else
                {
                    prevSize = curSize;
                    curSize *= Strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }
                GetVirtualValue(Values, dimT).CopyTo(temp);
                start[i] = temp[i];
                if (endKey != null && endKey.Values.Length > 0)
                {
                    GetVirtualValue(endKey.Values, dimT).CopyTo(temp);
                    end[i] = temp[i];
                }

                if (isLast)
                {
                    break;
                }
            }
            result = new Vector3(start[0], start[1], start[2]);
            if (endKey != null && endKey.Values.Length > 0)
            {
                result = Vector3.Lerp(result, new Vector3(end[0], end[1], end[2]), t);
            }
            return result;
        }

        private static Vector3 GetVirtualValue(Vector3[] values, float t)
        {
            Vector3 result;
            if (values.Length > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (values.Length - 1);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(values.Length - 1, Math.Max(0, startIndex));
                if (pos < values.Length - 1)
                {
                    float remainder_t = pos - startIndex;
                    result = Vector3.Lerp(values[startIndex], values[startIndex + 1], remainder_t);
                }
                else
                {
                    result = values[startIndex];
                }
            }
            else
            {
                result = values[0];
            }
            return result;
        }

    }
}
