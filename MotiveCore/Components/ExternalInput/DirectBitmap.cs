using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Motive.SeriesData;
using Motive.Samplers;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace Motive.Components.ExternalInput
{
	public class DirectBitmap : IDisposable
	{
		public Bitmap Bitmap { get; private set; }
		public Int32[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		public DirectBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new Int32[width * height];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
		}

		public DirectBitmap(Bitmap sourceBitmap, int targetWidth, int targetHeight)
		{
			Width = targetWidth;
			Height = targetHeight;

			Bits = new Int32[targetWidth * targetHeight];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(targetWidth, targetHeight, targetWidth * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());

            var g = Graphics.FromImage(Bitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //float scale = Math.MinSlots(Width / sourceBitmap.Width, Height / sourceBitmap.Height);
            g.DrawImage(sourceBitmap, 0, 0, Width, Height);
		}

		public FloatSeries ToFloatSeries()
		{
			var result = new float[Width * Height * 3];
			int index;
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int col = GetPixelValues(x, y);
					index = x * 3 + y * Width * 3;
					result[index + 0] = ((col >> 16) & 0xFF) / 255f; // red
					result[index + 1] = ((col >> 8) & 0xFF) / 255f; // green
					result[index + 2] = (col & 0xFF) / 255f; // blue
				}
			}
			return new FloatSeries(3, result);
		}
		public FloatSeries ToFloatSeriesHex() // temp until sampling is corrected
		{
			var result = new float[Width * Height * 3];
			int index;
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int col = GetPixelValues(x, y);
					index = x * 3 + y * Width * 3;
					result[index + 0] = ((col >> 16) & 0xFF) / 255f; // red
					result[index + 1] = ((col >> 8) & 0xFF) / 255f; // green
					result[index + 2] = (col & 0xFF) / 255f; // blue
					if ((y & 1) == 1 && x < Width - 1)
					{
						int col2 = GetPixelValues(x + 1, y);
						var r2 = ((col2 >> 16) & 0xFF) / 255f; // red
						var g2 = ((col2 >> 8) & 0xFF) / 255f; // green
						var b2 = (col2 & 0xFF) / 255f; // blue
						result[index] += (r2 - result[index]) / 2f;
						result[index+1] += (g2 - result[index+1]) / 2f;
						result[index+2] += (b2 - result[index+2]) / 2f;
                    }
				}
			}
			return new FloatSeries(3, result);
		}

        public IntSeries ToIntSeries()
		{
			var result = new int[Width * Height * 3];
			int index;
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int col = GetPixelValues(x, y);
					index = x * 3 + y * Width * 3;
                    result[index + 0] = (col >> 16) & 0xFF; // red
					result[index + 1] = (col >> 8) & 0xFF; // green
					result[index + 2] = col & 0xFF; // blue
					index += 3;
				}
			}
			return new IntSeries(3, result);
		}

        public void SetPixel(int x, int y, uint color)
		{
			int index = x + (y * Width);
			Bits[index] = (int)color;
		}

		public void SetPixel(int x, int y, Color color)
		{
			int index = x + (y * Width);
			int col = color.ToArgb();

			Bits[index] = col;
		}

		public int GetPixelValues(int x, int y)
		{
			int index = x + (y * Width);
			return Bits[index];
		}
        public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
        }

        public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Bitmap.Dispose();
			BitsHandle.Free();
		}
	}
}