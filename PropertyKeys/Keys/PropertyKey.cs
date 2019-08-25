using System;

namespace PropertyKeys.Keys
{
    public class PropertyKey
    {
        public int[] targetIDs;
        public readonly IValueStore[] ValueKeys;
        public Action<Values> property;

        public EasingType EasingType;
        public bool IsRepeating = false; // stop count repeats every n items
        public float t = 0f;

        public PropertyKey(IValueStore[] valueKeys, int[] targetIDs = null, EasingType easingType = EasingType.Linear)
        {
            ValueKeys = valueKeys;
            this.targetIDs = targetIDs;
            EasingType = easingType;
        }

        //public float[] GetValuesAtT(float indexT, float t)
        //{
        //    float[] result;

        //    int startIndex, endIndex;
        //    float vT;
        //    GetScaledT(t, out vT, out startIndex, out endIndex);

        //    if (startIndex == endIndex)
        //    {
        //        result = ValueKeys[startIndex].GetFloatArrayAtIndex(index);
        //    }
        //    else
        //    {
        //        result = ValueKeys[startIndex].BlendValueAtIndex(ValueKeys[endIndex], index, vT);
        //    }
        //    return result;
        //}
        public float[] GetValuesAtIndex(int index, float t)
        {
            float[] result;

            int startIndex, endIndex;
            float vT;
            GetScaledT(t, out vT, out startIndex, out endIndex);

            if (startIndex == endIndex)
            {
                result = ValueKeys[startIndex].GetFloatArrayAtIndex(index);
            }
            else
            {
                result = ValueKeys[startIndex].BlendValueAtIndex(ValueKeys[endIndex], index, vT);
            }
            return result;
        }
        public int GetElementCountAt(float t)
        {
            int result;
            int startIndex, endIndex;
            float vT;
            GetScaledT(t, out vT, out startIndex, out endIndex);

            if (startIndex == endIndex)
            {
                result = ValueKeys[startIndex].ElementCount;
            }
            else
            {
                int sec = ValueKeys[startIndex].ElementCount;
                int eec = ValueKeys[startIndex + 1].ElementCount;
                result = sec + (int)(vT * (eec - sec));
            }
            return result;
        }

        public void GetScaledT(float t, out float virtualT, out int startIndex, out int endIndex)
        {
            if (t >= 1)
            {
                startIndex = ValueKeys.Length - 1;
                endIndex = startIndex;
                virtualT = 1f;
            }
            else if (t <= 0)
            {
                startIndex = ValueKeys.Length - 0;
                endIndex = startIndex;
                virtualT = 0f;
            }
            else
            {
                float vt = t * (ValueKeys.Length - 1f);
                startIndex = (int)vt;
                endIndex = startIndex + 1;
                virtualT = vt - startIndex;
            }
        }

    }
}
