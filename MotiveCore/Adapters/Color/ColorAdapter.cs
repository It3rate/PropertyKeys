using System;
using System.Linq;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Adapters.Color
{
	public static class Colors
	{
	    public static readonly FloatSeries Black = new FloatSeries(3, 0, 0, 0);
        public static readonly FloatSeries White = new FloatSeries(3, 1f, 1f, 1f);
        public static readonly FloatSeries LightGray = new FloatSeries(3, .75f, .75f, .75f);
        public static readonly FloatSeries MidGray = new FloatSeries(3, .5f, .5f, .5f);
        public static readonly FloatSeries DarkGray = new FloatSeries(3, .25f, .25f, .25f);
        public static readonly FloatSeries Red = new FloatSeries(3, 1f, 0f, 0f);
	    public static readonly FloatSeries Green = new FloatSeries(3, 0f, 1f, 0f);
	    public static readonly FloatSeries Blue = new FloatSeries(3, 0f, 0f, 1f);
	    public static readonly FloatSeries Yellow = new FloatSeries(3, 1f, 1f, 0f);
	    public static readonly FloatSeries Cyan = new FloatSeries(3, 0f, 1f, 1f);
        public static readonly FloatSeries Magenta = new FloatSeries(3, 1f, 0f, 1f);

        public static readonly FloatSeries Pink = new FloatSeries(3, 1f, .75f, .75f);
        public static readonly FloatSeries DarkBlue = new FloatSeries(3, 0f, 0f, .25f);
        public static readonly FloatSeries MidBlue = new FloatSeries(3, 0f, 0f, .5f);
        public static readonly FloatSeries DarkRed = new FloatSeries(3, .25f, 0f, 0f);
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

        public static ParametricSeries RgbToHsl(this Series series)
        {
	        float[] input = series.GetRawDataAt(0).FloatDataRef;
			float[] result = new float[3];
	        var max = input.Max();
	        var min = input.Min();
	        var diff = max - min;
	        result[2] = (max + min) / 2f; // lightness
	        if (Math.Abs(diff) < 0.00001)
	        {
		        result[1] = 0; // saturation
		        result[0] = 0; // hue
	        }
	        else
	        {
		        if (result[2] <= 0.5)
		        {
			        result[1] = diff / (max + min);
		        }
		        else
		        {
			        result[1] = diff / (2 - max - min);
		        }

		        var rDist = (max - input[0]) / diff;
		        var gDist = (max - input[1]) / diff;
		        var bDist = (max - input[2]) / diff;

		        if (input[0] == max)
		        {
			        result[0] = bDist - gDist;
		        }
		        else if (input[1] == max)
		        {
			        result[0] = 2 + rDist - bDist;
		        }
		        else
		        {
			        result[0] = 4 + gDist - rDist;
		        }

		        result[0] /= 6f;
		        if (result[0] < 0) result[0] += 1f;
	        }
            return new ParametricSeries(3, result);
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
	                float al = Math.Max(0, Math.Min(1, a.AlphaComponent()));
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
