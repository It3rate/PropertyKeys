﻿using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Commands
{
	public class InterpolationCommand : CommandBase
	{
		public readonly Store[] Stores;
		public readonly EasingType EasingType;

		public InterpolationCommand(Store[] stores, EasingType easingType)
		{
			Stores = stores;
			EasingType = easingType;
		}


		// todo: return series
		public float[] GetValuesAtIndex(int index, float t)
		{
			float[] result;
			t = Easing.GetValueAt(t, EasingType);

			SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = Stores[startIndex].GetValuesAtIndex(index).FloatData;
			}
			else
			{
				result = BlendValueAtIndex(Stores[startIndex], Stores[endIndex], index, vT);
			}

			return result;
		}

		public float[] GetValuesAtT(float indexT, float t)
		{
			float[] result;
			t = Easing.GetValueAt(t, EasingType);

			SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = Stores[startIndex].GetValuesAtT(vT).FloatData;
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
			t = Easing.GetValueAt(t, EasingType);

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

		public static float[] BlendValueAtIndex(Store start, Store end, int index, float t)
		{
			var result = start.GetValuesAtIndex(index).FloatData;
			if (end != null)
			{
				var endAr = end.GetValuesAtIndex(index).FloatData;
				SeriesUtils.InterpolateInto(result, endAr, t);
			}

			return result;
		}

		public static float[] BlendValueAtT(Store start, Store end, float indexT, float t)
		{
			var result = start.GetValuesAtT(indexT).FloatData;
			if (end != null)
			{
				var endAr = end.GetValuesAtT(indexT).FloatData;
				SeriesUtils.InterpolateInto(result, endAr, t);
			}

			return result;
		}
	}
}