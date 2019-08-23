using PropertyKeys.Samplers;
using System;
using System.Linq;
using System.Numerics;

namespace PropertyKeys.Keys
{
    public enum SampleType
    {
        Default,
        Line,
        Grid,
        Ring,
        Hexagon,
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
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'Dimension' elements

        public Vector3 MinBounds { get; private set; }
        public Vector3 MaxBounds { get; private set; }
        public override float[] Size => new float[] { MaxBounds.X - MinBounds.X, MaxBounds.Y - MinBounds.Y, MaxBounds.Z - MinBounds.Z };

        public Vector3Key(Vector3[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null,
            bool isDiscrete = false, bool isRepeating = false, SampleType sampleType = SampleType.Default)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length : elementCount; // can be larger or smaller based on sampling
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

            CalculateBounds();
        }

        private void CalculateBounds()
        {
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float minz = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;
            float maxz = float.MinValue;
            foreach (Vector3 val in Values)
            {
                minx = val.X < minx ? val.X : minx;
                miny = val.Y < miny ? val.Y : miny;
                minz = val.Z < minz ? val.Z : minz;
                maxx = val.X > maxx ? val.X : maxx;
                maxy = val.Y > maxy ? val.Y : maxy;
                maxz = val.Z > maxz ? val.Z : maxz;
            }
            MinBounds = new Vector3(minx, miny, minz);
            MaxBounds = new Vector3(maxx, maxy, maxz);
        }

        public override float[] BlendValueAtIndex(ValueKey endKey, int index, float t)
        {
            float[] result;
            // todo: Getting grid size from data doesn't make sense, probably need to pass it with grid sampling class? Or calc from bounds of data (yes)?
            Vector3 start = GetVector3AtIndex(index);
            if (endKey!= null)
            {
                float[] endAr = endKey.GetFloatArrayAtIndex(index); // in case this isn't vect3, always use start size
                Vector3 end = ValueKey.MergeToVector3(start, endAr);
                result = new float[] { 0, 0, 0 };
                Vector3.Lerp(start, end, t).CopyTo(result);
            }
            else
            {
                result = GetFloatArray(start);
            }
            return result;
        }

        public override float[] GetFloatArrayAtIndex(int index)
        {
            float[] temp = new float[] { 0, 0, 0 };
            GetVector3AtIndex(index, ElementCount).CopyTo(temp);
            return temp;
        }
        public override float GetFloatAtIndex(int index)
        {
            Vector3 result = GetVector3AtIndex(index, ElementCount);
            return result.X;
        }
        public override Vector2 GetVector2AtIndex(int index)
        {
            Vector3 result = GetVector3AtIndex(index, ElementCount);
            return new Vector2(result.X, result.Y);
        }
        public override Vector3 GetVector3AtIndex(int index)
        {
            return GetVector3AtIndex(index, ElementCount);
        }
        public override Vector4 GetVector4AtIndex(int index)
        {
            Vector3 result = GetVector3AtIndex(index, ElementCount);
            return new Vector4(result.X, result.Y, result.Z, 0);
        }


        public Vector3 GetVector3AtIndex(int index, int elementCount)
        {
            Vector3 result;
            bool isZeros = Strides.All(o => o == 0);
            if (Sampler != null)
            {
                float[] sample = Sampler.GetSample(this, index);
                result = GetVector3(sample);
            }
            else // direct sample of data
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

    }
}
