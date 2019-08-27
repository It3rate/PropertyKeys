﻿using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public enum BezierMove : int
    {
        MoveTo,
        LineTo,
        QuadTo,
        CubeTo,
        End,
    }
    public class BezierStore : FloatStore
    {
        private static readonly int[] moveSize = new int[]{2,2,4,6,0};

        public BezierMove[] Moves { get; }
        public GraphicsPath Path { get; private set; }

        public BezierStore(float[] values, BezierMove[] moves) : base(0, values)
        {
            // default to polyline if moves is empty
            if(moves == null)
            {
                int len = values.Length / 2;
                moves = new BezierMove[len];
                for (int i = 0; i < len; i++)
                {
                    moves[i] = BezierMove.LineTo;
                }
            }
            Moves = moves;
            GeneratePath();
        }

        public override float[] GetFloatArrayAtT(float t)
        {
            DataUtils.GetScaledT(t, Moves.Length, out float vT, out int startIndex, out int endIndex);
            float[] a = GetFloatArrayAtIndex(startIndex);
            float[] b = GetFloatArrayAtIndex(endIndex);
            int p0Index = startIndex == endIndex ? 0 : a.Length - 2; // start from last point unless at start or end.
            int p2Index = b.Length - 2;
            BezierMove moveType = Moves[startIndex];
            float[] result = { 0, 0 };
            float it = 1f - t;
            switch (moveType)
            {
                case BezierMove.MoveTo:
                case BezierMove.LineTo:
                    result[0] = a[p0Index] + (b[p2Index] - a[p0Index]) * t;
                    result[1] = a[p0Index + 1] + (b[p2Index + 1] - a[p0Index + 1]) * t;
                    break;
                case BezierMove.QuadTo:
                    result[0] = it * it * a[p0Index] + 2 * it * t * b[0] + t * t * b[p2Index];
                    result[1] = it * it * a[p0Index + 1] + 2 * it * t * b[1] + t * t * b[p2Index + 1];
                    break;
                case BezierMove.CubeTo:
                    // todo: cubic bezier calc
                    break;
                case BezierMove.End:
                default:
                    result = b;
                    break;
            }
            return result;
        }

        public override float[] GetFloatArrayAtIndex(int index)
        {
            index = Math.Max(0, Math.Min(Moves.Length - 1, index));
            int start = 0;
            for (int i = 0; i < index; i++)
            {
                start += moveSize[(int)Moves[i]];
            }
            int size = moveSize[(int)Moves[index]];
            float[] result = new float[size];
            Array.Copy(Values, start, result, 0, size);
            return result;
        }

        private void GeneratePath()
        {
            Path = new GraphicsPath();
            Path.FillMode = FillMode.Alternate;
            float posX = 0;
            float posY = 0;
            int index = 0;
            for (int i = 0; i < Moves.Length; i++)
            {
                BezierMove moveType = Moves[i];
                switch (moveType)
                {
                    case BezierMove.MoveTo:
                        posX = Values[index];
                        posY = Values[index + 1];
                        break;
                    case BezierMove.LineTo:
                        Path.AddLine(posX, posY, Values[index], Values[index + 1]);
                        posX = Values[index];
                        posY = Values[index + 1];
                        break;
                    case BezierMove.QuadTo:
                        // must convert to cubic for gdi
                        float cx = Values[index];
                        float cy = Values[index + 1];
                        float a1X = Values[index + 2];
                        float a1Y = Values[index + 3];
                        float c1x = (cx - posX) * 2 / 3 + posX;
                        float c1y = (cy - posY) * 2 / 3 + posY;
                        float c2x = a1X - (a1X - cx) * 2 / 3;
                        float c2y = a1Y - (a1Y - cy) * 2 / 3;
                        Path.AddBezier(posX, posY, c1x, c1y, c2x, c2y, a1X, a1Y);
                        posX = a1X;
                        posY = a1Y;
                        break;
                    case BezierMove.CubeTo:
                        Path.AddBezier(posX, posY, Values[index], Values[index + 1], Values[index + 2], Values[index + 3], Values[index+4], Values[index+5]);
                        posX = Values[index + 4];
                        posY = Values[index + 5];
                        break;
                    case BezierMove.End:
                    default:
                        Path.CloseFigure();
                        break;
                }
                index += moveSize[(int)moveType];
            }
        }
    }
}