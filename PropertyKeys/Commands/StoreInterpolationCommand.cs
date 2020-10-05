using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Commands
{
	public class StoreInterpolationCommand : CommandBase
	{
		public readonly Store[] Stores;
		public readonly EasingType EasingType;

		public StoreInterpolationCommand(Store[] stores, EasingType easingType)
		{
			Stores = stores;
			EasingType = easingType;
		}
		
		public float[] GetValuesAtT(float indexT, float t)
		{
			float[] result;
			t = Easing.GetValueAt(new ParametricSeries(1, t), EasingType).X;

            SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = Stores[startIndex].GetValuesAtT(vT).FloatDataRef; // getValues is a new object, so ref ok
			}
			else
			{
				result = BlendValueAtT(Stores[startIndex], Stores[endIndex], indexT, vT);
			}

			return result;
		}

		public int GetElementCountAt(float t)
		{
			int result;
			t = Easing.GetValueAt(new ParametricSeries(1, t), EasingType).X;

            SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = Stores[startIndex].Capacity;
			}
			else
			{
				var sec = Stores[startIndex].Capacity;
				var eec = Stores[startIndex + 1].Capacity;
				result = sec + (int) (vT * (eec - sec));
			}

			return result;
		}
		public static float[] BlendValueAtT(Store start, Store end, float indexT, float t)
		{
			var result = start.GetValuesAtT(indexT).FloatDataRef;
			if (end != null)
			{
				var endAr = end.GetValuesAtT(indexT).FloatDataRef;
				ArrayExtension.InterpolateInto(result, endAr, t);
			}

			return result;
		}
	}
}