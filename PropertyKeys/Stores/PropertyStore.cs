using System;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public class PropertyStore
    {
        public readonly Store[] Stores; // todo: Move store sequence to animation?
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
        public Series GetValuesAtIndex(int index, float t, int virtualCount = -1)
        {
            Series result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].GetValueAtIndex(index, virtualCount);
            }
            else
            {
                result = BlendValueAtIndex(Stores[startIndex], Stores[endIndex], index, vT, virtualCount);
            }
            return result;
        }

        public Series GetValuesAtT(float indexT, float t, int virtualCount = -1)
        {
            Series result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].GetValueAtT(indexT, virtualCount);
            }
            else
            {
                result = BlendValueAtT(Stores[startIndex], Stores[endIndex], indexT, vT, virtualCount);
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

        public virtual void Reset()
        {
            foreach (var store in Stores)
            {
                store.Reset();
            }
        }

        public virtual void Update()
        {
            foreach (var store in Stores)
            {
                store.Update();
            }
        }

        public static Series BlendValueAtIndex(Store start, Store end, int index, float t, int virtualCount = -1)
        {
            Series result = start.GetValueAtIndex(index, virtualCount);
            if (end != null)
            {
                Series endAr = end.GetValueAtIndex(index, virtualCount);
                result.Interpolate(endAr, t);
            }
            return result;
        }
        public static Series BlendValueAtT(Store start, Store end, float indexT, float t, int virtualCount = -1)
        {
            Series result = start.GetValueAtT(indexT, virtualCount);
            if (end != null)
            {
                Series endAr = end.GetValueAtT(indexT, virtualCount);
                result.Interpolate(endAr, t);
            }
            return result;
        }
    }
}
