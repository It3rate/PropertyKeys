using System.Drawing;

namespace DataArcs.Graphic
{
	public class GraphicUtils
	{
		public static Color GetRGBColorFrom(Series.Series a)
		{
			Color result;
			switch (a.VirtualCount * a.VectorSize)
			{
				case 1:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[0] * 255), (int) (a[0] * 255));
					break;
				case 2:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[1] * 255), 0);
					break;
				case 3:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[1] * 255), (int) (a[2] * 255));
					break;
				case 4:
					result = Color.FromArgb((int) (a[3] * 255), (int) (a[0] * 255), (int) (a[1] * 255),
						(int) (a[2] * 255));
					break;
				default:
					result = Color.Red;
					break;
			}

			return result;
		}

		public static Color GetRGBColorFrom(float[] a)
		{
			Color result;
			switch (a.Length)
			{
				case 1:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[0] * 255), (int) (a[0] * 255));
					break;
				case 2:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[1] * 255), 0);
					break;
				case 3:
					result = Color.FromArgb(255, (int) (a[0] * 255), (int) (a[1] * 255), (int) (a[2] * 255));
					break;
				case 4:
					result = Color.FromArgb((int) (a[3] * 255), (int) (a[0] * 255), (int) (a[1] * 255),
						(int) (a[2] * 255));
					break;
				default:
					result = Color.Red;
					break;
			}

			return result;
		}
	}
}