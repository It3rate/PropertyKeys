using DataArcs.SeriesData;

namespace DataArcs.Adapters.Color
{
    public static class ColorAdapter
    {
	    public static System.Drawing.Color RGB(this Series a)
	    {
		    System.Drawing.Color result;
		    switch (a.VirtualCount * a.VectorSize)
		    {
                case 1:
				    result = System.Drawing.Color.FromArgb(255, (int)(a[0] * 255), (int)(a[0] * 255), (int)(a[0] * 255));
				    break;
			    case 2:
				    result = System.Drawing.Color.FromArgb(255, (int)(a[0] * 255), (int)(a[1] * 255), 0);
				    break;
			    case 3:
				    result = System.Drawing.Color.FromArgb(255, (int)(a[0] * 255), (int)(a[1] * 255), (int)(a[2] * 255));
				    break;
                default:
				    result = System.Drawing.Color.FromArgb((int)(a[3] * 255), (int)(a[0] * 255), (int)(a[1] * 255),
					    (int)(a[2] * 255));
				    break;
		    }

		    return result;
	    }

    }
}
