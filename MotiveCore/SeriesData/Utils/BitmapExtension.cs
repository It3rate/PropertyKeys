using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.Components.ExternalInput;

namespace MotiveCore.SeriesData.Utils
{
   public static class BitmapExtension
    {
	    public static FloatSeries ToFloatSeries(this Bitmap bitmap, int targetWidth = -1, int targetHeight = -1)
	    {
		    int w = targetWidth == -1 ? bitmap.Width : targetWidth;
		    int h = targetHeight == -1 ? bitmap.Height : targetHeight;
		    DirectBitmap dBmp = new DirectBitmap(bitmap, w, h);
		    return dBmp.ToFloatSeries();
	    }
	    public static FloatSeries ToFloatSeriesHex(this Bitmap bitmap, int targetWidth = -1, int targetHeight = -1)
	    {
		    int w = targetWidth == -1 ? bitmap.Width : targetWidth;
		    int h = targetHeight == -1 ? bitmap.Height : targetHeight;
		    DirectBitmap dBmp = new DirectBitmap(bitmap, w, h);
		    return dBmp.ToFloatSeriesHex();
	    }
        public static IntSeries ToIntSeries(this Bitmap bitmap, int targetWidth = -1, int targetHeight = -1)
	    {
		    int w = targetWidth == -1 ? bitmap.Width : targetWidth;
		    int h = targetHeight == -1 ? bitmap.Height : targetHeight;
		    DirectBitmap dBmp = new DirectBitmap(bitmap, w, h);
		    return dBmp.ToIntSeries();
	    }
    }
}
