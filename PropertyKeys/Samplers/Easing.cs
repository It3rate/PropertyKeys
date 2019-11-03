using DataArcs.SeriesData;
using System;

namespace DataArcs.Samplers
{
    public enum EasingType
	{
		None = 0,
		Linear,
		SmoothStart2,
		SmoothStart3,
		SmoothStart4,
		SmoothStart5,
		SmoothStart6,
		SmoothStop2,
		SmoothStop3,
		SmoothStop4,
		SmoothStop5,
		SmoothStop6,
		SmoothStep2,
		SmoothStep3,
		SmoothStep4,
		SmoothStep5,
		SmoothStep6,

		Arch,
		BellCurve4,
        BellCurve6,
		InverseBellCurve4,
		InverseBellCurve6,
        InverseSquared,
		EaseInOut,
		EaseCenter,

		EaseInOut2,
		EaseInOut3,
		EaseInOut4,
		EaseInOut5,
		EaseInOut6,
		Sigmoid,

        EaseInOut3AndBack,
    }

	public class Easing : Sampler
	{
		public EasingType[] EasingTypes;
		private bool _clamp;

		public Easing(EasingType easingType = EasingType.Linear, Slot[] swizzleMap = null, int capacity = 1, bool clamp = false) : base(swizzleMap, capacity)
        {
			EasingTypes = new EasingType[] { easingType };
			_clamp = clamp;
        }
		public Easing(params EasingType[] easingTypes)
        {
			EasingTypes = easingTypes;
		}
        
		public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
		{
			var result = new ParametricSeries(EasingTypes.Length, new float[EasingTypes.Length]);
			for (int i = 0; i < EasingTypes.Length; i++)
			{
				result[i] = GetValueAt(seriesT, EasingTypes[i], _clamp)[i];
			}
			return Swizzle(result, seriesT);
        }

		public static float GetSingleValueAt(float t, EasingType easingType)
		{
			return GetValueAt(new ParametricSeries(1, t), easingType).X;
		}

