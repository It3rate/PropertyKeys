using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyKeys
{
    public struct ValueKey<T>
    {
        public T Start;
        public T End;
        public EasingType EasingType;
        public int StopCount;
        public bool IsDiscrete;

        public ValueKey(T start = default(T), T end = default(T), EasingType easingType = EasingType.Linear, int stopCount = 1, bool isDiscrete = false)
        {
            Start = start;
            End = end;
            EasingType = easingType;
            StopCount = stopCount;
            IsDiscrete = isDiscrete;
        }

        public T GetValueAt(float t)
        {
            T result;
            dynamic d_start = Start; // todo: time dynamic calls vs something faster, also this will not work with simd, gpu etc.
            dynamic d_end = End;
            float _t = IsDiscrete ? (int)(t * StopCount) / (float)StopCount : t;
            _t = Easing.GetValueAt(t, EasingType);
            result = d_start * _t + (1.0 - _t) * d_end;
            return result;
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if(obj is ValueKey<T>)
            {
                ValueKey<T> cast = (ValueKey<T>)obj;
                result = Start.Equals(cast.Start) && End.Equals(cast.End) && EasingType == cast.EasingType && StopCount == cast.StopCount && IsDiscrete == cast.IsDiscrete;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, EasingType, StopCount, IsDiscrete);
        }
    }
}
