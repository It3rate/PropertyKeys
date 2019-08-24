using PropertyKeys.Graphic;
using PropertyKeys.Keys;
using PropertyKeys.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Components
{
    public class Hexgrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public float Spacing { get; set; }
        public PolyShape Shape { get; set; }
        public Vector3Store Locations { get; set; }

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
            Shape = new PolyShape(pointCount: 6, radius: 10f, orientation: 1f / 12f);

            float totalWidth = 1f;
            float armLen = totalWidth / (float)(Columns - 1) / 3f;
            float totalHeight = (armLen * (float)Math.Sqrt(3)) / 2f * (Rows - 1f);
            Shape.Radius = armLen + Spacing * armLen;
            Vector3[] start = new Vector3[] { new Vector3(0, 0, 0), new Vector3(totalWidth, totalHeight, 0) };
            Locations = new Vector3Store(start, elementCount: Columns * Columns, dimensions: new int[] { Columns, 0, 0 }, sampleType: SampleType.Hexagon);
        }
    }
}
