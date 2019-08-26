using DataArcs.Graphic;
using DataArcs.Stores;
using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Components
{
    public class Hexgrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public float Spacing { get; set; }
        public PolyShape Shape { get; set; }
        public FloatStore Locations { get; set; }

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
            float[] start = new float[] { 0, 0, totalWidth, totalHeight };
            Locations = new FloatStore(2, start, elementCount: Columns * Columns, dimensions: new int[] { Columns, 0, 0 }, sampleType: SampleType.Hexagon);
        }
    }
}
