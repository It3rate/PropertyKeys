using DataArcs.SeriesData;
using System;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Adapters.Color
{
	public static class Colors
	{
	    public static readonly FloatSeries Black = new FloatSeries(3, 0, 0, 0);
	    public static readonly FloatSeries White = new FloatSeries(3, 1f, 1f, 1f);
	    public static readonly FloatSeries Red = new FloatSeries(3, 1f, 0f, 0f);
	    public static readonly FloatSeries Green = new FloatSeries(3, 0f, 1f, 0f);
	    public static readonly FloatSeries Blue = new FloatSeries(3, 0f, 0f, 1f);
	    public static readonly FloatSeries Yellow = new FloatSeries(3, 1f, 1f, 0f);
	    public static readonly FloatSeries Cyan = new FloatSeries(3, 0f, 1f, 1f);
        public static readonly FloatSeries Magenta = new FloatSeries(3, 1f, 0f, 1f);
	}

    public static class ColorAdapter
    {

        public static float RedComponent(this Series series)
        {
            return series.X;
        }
        public static float GreenComponent(this Series series)
        {
            return series.Y;
        }
        public static float BlueComponent(this Series series)
        {
            return series.FloatDataAt(2);
        }
        public static float AlphaComponent(this Series series)
        {
            return series.FloatDataAt(3);
        }

        public static System.Drawing.Color RGB(this Series a)
	    {
		    System.Drawing.Color result;
            float r = Math.Max(0, Math.Min(1, a.RedComponent()));
            float g = Math.Max(0, Math.Min(1, a.GreenComponent()));
            float b = Math.Max(0, Math.Min(1, a.BlueComponent()));
            switch (a.Count * a.VectorSize)
		    {
                case 1:
				    result = System.Drawing.Color.FromArgb(255, (int)(r * 255), (int)(r * 255), (int)(r * 255));
				    break;
			    case 2:
                    result = System.Drawing.Color.FromArgb(255, (int)(r * 255), (int)(g * 255), 0);
				    break;
			    case 3:
                    result = System.Drawing.Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
				    break;
                default:
                    float al = a.AlphaComponent();
                    result = System.Drawing.Color.FromArgb((int)(al * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));
				    break;
		    }

		    return result;
	    }

        public static Series RandomColor(float minR, float maxR, float minG, float maxG, float minB, float maxB)
        {
		        return new FloatSeries(3,
			        (float)SeriesUtils.Random.NextDouble() * maxR + minR,
			        (float)SeriesUtils.Random.NextDouble() * maxG + minG,
			        (float)SeriesUtils.Random.NextDouble() * maxB + minB);
        }

    }
}
