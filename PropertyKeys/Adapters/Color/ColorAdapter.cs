using DataArcs.SeriesData;

namespace DataArcs.Adapters.Color
{
    public static class ColorAdapter
    {
        public static float Red(this Series series)
        {
            return series.FloatDataAt(0);
        }
        public static float Green(this Series series)
        {
            return series.FloatDataAt(1);
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
            float r = a.Red();
            float g = a.Green();
            float b = a.Blue();
            switch (a.VirtualCount * a.VectorSize)
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
