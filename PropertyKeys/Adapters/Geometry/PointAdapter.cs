using System;
using DataArcs.SeriesData;

namespace DataArcs.Adapters.Geometry
{
	public static class PointAdapter
    {
	    public static float X(this Series series) => series.FloatDataAt(0);

	    public static float Y(this Series series) => series.FloatDataAt(Math.Min(1, series.DataSize));
    }
}
