using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public class Vector3Key
    {
        private static readonly Vector3[] Empty = new Vector3[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        private const int VectorSize = 3;

        public int ElementCount;
        public int[] Strides;

        private readonly Vector3[] Start;
        private readonly Vector3[] End;

        public EasingType[] EasingTypes;
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'Dimension' elements

        public Vector3Key(Vector3[] start, Vector3[] end = null, EasingType[] easingTypes = null, int elementCount = -1,
            int[] dimensions = null, bool isDiscrete = false, bool isRepeating = false)
        {
            ElementCount = (elementCount < 1) ? start.Length : elementCount; // can be larger or smaller based on sampling
            Start = start;
            End = (end == null) ? Empty : end;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            Strides = (dimensions == null) ? DefaultDimensions : dimensions;
            IsDiscrete = isDiscrete;
            IsRepeating = isRepeating;
        }

        public void ApplyAt(float t, bool interpolate, Vector3[] outValues)
        {
            for (int i = 0; i < outValues.Length; i++)
            {
                outValues[i] = GetVector3AtIndex(i, interpolate, t, outValues.Length);
            }
        }

        public Vector3 GetVector3AtIndex(int index, bool interpolate, float t)
        {
            return GetVector3AtIndex(index, interpolate, t, ElementCount);
        }
        public Vector3 GetVector3AtIndex(int index, bool interpolate, float t, int elementCount)
        {
            Vector3 result;
            bool isZeros = Strides.All(o => o == 0);
            if (isZeros || Start.Length != elementCount || End.Length != elementCount)
            {
                if (isZeros)
                {
                    float index_t = index / ((float)elementCount - 1f);
                    result = GetVirtualValue(Start, index_t);
                    if (End != null && End.Length > 0)
                    {
                        Vector3 endV = GetVirtualValue(End, index_t);
                        result = Vector3.Lerp(result, endV, t);
                    }
                }
                else
                {
                    result = RingSample(index, interpolate, t, elementCount);
                    //result = GridSample(index, interpolate, t, elementCount);
                }
            }
            else
            {
                int startIndex = Math.Min(Start.Length - 1, Math.Max(0, index));
                result = Start[startIndex];
                if (End != null && End.Length > 0)
                {
                    int endIndex = Math.Min(Start.Length - 1, Math.Max(0, index));
                    result = Vector3.Lerp(result, End[endIndex], t);
                }
            }
            return result;
        }

        private Vector3 RingSample(int index, bool interpolate, float t, int elementCount)
        {
            Vector3 result;
            float index_t = index / (float)elementCount;

            Vector3 tls = GetVirtualValue(Start, 0);
            Vector3 brs = GetVirtualValue(Start, 1);
            Vector3 tle = GetVirtualValue(End, 0);
            Vector3 bre = GetVirtualValue(End, 1);
            Vector3 tl = Vector3.Lerp(tls, tle, t);
            Vector3 br = Vector3.Lerp(brs, bre, t);

            float dx = (br.X - tl.X) / 2.0f;
            float dy = (br.Y - tl.Y) / 2.0f;
            result = new Vector3(
                (float)(Math.Sin(index_t * 2.0f * Math.PI) * dx + tl.X + dx),
                (float)(Math.Cos(index_t * 2.0f * Math.PI) * dy + tl.Y + dy), 0);
            return result;

            //Vector3 start = GetVirtualValue(Start, index_t);
            //Vector3 end = start;
            //Vector3 vt = start;
            //if (End != null && End.Length > 0)
            //{
            //    end = GetVirtualValue(End, index_t);
            //    vt = Vector3.Lerp(start, end, t);
            //}
            //return vt;

        }

        private Vector3 GridSample(int index, bool interpolate, float t, int elementCount)
        {
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
                GetVirtualValue(Start, dimT).CopyTo(temp);
                start[i] = temp[i];
                if (End != null && End.Length > 0)
                {
                    GetVirtualValue(End, dimT).CopyTo(temp);
                    end[i] = temp[i];
                }

                if (isLast)
                {
                    break;
                }
            }
            result = new Vector3(start[0], start[1], start[2]);
            if (End != null && End.Length > 0)
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
