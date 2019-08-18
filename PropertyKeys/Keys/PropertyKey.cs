using PropertyKeys.Keys;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PropertyKeys
{
    public class PropertyKey<T>
    {
        public int[] targetIDs;
        private readonly Vector3Key Start;
        private readonly Vector3Key End;
        public Action<Values> property;

        public EasingType EasingType;
        public bool IsRepeating = false; // stop count repeats every n items
        public float t = 0f;

        public PropertyKey(Vector3Key start, Vector3Key end, int[] targetIDs = null, EasingType easingType = EasingType.Linear)
        {
            Start = start;
            End = end;
            this.targetIDs = targetIDs;
            EasingType = easingType;
        }

        public Vector3 GetVector3AtIndex(int index, bool interpolate, float t)
        {
            Vector3 start = Start.GetVector3AtIndex(index, interpolate, 0, End);
            Vector3 end = End.GetVector3AtIndex(index, interpolate, 1, null);
            t = Easing.GetValueAt(t, EasingType.Squared);
            return Vector3.Lerp(start, end, t);
        }
        public int GetElementCountAt(float t)
        {
            return End == null ? Start.ElementCount : (int)(Start.ElementCount + t * (End.ElementCount - Start.ElementCount));
        }
    }
}
