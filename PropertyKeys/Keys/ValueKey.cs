using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyKeys
{
    public struct ValueKey
    {
        public Values Start;
        public Values End;
        public EasingType EasingType;
        public int StopCount;
        public bool IsDiscrete;

        public ValueKey(Values start = default(Values), Values end = default(Values), EasingType easingType = EasingType.Linear, int stopCount = 2, bool isDiscrete = false)
        {
            Start = start;
            End = end;
            EasingType = easingType;
            StopCount = stopCount;
            IsDiscrete = isDiscrete;
        }

        public void SetT(float t, Values outValues)
        {
            float _t;
            _t = IsDiscrete ? (int)(t * StopCount) / (float)StopCount : t; // is discrete step in T? Or in easing?
            _t = Easing.GetValueAt(t, EasingType);
            outValues.ApplyValues(_t, this);// d_start + d_start * _t + (1.0f - _t) * d_end;
        }


    }
}
