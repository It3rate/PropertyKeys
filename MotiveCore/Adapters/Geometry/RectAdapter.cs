using System;
using Motive.SeriesData;

namespace Motive.Adapters.Geometry
{
	public static class RectAdapter
    {
	    public static float Width(this ISeries series) => series.Bottom() - series.Left();
	    public static float Height(this ISeries series) => series.Right() - series.Top();
	    public static float CenterX(this ISeries series) => series.Width() / 2f + series.X;
	    public static float CenterY(this ISeries series) => series.Height() / 2f + series.Y;

	    public static float Top(this ISeries series) => series.FloatValueAt(Math.Min(1, series.DataSize - 1));
        public static float Left(this ISeries series) => series.X;
	    public static float Bottom(this ISeries series)
	    {
		    int index = series.DataSize == 2 ? 1 : 3; // allow for easy [0,1] rect
		    return series.FloatValueAt(Math.Min(index, series.DataSize - 1));
	    }

	    public static float Right(this ISeries series)
	    {
		    int index = series.DataSize == 2 ? 0 : 2; // allow for easy [0,1] rect
            return series.FloatValueAt(Math.Min(index, series.DataSize - 1));
	    }

	    public static ISeries Center(this ISeries series) => new FloatSeries(2, series.CenterX(), series.CenterY());
    }
}
