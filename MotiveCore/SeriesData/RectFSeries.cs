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

		/// <summary>
        /// Create a rectF with l,t,r,b.
        /// </summary>
        /// <param name="values"></param>
	    public RectFSeries(params float[] values) : base(2, values)
	    {
            EnsureCount(2, true);
        }
        public float Left
        {
	        get => X;
	        set => X = value;
        }
        public float Top
        {
	        get => Y;
	        set => Y = value;
        }
        public float Right
        {
	        get => Z;
	        set => Z = value;
        }
        public float Bottom
        {
	        get => W;
	        set => W = value;
        }
	    public float Width => Right - Left;
        public float Height => Bottom - Top;
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
