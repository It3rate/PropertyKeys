using System;

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
        public readonly FloatStore[] ValueStores;
        public EasingType EasingType;

        public CombineFunction CombineFunction { get; } = CombineFunction.Replace;
        public PropertyStore Parameter0 { get; }
        public float Parameter1 { get; } = 0;
        public int Parameter2 { get; } = 0;

        public float CurrentT { get; set; } = 0;

        public PropertyStore(FloatStore[] valueStores, EasingType easingType = EasingType.Linear)
        {
            ValueStores = valueStores;
            EasingType = easingType;
        }

        public float[] GetValuesAtIndex(int index, float t)
        {
            float[] result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, ValueStores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = ValueStores[startIndex].GetFloatArrayAtIndex(index);
            }
            else
            {
                result = BlendValueAtIndex(ValueStores[startIndex], ValueStores[endIndex], index, vT);
            }
            return result;
        }

        public float[] GetValuesAtT(float indexT, float t)
        {
            float[] result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, ValueStores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = ValueStores[startIndex].GetFloatArrayAtT(vT);
            }
            else
            {
                result = BlendValueAtT(ValueStores[startIndex], ValueStores[endIndex], indexT, vT);
            }
            return result;
        }

        public int GetElementCountAt(float t)
        {
            int result;
            t = Easing.GetValueAt(t, EasingType);

            DataUtils.GetScaledT(t, ValueStores.Length, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = ValueStores[startIndex].ElementCount;
            }
            else
            {
                int sec = ValueStores[startIndex].ElementCount;
                int eec = ValueStores[startIndex + 1].ElementCount;
                result = sec + (int)(vT * (eec - sec));
            }
            return result;
        }




        public static float[] BlendValueAtIndex(FloatStore start, FloatStore end, int index, float t)
        {
            float[] result = start.GetFloatArrayAtIndex(index);
            if (end != null)
            {
                float[] endAr = end.GetFloatArrayAtIndex(index);
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
        public static float[] BlendValueAtT(FloatStore start, FloatStore end, float index_t, float t)
        {
            float[] result = start.GetFloatArrayAtT(index_t);
            if (end != null)
            {
                float[] endAr = end.GetFloatArrayAtT(index_t);
                DataUtils.InterpolateInto(result, endAr, t);
            }
            return result;
        }
    }
}
