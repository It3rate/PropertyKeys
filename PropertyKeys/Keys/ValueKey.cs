using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PropertyKeys
{
    public abstract class ValueKey
    {
        public abstract int ElementCount { get; set;  }
        public abstract float[] GetFloatArrayAtIndex(int index, bool interpolate, float t);
        public abstract float GetFloatAtIndex(int index, bool interpolate, float t);
        public abstract Vector2 GetVector2AtIndex(int index, bool interpolate, float t);
        public abstract Vector3 GetVector3AtIndex(int index, bool interpolate, float t);
        public abstract Vector4 GetVector4AtIndex(int index, bool interpolate, float t);

        //public abstract float[] GetValueAtIndex(int index, bool interpolate, float t);
        //public abstract int GetElementCountAt(float t);


        public static Vector2 GetVector2(Vector3 a)
        {
            return new Vector2(a.X, a.Y);
        }

        public static float MergeToFloat(float a, float[] b)
        {
            return b.Length > 0 ? b[0] : a;
        }
        public static Vector2 MergeToVector2(Vector2 a, float[] b)
        {
            return new Vector2(b.Length > 0 ? b[0] : a.X, b.Length > 1 ? b[1] : a.Y);
        }
        public static Vector3 MergeToVector3(Vector3 a, float[] b)
        {
            return new Vector3(b.Length > 0 ? b[0] : a.X, b.Length > 1 ? b[1] : a.Y, b.Length > 2 ? b[2] : a.Z);
        }
        public static Vector4 MergeToVector4(Vector4 a, float[] b)
        {
            return new Vector4(b.Length > 0 ? b[0] : a.X, b.Length > 1 ? b[1] : a.Y, b.Length > 2 ? b[2] : a.Z, b.Length > 3 ? b[3] : a.W);
        }


        public static float MergeVectors(float a, float b)
        {
            return b;
        }
        public static float MergeVectors(float a, Vector2 b)
        {
            return b.X;
        }
        public static float MergeVectors(float a, Vector3 b)
        {
            return b.X;
        }
        public static float MergeVectors(float a, Vector4 b)
        {
            return b.X;
        }

        public static Vector2 MergeVectors(Vector2 a, float b)
        {
            return new Vector2(b, a.Y);
        }
        public static Vector2 MergeVectors(Vector2 a, Vector2 b)
        {
            return b;
        }
        public static Vector2 MergeVectors(Vector2 a, Vector3 b)
        {
            return new Vector2(b.X, b.Y);
        }
        public static Vector2 MergeVectors(Vector2 a, Vector4 b)
        {
            return new Vector2(b.X, b.Y);
        }

        public static Vector3 MergeVectors(Vector3 a, float b)
        {
            return new Vector3(b, a.Y, a.Z);
        }
        public static Vector3 MergeVectors(Vector3 a, Vector2 b)
        {
            return new Vector3(b, a.Z);
        }
        public static Vector3 MergeVectors(Vector3 a, Vector3 b)
        {
            return b;
        }
        public static Vector3 MergeVectors(Vector3 a, Vector4 b)
        {
            return new Vector3(b.X, b.Y, b.Z);
        }

        public static Vector4 MergeVectors(Vector4 a, float b)
        {
            return new Vector4(b, a.Y, a.Z, a.W);
        }
        public static Vector4 MergeVectors(Vector4 a, Vector2 b)
        {
            return new Vector4(b, a.Z, a.W);
        }
        public static Vector4 MergeVectors(Vector4 a, Vector3 b)
        {
            return new Vector4(b, a.W);
        }
        public static Vector4 MergeVectors(Vector4 a, Vector4 b)
        {
            return b;
        }
    }
}
