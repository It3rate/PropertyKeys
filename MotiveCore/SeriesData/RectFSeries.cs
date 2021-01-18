using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData.Utils;

namespace Motive.SeriesData
{
    public class RectFSeries : FloatSeries
    {
	    public override SeriesType Type => SeriesType.RectF;
	    private const int RectSize = 4;

		/// <summary>
        /// Create a rectF with l,t,r,b.
        /// </summary>
        /// <param name="values"></param>
	    public RectFSeries(params float[] values) : base(2, values)
	    {
		    if (_floatValues.Length > RectSize)
		    {
			    var old = _floatValues;
			    _floatValues = new float[RectSize];
			    Array.Copy(old, _floatValues, RectSize);
		    }
			else if (_floatValues.Length < RectSize)
		    {
			    var old = _floatValues;
			    _floatValues = new float[RectSize];
			    for (int i = 0; i < RectSize; i++)
			    {
				    _floatValues[i] = i < RectSize ? old[i] : old[old.Length - 1];
                }
            }
	    }
	    public float this[int index]
	    {
		    get => index < RectSize ? _floatValues[index] : _floatValues[RectSize - 1];
		    set => _floatValues[index < RectSize ? index : RectSize - 1] = value;
	    }
	    public new float X => _floatValues[0];
        public new float Y => _floatValues[1];
	    public float Right =>  _floatValues[2];
	    public float Bottom => _floatValues[3];
	    public float Width => _floatValues[2] - _floatValues[0];
        public float Height =>_floatValues[3] - _floatValues[1];
	    public float Top => _floatValues[1];
	    public float Left => _floatValues[0];
	    public float Area => Width * Height;

	    public float CX => X + Width / 2f;
	    public float CY => Y + Height / 2f;

        public ISeries LocationF => new FloatSeries(2, X, Y);
        public ISeries SizeF => new FloatSeries(2, Width, Height);
        public ISeries CenterF => new FloatSeries(2, CX, CY);

        public ISeries LeftTop => new FloatSeries(2, Left, Top);
	    public ISeries RightTop => new FloatSeries(2, Right, Top);
	    public ISeries LeftBottom => new FloatSeries(2, Left, Bottom);
	    public ISeries RightBottom => new FloatSeries(2, Right, Bottom);

	    public RectFSeries Outset(float outset)
	    {
		    return new RectFSeries(X - outset, Y - outset, Right + outset * 2f, Bottom + outset * 2f);
	    }
	    public RectFSeries Outset(float outsetX, float outsetY)
	    {
		    return new RectFSeries(X - outsetX, Y - outsetY, Right + outsetX * 2f, Bottom + outsetY * 2f);
	    }

	    public override ISeries Copy()
	    {
		    return new RectFSeries(_floatValues);
	    }
    }
}
