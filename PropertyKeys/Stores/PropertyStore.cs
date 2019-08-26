using System;

namespace DataArcs.Stores
{
    public class PropertyStore
    {
        public readonly FloatStore[] ValueStores;
        public EasingType EasingType;

        public PropertyStore(FloatStore[] valueStores, EasingType easingType = EasingType.Linear)
        {
            ValueStores = valueStores;
            EasingType = easingType;
        }

        public float[] GetValuesAtIndex(int index, float t)
        {
            float[] result;

            GetScaledT(t, out float vT, out int startIndex, out int endIndex);

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

        public float[] GetValuesAtT(float index_t, float t)
        {
            float[] result;

            GetScaledT(t, out float vT, out int startIndex, out int endIndex);

            if (startIndex == endIndex)
            {
                result = ValueStores[startIndex].GetFloatArrayAtT(vT);
            }
            else
            {
                result = BlendValueAtT(ValueStores[startIndex], ValueStores[endIndex], index_t, vT);
            }
            return result;
        }

        public int GetElementCountAt(float t)
        {
            int result;
            GetScaledT(t, out float vT, out int startIndex, out int endIndex);

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

        public void GetScaledT(float t, out float virtualT, out int startIndex, out int endIndex)
        {
            if (t >= 1)
            {
                startIndex = ValueStores.Length - 1;
                endIndex = startIndex;
                virtualT = 1f;
            }
            else if (t <= 0)
            {
                startIndex = ValueStores.Length - 0;
                endIndex = startIndex;
                virtualT = 0f;
            }
            else
            {
                float vt = t * (ValueStores.Length - 1f);
                startIndex = (int)vt;
                endIndex = startIndex + 1;
                virtualT = vt - startIndex;
            }
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
