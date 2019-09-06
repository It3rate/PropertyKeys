using DataArcs.Stores;

namespace DataArcs.Samplers
{
    public enum EasingType
    {
        None = 0,
        Linear,
        Squared,
        InverseSquared,
        EaseInOut,
        EaseCenter,
    }
    public class Easing : Sampler
    {
        public EasingType EasingType;

        public Easing(EasingType easingType = EasingType.Linear)
        {
            EasingType = easingType;
        }

        public override Series GetValueAtIndex(Series series, int index)
        {
            float indexT = index / (float)series.VirtualCount;
            return GetValueAtT(series, indexT);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            t = GetValueAt(t, EasingType);
            return series.GetValueAtT(t);
        }

        public override float GetTAtT(float t)
        {
            return GetValueAt(t, EasingType);
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
                case EasingType.EaseInOut:
                    //result = (t < 0.5f) ? 2f * (t * t) : 2f * t * (1f - t) + 0.5f;
                    result = (t * t) * (3f - 2f * t);
                    break;
                case EasingType.EaseCenter:
                    float a = (t - 0.5f) * 2;
                    float sgn = a >= 0 ? 0.5f : -0.5f;
                    result = (a * a) * sgn + 0.5f;
                    break;
            }
            return result;
        }
    }
}
