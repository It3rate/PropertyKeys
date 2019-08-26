using System;
using System.Collections.Generic;
using System.Text;

namespace DataArcs
{
    public enum EasingType
    {
        None = 0,
        Linear = 1,
        Squared = 2,
        InverseSquared = 3,
    }
    public struct Easing
    {
        public EasingType EasingType;

        public Easing(EasingType easingType = EasingType.Linear)
        {
            EasingType = easingType;
        }

        public static float GetValueAt(float t, EasingType easingType)
        {
            float result = t;
            switch (easingType)
            {
                case EasingType.None:
                    result = 0;
                    break;
                case EasingType.Linear:
                    result = t;
                    break;
                case EasingType.Squared:
                    result = t * t;
                    break;
                case EasingType.InverseSquared:
                    result = 1f - (t * t);
                    break;
            }
            return result;
        }

        public float GetValueAt(float t)
        {
            return Easing.GetValueAt(t, EasingType);
        }
    }
}
