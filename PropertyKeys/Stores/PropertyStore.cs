using System;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public enum CombineFunction
    {
        Replace,
        Append,
        Add,
        Subtract,
        Multiply,
        Divide,
        Average,
        Interpolate,
    }
    public class PropertyStore
    {
        public readonly Store[] Stores;
        public EasingType EasingType;

        public CombineFunction CombineFunction { get; } = CombineFunction.Replace;
        public PropertyStore Parameter0 { get; }
        public float Parameter1 { get; } = 0;
        public int Parameter2 { get; } = 0;

        public float CurrentT { get; set; } = 0;

        public PropertyStore(Store[] stores, EasingType easingType = EasingType.Linear)
        {
            Stores = stores;
            EasingType = easingType;
        }
        
        // todo: return series
        public float[] GetValuesAtIndex(int index, float t)
        {
            float[] result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].GetValueAtIndex(index).FloatValuesCopy;
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

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].GetValueAtT(vT).FloatValuesCopy;
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

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].VirtualCount;
            }
            else
            {
                int sec = Stores[startIndex].VirtualCount;
                int eec = Stores[startIndex + 1].VirtualCount;
                result = sec + (int)(vT * (eec - sec));
            }
            return result;
        }




        public static float[] BlendValueAtIndex(Store start, Store end, int index, float t)
        {
            float[] result = start.GetValueAtIndex(index).FloatValuesCopy;
            if (end != null)
            {
                float[] endAr = end.GetValueAtIndex(index).FloatValuesCopy;
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
        public static float[] BlendValueAtT(Store start, Store end, float indexT, float t)
        {
            float[] result = start.GetValueAtT(indexT).FloatValuesCopy;
            if (end != null)
            {
                float[] endAr = end.GetValueAtT(indexT).FloatValuesCopy;
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
    }
}
