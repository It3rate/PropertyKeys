using System;
using DataArcs.SeriesData;

namespace DataArcs.Adapters.Geometry
{
	public static class RectAdapter
    {
	    public static float Width(this Series series) => series.FloatDataAt(2) - series.FloatDataAt(0);
	    public static float Height(this Series series) => series.FloatDataAt(3) - series.FloatDataAt(Math.Min(1, series.DataSize));
	    public static float CenterX(this Series series) => series.Width() / 2f + series.X();
	    public static float CenterY(this Series series) => series.Height() / 2f + series.Y();

	    public static float Top(this Series series) => series.FloatDataAt(Math.Min(1, series.DataSize));
        public static float Left(this Series series) => series.FloatDataAt(0);
	    public static float Bottom(this Series series) => series.FloatDataAt(Math.Min(2, series.DataSize));
	    public static float Right(this Series series) => series.FloatDataAt(Math.Min(3, series.DataSize));
	    
        public static Series Center(this Series series) => new FloatSeries(2, series.CenterX(), series.CenterY());
    }
}
