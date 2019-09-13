using DataArcs.SeriesData;
using System;

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
		EaseInOutQuart,
    }

	public class Easing : Sampler
	{
		public EasingType EasingType;

		public Easing(EasingType easingType = EasingType.Linear)
		{
			EasingType = easingType;
		}

		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			// todo: check if this virtualCount assignment should happen in easing at this point or pass through.
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			var indexT = index / (float) virtualCount;
			return GetValueAtT(series, indexT, virtualCount);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
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
			var result = t;
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
					result = 1f - t * t;
					break;
				case EasingType.EaseInOut:
					result = t * t *  (3f - 2f * t);
					break;
				case EasingType.EaseCenter:
					var a = (t - 0.5f) * 2;
					var sgn = a >= 0 ? 0.5f : -0.5f;
					result = a * a * sgn + 0.5f;
					break;
				case EasingType.EaseInOutQuart:
                    result = t < 0.5f ?
						(float)(8.0 * t * t * t * t) :
						(float)(1.0 - Math.Pow(-2.0 * t + 2.0, 4) / 2.0);
					break;
            }

			return result;
		}
	}
}