using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PropertyKeys
{
    public class ValueKey
    {
        private static readonly Vector2[] Empty = new Vector2[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        public int ElementCount;
        public int[] Dimensions;

        private readonly Vector2[] Start;
        private readonly Vector2[] End;

        public EasingType[] EasingTypes;
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'Dimension' elements

        public ValueKey(Vector2[] start, Vector2[] end = null, EasingType[] easingTypes = null, int elementCount = -1,
            int[] dimensions = null, bool isDiscrete = false, bool isRepeating = false)
        {
            ElementCount = (elementCount < 1) ? start.Length : elementCount; // can be larger or smaller based on sampling
            Start = start;
            End = (end == null) ? Empty : end;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            Dimensions = (dimensions == null) ? DefaultDimensions : dimensions;
            IsDiscrete = isDiscrete;
            IsRepeating = isRepeating;
        }

        public void ApplyAt(float t, bool interpolate, Vector2[] outValues)
        {
            for (int i = 0; i < outValues.Length; i++)
            {
                outValues[i] = GetVector2AtIndex(i, interpolate, t, outValues.Length);
            }
        }
        
        public Vector2 GetVector2AtIndex(int index, bool interpolate, float t)
        {
            return GetVector2AtIndex(index, interpolate, t, ElementCount);
        }
        public Vector2 GetVector2AtIndex(int index, bool interpolate, float t, int elementCount)
        {
            Vector2 result;
            int dim = Dimensions[0];
            if (dim != 0 || Start.Length != elementCount || End.Length != elementCount)
            {
                float index_t = index / ((float)elementCount - 1f);
                if (dim == 0)
                {
                    result = GetVirtualValue(Start, index_t);
                    if(End != null && End.Length > 0)
                    {
                        Vector2 endV = GetVirtualValue(End, index_t);
                        result = Vector2.Lerp(result, endV, t);
                    }
                }
                else
                {
                    int xIndex = index % dim;
                    int yIndex = index / dim;
                    int rows = elementCount / dim;
                    float xt = xIndex / (dim - 0f);
                    float yt = (index / dim) / (float)rows;
                    Vector2 startVx = GetVirtualValue(Start, xt);
                    Vector2 startVy = GetVirtualValue(Start, yt);
                    result = new Vector2(startVx.X, startVy.Y);

                    if (End != null && End.Length > 0)
                    {
                        Vector2 endVx = GetVirtualValue(End, xt);
                        Vector2 endVy = GetVirtualValue(End, yt);
                        Vector2 endV = new Vector2(endVx.X, endVy.Y);
                        result = Vector2.Lerp(result, endV, t);
                    }
                }
            }
            else
            {
                int startIndex = Math.Min(Start.Length - 1, Math.Max(0, index));
                result = Start[startIndex];
                if (End != null && End.Length > 0)
                {
                    int endIndex = Math.Min(Start.Length - 1, Math.Max(0, index));
                    result = Vector2.Lerp(result, End[endIndex], t);
                }
            }
            return result;
        }

        private static Vector2 GetVirtualValue(Vector2[] values, float t)
        {
            Vector2 result;
            if (values.Length > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (values.Length - 1);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(values.Length - 1, Math.Max(0, startIndex));
                if (pos < values.Length - 1)
                {
                    float remainder_t = pos - startIndex;
                    result = Vector2.Lerp(values[startIndex], values[startIndex + 1], remainder_t);
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
