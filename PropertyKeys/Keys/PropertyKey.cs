using System;

namespace PropertyKeys
{
    public class PropertyKey
    {
        public int[] targetIDs;
        private readonly ValueKey Start;
        private readonly ValueKey End;
        public Action<Values> property;

        public EasingType EasingType;
        public bool IsRepeating = false; // stop count repeats every n items
        public float t = 0f;

        public PropertyKey(ValueKey start, ValueKey end, int[] targetIDs = null, EasingType easingType = EasingType.Linear)
        {
            Start = start;
            End = end;
            this.targetIDs = targetIDs;
            EasingType = easingType;
        }

        public float[] GetValuesAtIndex(int index, float t)
        {
            return Start.BlendValueAtIndex(End, index, t);
        }
        public int GetElementCountAt(float t)
        {
            return End == null ? Start.ElementCount : (int)(Start.ElementCount + t * (End.ElementCount - Start.ElementCount));
        }
    }
}
