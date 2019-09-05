using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    // maybe store all values as quadratic, allowing easier blending?
    // try blending with len/angle from zero vs interpolate points.
    public enum BezierMove : int
    {
        MoveTo,
        LineTo,
        QuadTo,
        CubeTo,
        End,
    }

    /// <summary>
    /// Specialized series for 2D Beziers, holds the extra move data. Use a BezierSampler to render and interpolate.
    /// </summary>
    public class BezierSeries : FloatSeries
    {
        public static readonly int[] MoveSize = new int[] { 2, 2, 4, 6, 0 };

        public BezierMove[] Moves { get; }

        public BezierSeries(float[] values, BezierMove[] moves = null) : base(2, values, moves?.Length ?? values.Length / 2)
        {
            // default to polyline if moves is empty
            if (moves == null)
            {
                int len = values.Length / 2;
                moves = new BezierMove[len];
                for (int i = 0; i < len; i++)
                {
                    moves[i] = BezierMove.LineTo;
                }
            }
            Moves = moves;
        }

        public override Series GetValueAtT(float t)
        {
            int index = (int)(t * VirtualCount);
            return GetValueAtIndex(index);
        }

        public override Series GetValueAtIndex(int index)
        {
            index = Math.Max(0, Math.Min(Moves.Length - 1, index));
            int start = 0;
            for (int i = 0; i < index; i++)
            {
                start += MoveSize[(int)Moves[i]];
            }
            int size = MoveSize[(int)Moves[index]];
            float[] result = new float[size];
            Array.Copy(_floatValues, start, result, 0, size);
            return new FloatSeries(2, result);
        }


        public GraphicsPath Path()
        {
            var path = new GraphicsPath();
            path.FillMode = FillMode.Alternate;
            float posX = 0;
            float posY = 0;
            int index = 0;
            foreach (var moveType in Moves)
            {
                switch (moveType)
                {
                    case BezierMove.MoveTo:
                        posX = _floatValues[index];
                        posY = _floatValues[index + 1];
                        break;
                    case BezierMove.LineTo:
                        path.AddLine(posX, posY, _floatValues[index], _floatValues[index + 1]);
                        posX = _floatValues[index];
                        posY = _floatValues[index + 1];
                        break;
                    case BezierMove.QuadTo:
                        // must convert to cubic for gdi
                        float cx = _floatValues[index];
                        float cy = _floatValues[index + 1];
                        float a1x = _floatValues[index + 2];
                        float a1y = _floatValues[index + 3];
                        float c1x = (cx - posX) * 2 / 3 + posX;
                        float c1y = (cy - posY) * 2 / 3 + posY;
                        float c2x = a1x - (a1x - cx) * 2 / 3;
                        float c2y = a1y - (a1y - cy) * 2 / 3;
                        path.AddBezier(posX, posY, c1x, c1y, c2x, c2y, a1x, a1y);
                        posX = a1x;
                        posY = a1y;
                        break;
                    case BezierMove.CubeTo:
                        path.AddBezier(posX, posY,
                            _floatValues[index], _floatValues[index + 1],
                            _floatValues[index + 2], _floatValues[index + 3],
                            _floatValues[index + 4], _floatValues[index + 5]);
                        posX = _floatValues[index + 4];
                        posY = _floatValues[index + 5];
                        break;
                    case BezierMove.End:
                    default:
                        path.CloseFigure();
                        break;
                }
                index += MoveSize[(int)moveType];
            }

            return path;
        }
    }
}
