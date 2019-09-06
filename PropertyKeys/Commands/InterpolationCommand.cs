using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
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

            DataUtils.GetScaledT(t, Stores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = Stores[startIndex].GetValueAtIndex(index).Floats;
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
                result = Stores[startIndex].GetValueAtT(vT).Floats;
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
            float[] result = start.GetValueAtIndex(index).Floats;
            if (end != null)
            {
                float[] endAr = end.GetValueAtIndex(index).Floats;
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
        public static float[] BlendValueAtT(Store start, Store end, float indexT, float t)
        {
            float[] result = start.GetValueAtT(indexT).Floats;
            if (end != null)
            {
                float[] endAr = end.GetValueAtT(indexT).Floats;
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
    }
}
