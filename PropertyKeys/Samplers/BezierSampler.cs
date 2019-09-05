using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
    public class BezierSampler : Sampler
    {
        public override Series GetValueAtIndex(Series series, int index)
        {
            float t = index / (float)series.VirtualCount;
            return GetValueAtT(series, t);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            BezierMove[] moves = series is BezierSeries bezierSeries ? bezierSeries.Moves : new[] { BezierMove.LineTo };
            return GetValueAtT(series, moves, t);
        }

        public static Series GetValueAtT(Series series, BezierMove[] moves, float t)
        {
            DataUtils.GetScaledT(t, series.VirtualCount, out float vT, out int startIndex, out int endIndex);
            float[] a = series.GetValueAtIndex(startIndex).FloatValuesCopy;// GetFloatArrayAtIndex(startIndex);
            float[] b = series.GetValueAtIndex(endIndex).FloatValuesCopy;// GetFloatArrayAtIndex(endIndex);
            int p0Index = startIndex == endIndex ? 0 : a.Length - 2; // start from last point unless at start or end.
            int p2Index = b.Length - 2;
            BezierMove moveType = startIndex < moves.Length ? moves[startIndex] : BezierMove.LineTo;
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
            return new FloatSeries(2, result);
        }
        
    }
}
