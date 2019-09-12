using DataArcs.Samplers;
using DataArcs.Series;
using DataArcs.Stores;

namespace DataArcs.Commands
{
    public class InterpolationCommand : BaseCommand
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
                result = Stores[startIndex].GetValueAtIndex(index).FloatData;
            else
                result = BlendValueAtIndex(Stores[startIndex], Stores[endIndex], index, vT);
            return result;
        }

        public float[] GetValuesAtT(float indexT, float t)
        {
            float[] result;
            t = Easing.GetValueAt(t, EasingType);

            SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

            if (startIndex == endIndex)
                result = Stores[startIndex].GetValueAtT(vT).FloatData;
            else
                result = BlendValueAtT(Stores[startIndex], Stores[endIndex], indexT, vT);
            return result;
        }

        public int GetElementCountAt(float t)
        {
            int result;
            t = Easing.GetValueAt(t, EasingType);

            SeriesUtils.GetScaledT(t, Stores.Length, out var vT, out var startIndex, out var endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].VirtualCount;
            }
            else
            {
                var sec = Stores[startIndex].VirtualCount;
                var eec = Stores[startIndex + 1].VirtualCount;
                result = sec + (int) (vT * (eec - sec));
            }

            return result;
        }

        public static float[] BlendValueAtIndex(Store start, Store end, int index, float t)
        {
            var result = start.GetValueAtIndex(index).FloatData;
            if (end != null)
            {
                var endAr = end.GetValueAtIndex(index).FloatData;
                SeriesUtils.InterpolateInto(result, endAr, t);
            }

            return result;
        }

        public static float[] BlendValueAtT(Store start, Store end, float indexT, float t)
        {
            var result = start.GetValueAtT(indexT).FloatData;
            if (end != null)
            {
                var endAr = end.GetValueAtT(indexT).FloatData;
                SeriesUtils.InterpolateInto(result, endAr, t);
            }

            return result;
        }
    }
}