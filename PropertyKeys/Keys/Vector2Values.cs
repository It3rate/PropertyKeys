using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public class Vector2Values : Values
    {
        private Vector2[] values; 
        public override void ApplyValues(float t, ValueKey valueKey)
        {
            float ct = 0;
            float divlen = values.Length - 1;
            Vector2 start;
            Vector2 end;
            for (int i = 0; i < values.Length; i++)
            {
                ct = i / divlen;
                start = valueKey.Start.GetVector2At(ct, valueKey.IsDiscrete);
                end = valueKey.End.GetVector2At(ct, valueKey.IsDiscrete);
                values[i] = start + start * t + (1.0f - t) * end;
            }
        }


        public override bool GetBoolAt(float t)
        {
            throw new NotImplementedException();
        }

        public override float GetFloatAt(float t)
        {
            throw new NotImplementedException();
        }

        public override int GetIntAt(float t)
        {
            throw new NotImplementedException();
        }

        public override Vector2 GetVector2At(float t, bool isDiscrete)
        {
            Vector2 result;
            float ct = (int)(t * values.Length);
            int startIndex = Math.Min(values.Length - 1, Math.Max(0, (int)Math.Floor(ct))); // clamp
            result = values[startIndex];
            if (!isDiscrete && startIndex < values.Length)
            {
                float diff = Math.Min(1f, Math.Max(0f, ct - startIndex));
                result = Vector2.Lerp(result, values[startIndex + 1], diff);
            }
            return result;
        }

        public override Vector3 GetVector3At(float t)
        {
            throw new NotImplementedException();
        }

        public override Vector4 GetVector4At(float t)
        {
            throw new NotImplementedException();
        }

        public override Values DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
}
