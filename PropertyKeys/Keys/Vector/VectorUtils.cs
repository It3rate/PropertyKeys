﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public class VectorUtils
    {
        #region Static Helpers
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

        public static float[] MergeToFloatArray(float[] a, float[] b)
        {
            float[] result = (float[])a.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                if (i < result.Length)
                {
                    a[i] = b[i];
                }
                else
                {
                    break;
                }
            }
            return result;
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

        public static float[] GetFloatArray(float input)
        {
            return new float[] { input };
        }
        public static float[] GetFloatArray(Vector2 input)
        {
            float[] temp = new float[] { 0, 0 };
            input.CopyTo(temp);
            return temp;
        }
        public static float[] GetFloatArray(Vector3 input)
        {
            float[] temp = new float[] { 0, 0, 0 };
            input.CopyTo(temp);
            return temp;
        }
        public static float[] GetFloatArray(Vector4 input)
        {
            float[] temp = new float[] { 0, 0, 0, 0 };
            input.CopyTo(temp);
            return temp;
        }


        public static float GetFloat(float[] b)
        {
            return b.Length > 0 ? b[0] : 0;
        }
        public static Vector2 GetVector2(float[] b)
        {
            return new Vector2(b.Length > 0 ? b[0] : 0, b.Length > 1 ? b[1] : 0);
        }
        public static Vector3 GetVector3(float[] b)
        {
            return new Vector3(b.Length > 0 ? b[0] : 0, b.Length > 1 ? b[1] : 0, b.Length > 2 ? b[2] : 0);
        }
        public static Vector4 GetVector4(float[] b)
        {
            return new Vector4(b.Length > 0 ? b[0] : 0, b.Length > 1 ? b[1] : 0, b.Length > 2 ? b[2] : 0, b.Length > 3 ? b[3] : 0);
        }

        public static Color GetRGBColorFrom(float a)
        {
            return Color.FromArgb(255, (int)(a * 255), (int)(a * 255), (int)(a * 255));
        }
        public static Color GetRGBColorFrom(Vector2 a)
        {
            return Color.FromArgb(255, (int)(a.X * 255), (int)(a.Y * 255), 0);
        }
        public static Color GetRGBColorFrom(Vector3 a)
        {
            return Color.FromArgb(255, (int)(a.X * 255), (int)(a.Y * 255), (int)(a.Z * 255));
        }
        public static Color GetRGBColorFrom(Vector4 a)
        {
            return Color.FromArgb((int)(a.W * 255), (int)(a.X * 255), (int)(a.Y * 255), (int)(a.Z * 255));
        }
        #endregion
    }
}
