using System;
using MotiveCore.SeriesData;

namespace MotiveCore.Adapters.Geometry
{
	public static class RectAdapter
    {
	    public static float Width(this Series series) => series.Bottom() - series.Left();
	    public static float Height(this Series series) => series.Right() - series.Top();
	    public static float CenterX(this Series series) => series.Width() / 2f + series.X;
	    public static float CenterY(this Series series) => series.Height() / 2f + series.Y;

	    public static float Top(this Series series) => series.FloatDataAt(Math.Min(1, series.DataSize - 1));
        public static float Left(this Series series) => series.X;
	    public static float Bottom(this Series series)
	    {
		    int index = series.DataSize == 2 ? 1 : 3; // allow for easy [0,1] rect
		    return series.FloatDataAt(Math.Min(index, series.DataSize - 1));
	    }

	    public static float Right(this Series series)
	    {
		    int index = series.DataSize == 2 ? 0 : 2; // allow for easy [0,1] rect
            return series.FloatDataAt(Math.Min(index, series.DataSize - 1));
	    }

	    public static Series Center(this Series series) => new FloatSeries(2, series.CenterX(), series.CenterY());
    }
}
