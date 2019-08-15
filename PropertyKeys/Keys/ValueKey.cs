using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PropertyKeys
{
    public abstract class ValueKey
    {
        private static readonly Vector2[] Empty = new Vector2[] { };
        private static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        private static readonly int[] DefaultDimensions = new int[] { 0, 0, 0, 0 }; // zero means repeating, so this is a regular one row array

        private int ElementCount;
        private readonly List<Vector2> Elements;
        public int[] Dimensions;

        private readonly Vector2[] Start;
        private readonly Vector2[] End;

        public EasingType[] EasingTypes;
        public bool IsDiscrete;
        public bool IsRepeating; // start>end interpolation applies each 'StopCount' elements

        public ValueKey(Vector2[] start, Vector2[] end = null, EasingType[] easingTypes = null,
            int[] dimensions = null, bool isDiscrete = false, bool isRepeating = false)
        {
            ElementCount = start.Length; // can be larger or smaller based on sampling
            Elements = new List<Vector2>(ElementCount);
            Start = start;
            End = (end == null) ? Empty : end;
            EasingTypes = (easingTypes == null) ? DefaultEasing : easingTypes;
            Dimensions = (dimensions == null) ? DefaultDimensions : dimensions;
            IsDiscrete = isDiscrete;
            IsRepeating = isRepeating;
        }

        public void SetT(float t, Values outValues)
        {
            float _t;
            float cols = Dimensions[0] > 0 ? Dimensions[0] : ElementCount;
            _t = IsDiscrete ? (int)(t * cols) / (float)cols : t; // is discrete step in T? Or in easing?

            // tx = Easing.GetValueAt(t, EasingTypes[0]);
            // ty = Easing.GetValueAt(t, EasingTypes[1]); // if exists, else [0]
            // x will be (tx * ElementCount) - cols
            // y will be (int)((ty * ElementCount) / cols)  /  Math.Ceiling(ElementCount/cols)

            _t = Easing.GetValueAt(t, EasingTypes[0]);
            outValues.ApplyValues(_t, this);// d_start + d_start * _t + (1.0f - _t) * d_end;
        }

        public void ApplyValues(float t)
        {
            float ct = 0;
            float divlen = ElementCount - 1;
            float cols = Dimensions[0] > 0 ? Dimensions[0] : ElementCount;
            float wrap = divlen / (cols - 1f);
            float wrappingT = (t * wrap) % 1; // used in grid drawing
            Vector2 start;
            Vector2 end;
            for (int i = 0; i < ElementCount; i++)
            {
                ct = i / divlen;
                //start = valueKey.Start.GetVector2At(ct, valueKey.IsDiscrete);
                //end = valueKey.End.GetVector2At(ct, valueKey.IsDiscrete);
                //values[i] = Vector2.Lerp(start, end, wrappingT);// start + start * t + (1.0f - t) * end;
            }
        }
        public abstract Vector2 GetVector2At(float t, bool interpolate);

    }
}
