using DataArcs.SeriesData;
using System;

namespace DataArcs.Adapters.Color
{
    public static class ColorAdapter
    {
        public static float Red(this Series series)
        {
            return series.X;
        }
        public static float Green(this Series series)
        {
            return series.Y;
        }
        public static float Blue(this Series series)
        {
            return series.FloatDataAt(2);
        }
        public static float Alpha(this Series series)
        {
            return series.FloatDataAt(3);
        }

        public static System.Drawing.Color RGB(this Series a)
	    {
		    System.Drawing.Color result;
            float r = Math.Max(0, Math.Min(1, a.Red()));
            float g = Math.Max(0, Math.Min(1, a.Green()));
            float b = Math.Max(0, Math.Min(1, a.Blue()));
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
                    float al = a.Alpha();
                    result = System.Drawing.Color.FromArgb((int)(al * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));
				    break;
		    }

		    return result;
	    }

    }
}