		public static ParametricSeries GetValueAt(ParametricSeries seriesT, EasingType easingType, bool clamp = false)
		{
			// influence https://stackoverflow.com/questions/4900069/how-to-make-inline-functions-in-c-sharp
			ParametricSeries result2 = (ParametricSeries)seriesT.Copy();
			for (int i = 0; i < seriesT.VectorSize; i++)
			{
				float t = clamp ? Math.Max(0, Math.Min(1, seriesT[i])) : seriesT[i];
				var result = t;
				float it = 1f - t;
				switch (easingType)
				{
					case EasingType.None:
						result = 0;
						break;
					case EasingType.Linear:
						result = t;
						break;
					case EasingType.SmoothStart2:
						result = Pow(2, t);
						break;
					case EasingType.SmoothStart3:
						result = Pow(3, t);
						break;
					case EasingType.SmoothStart4:
						result = Pow(4, t);
						break;
					case EasingType.SmoothStart5:
						result = Pow(5, t);
						break;
					case EasingType.SmoothStart6:
						result = Pow(6, t);
						break;
					case EasingType.SmoothStop2:
						result = FlipPow(2, t);
						break;
					case EasingType.SmoothStop3:
						result = FlipPow(3, t);
						break;
					case EasingType.SmoothStop4:
						result = FlipPow(4, t);
						break;
					case EasingType.SmoothStop5:
						result = FlipPow(5, t);
						break;
					case EasingType.SmoothStop6:
						result = FlipPow(6, t);
						break;

					case EasingType.SmoothStep2:
						result = CrossFade(EasingType.SmoothStart2, EasingType.SmoothStop2, t);
						break;
					case EasingType.SmoothStep3:
						result = CrossFade(EasingType.SmoothStart3, EasingType.SmoothStop3, t);
						break;
					case EasingType.SmoothStep4:
						result = CrossFade(EasingType.SmoothStart4, EasingType.SmoothStop4, t);
						break;
					case EasingType.SmoothStep5:
						result = CrossFade(EasingType.SmoothStart5, EasingType.SmoothStop5, t);
						break;
					case EasingType.SmoothStep6:
						result = CrossFade(EasingType.SmoothStart6, EasingType.SmoothStop6, t);
						break;

					case EasingType.Arch:
						result = Scale(EasingType.Linear, t);
						break;
					case EasingType.BellCurve4:
						result = Mult(EasingType.SmoothStop2, EasingType.SmoothStart2, t);
						break;
					case EasingType.BellCurve6:
						result = Mult(EasingType.SmoothStop3, EasingType.SmoothStart3, t);
						break;
					case EasingType.InverseBellCurve4:
						result = 1f - Mult(EasingType.SmoothStop2, EasingType.SmoothStart2, t);
						break;
					case EasingType.InverseBellCurve6:
						result = 1f - Mult(EasingType.SmoothStop3, EasingType.SmoothStart3, t);
						break;


					case EasingType.InverseSquared:
						result = 1f - t * t;
						break;
					case EasingType.EaseInOut:
						result = t * t * (3f - 2f * t);
						break;
					case EasingType.EaseCenter:
						var a = (t - 0.5f) * 2;
						var sgn = a >= 0 ? 0.5f : -0.5f;
						result = a * a * sgn + 0.5f;
						break;

					case EasingType.EaseInOut2:
						result = InOut(2, t);
						break;
					case EasingType.EaseInOut3:
						result = InOut(3, t);
						break;
					case EasingType.EaseInOut4:
						result = InOut(4, t);
						break;
					case EasingType.EaseInOut5:
						result = InOut(5, t);
						break;
					case EasingType.EaseInOut6:
						result = InOut(6, t);
						break;

					case EasingType.Sigmoid:
						result = 1f / (1f + (float) Math.Exp(-t));
						break;

					case EasingType.EaseInOut3AndBack:
						if (t < 0.5f)
						{
							result = InOut(3, t * 2);
						}
						else
						{
							result = 1f - InOut(3, (t - 0.5f) * 2f);
						}

						break;
				}

				result2[i] = clamp ?  Math.Max(0, Math.Min(1, result)) : result;
			}

			return result2;
		}


		private static float Pow(int pow, float t)
		{
            float result = t;
			for (int i = 1; i < pow; i++)
			{
				result *= t;
			}
			return result;
		}
		private static float FlipPow(int pow, float t)
		{
			t = 1f - t;
			return 1f - Pow(pow, t);
		}

		private static float InOut(int pow, float t) => t < 0.5f ? Pow(pow - 1, 2) * Pow(pow, t) : 1f - Pow(pow, -2f * t + 2f) / 2f;
        private static float Mix(float a, float b, float weightB, float t) => t * (a + weightB * (b - a));// (1f - weightB) * a + weightB * b;
        private static float CrossFade(float a, float b, float t) => a + t * (b - a);
        private static float CrossFade(EasingType easingTypeA, EasingType easingTypeB, float t) => CrossFade( GetSingleValueAt(t, easingTypeA), GetSingleValueAt(t, easingTypeB), t);
        private static float Scale(EasingType easingType, float t) => t * GetSingleValueAt(t, easingType);
        private static float Scale(EasingType easingType, int pow, float t) => Scale(easingType, Pow(pow, t));
        private static float ReverseScale(EasingType easingType, float t) => (1f - t) * GetSingleValueAt(t, easingType);
        private static float ReverseScale(EasingType easingType, int pow, float t) => ReverseScale(easingType, Pow(pow, t));
        private static float Mult(EasingType easingTypeA, EasingType easingTypeB, float t) => GetSingleValueAt(t, easingTypeA) * GetSingleValueAt(t, easingTypeB);


        private static float FakePow(float a, float pow)
		{
			float result = a;
			pow -= 1f;
			while (pow > 1)
			{
				result *= a;
				pow -= 1f;
			}
            return result * pow + result * a * (1f - pow);
		}
	}
}