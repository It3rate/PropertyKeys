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
    public class Vector3Key : ValueKey
    {
        private static readonly Vector3[] Empty = new Vector3[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        public override int VectorSize => 3;
        private readonly Vector3[] Values;

        public override int ElementCount { get; set; }
        //public EasingType[] EasingTypes; // per dimension
        //public int[] Strides;
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

        public override float[] BlendValueAtIndex(ValueKey endKey, int index, float t)
        {
            // todo: Getting grid size from data doesn't make sense, probably need to pass it with grid sampling class? Or calc from bounds of data (yes)?
            Vector3 start = GetVector3AtIndex(index, 0);
            float[] endAr = endKey.GetFloatArrayAtIndex(index, 1); // in case this isn't vect3, always use start size
            Vector3 end = ValueKey.MergeToVector3(start, endAr);
            float[] temp = new float[] { 0, 0, 0 };
            Vector3.Lerp(start, end, t).CopyTo(temp);
            return temp;
        }

        public override float[] GetFloatArrayAtIndex(int index, float t)
        {
            float[] temp = new float[] { 0, 0, 0 };
            GetVector3AtIndex(index, t, ElementCount).CopyTo(temp);
            return temp;
        }
        public override float GetFloatAtIndex(int index, float t)
        {
            Vector3 result = GetVector3AtIndex(index, t, ElementCount);
            return result.X;
        }
        public override Vector2 GetVector2AtIndex(int index, float t)
        {
            Vector3 result = GetVector3AtIndex(index, t, ElementCount);
            return new Vector2(result.X, result.Y);
        }
        public override Vector3 GetVector3AtIndex(int index, float t)
        {
            return GetVector3AtIndex(index, t, ElementCount);
        }
        public override Vector4 GetVector4AtIndex(int index, float t)
        {
            Vector3 result = GetVector3AtIndex(index, t, ElementCount);
            return new Vector4(result.X, result.Y, result.Z, 0);
        }


        public Vector3 GetVector3AtIndex(int index, float t, int elementCount)
        {
            Vector3 result;
            bool isZeros = Strides.All(o => o == 0);
            if (isZeros || Values.Length != elementCount)
            {
                if (isZeros)
                {
                    float index_t = index / ((float)elementCount - 1f);
                    result = GetVirtualValue(Values, index_t);
                }
                else
                {
                    if(SampleType == SampleType.Default || SampleType == SampleType.Grid)
                    {
                        result = GetVector3(GridSample(this, index, t, elementCount));
                    }
                    else
                    {
                        result = GetVector3(RingSample(this, index, t, elementCount));
                    }
                }
            }
            else
            {
                int startIndex = Math.Min(Values.Length - 1, Math.Max(0, index));
                result = Values[startIndex];
            }
            return result;
        }

        public override float[] GetVirtualValue(float t)
        {
            return GetFloatArray(GetVirtualValue(this.Values, t));
        }
        public override void GetVirtualValue(float t, float[] copyInto)
        {
            GetVirtualValue(this.Values, t).CopyTo(copyInto);
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

        private static float[] RingSample(ValueKey valueKey, int index, float t, int elementCount)
        {
            float[] result;
            float index_t = index / (float)(elementCount - 1f); // full circle

            float[] tl = valueKey.GetVirtualValue(0f);
            float[] br = valueKey.GetVirtualValue(1f);

            float dx = (br[0] - tl[0]) / 2.0f;
            float dy = (br[1] - tl[1]) / 2.0f;
            result = new float[] {
                (float)(Math.Sin(index_t * 2.0f * Math.PI + Math.PI) * dx + tl[0] + dx),
                (float)(Math.Cos(index_t * 2.0f * Math.PI + Math.PI) * dy + tl[1] + dy) };
            return result;
        }

        private static float[] GridSample(ValueKey valueKey, int index, float t, int elementCount)
        {
            float[] result = new float[] { 0, 0, 0 };
            float dimT = 1f;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = new float[] { 0, 0, 0 };
            for (int i = 0; i < valueKey.VectorSize; i++)
            {
                // first zero results in fill to end
                bool isLast = (valueKey.Strides.Length - 1 < i) || (valueKey.Strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(elementCount / (curSize + prevSize));
                }
                else
                {
                    prevSize = curSize;
                    curSize *= valueKey.Strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }

                if (i < valueKey.EasingTypes.Length)
                {
                    dimT = Easing.GetValueAt(dimT, valueKey.EasingTypes[i]);
                }
                valueKey.GetVirtualValue(dimT, temp);
                result[i] = temp[i];

                if (isLast)
                {
                    break;
                }
            }
            return result;
        }


    }
}
