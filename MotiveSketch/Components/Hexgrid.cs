using System;
using System.Drawing.Drawing2D;
using Motive.Graphic;
using Motive.Samplers.Utils;

namespace Motive.Components
{
	public class Hexgrid
	{
		public int Rows { get; set; }
		public int Columns { get; set; }

		public float Spacing { get; set; }
		//public FloatStore Locations { get; set; }

		//private PolyShape graphic;
		//private HexagonSampler sampler;

		public Matrix Transform { get; set; }

		public Hexgrid(int rows, int columns, float spacing = 0.02f)
		{
			Rows = rows;
			Columns = columns;
			Spacing = spacing;

			CreateGrid();
		}

		public void CreateGrid()
		{
			//Shape = new PolyShape(pointCount: 6, radius: 10f, orientation: 1f / 12f);

			var totalWidth = 1f;
			var armLen = totalWidth / (float) (Columns - 1) / 3f;
			var totalHeight = armLen * (float) Math.Sqrt(3) / 2f * (Rows - 1f);
			//Shape.Radius = armLen + Spacing * armLen;
			var start = new float[] {0, 0, totalWidth, totalHeight};
			//Locations = new FloatStore(2, start, elementCount: Columns * Columns, dimensions: new int[] { Columns, 0, 0 }, sampleType: SampleType.Hexagon);
		}
	}
}