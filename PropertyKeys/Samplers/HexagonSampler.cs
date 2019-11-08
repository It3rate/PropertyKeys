using System;
using System.Diagnostics;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public class HexagonSampler : GridSampler
	{
		public HexagonSampler(int[] strides, Slot[] swizzleMap = null) : base(strides, swizzleMap) { }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            var result = seriesT.VectorSize == 1 ? SamplerUtils.GetMultipliedJaggedTFromT(Strides, Capacity, seriesT.X) : seriesT;
            bool isOddRow = (SamplerUtils.IndexFromT(Strides[1], result.Y) & 1) == 1;
            float hexRowScale = 1f / (Strides[0] - 1f) * 0.5f;
            result[0] *= (1f - hexRowScale);
            result[1] *= (1f - hexRowScale);
            if (isOddRow)
            {
                result[0] += hexRowScale;
            }
            return Swizzle(result, seriesT);
        }
        public override int NeighborCount => 6;
        private int WrappedIndexes(int x, int y) => (x >= Strides[0] ? 0 : x < 0 ? Strides[0] - 1 : x) + Strides[0] * (y >= Strides[1] ? 0 : y < 0 ? Strides[1] - 1 : y);
        public override Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, Capacity, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
	        var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);
            int offset = (indexY & 1) == 1 ? 0 : -1;
            
            result.SetSeriesAtIndex(0, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1, indexY), Capacity)); // right
	        result.SetSeriesAtIndex(1, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1 + offset, indexY - 1), Capacity)); // top right
	        result.SetSeriesAtIndex(2, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 0 + offset, indexY - 1), Capacity)); // top Left
            result.SetSeriesAtIndex(3, series.GetValueAtVirtualIndex(WrappedIndexes(indexX - 1, indexY), Capacity)); // left
            result.SetSeriesAtIndex(4, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 0 + offset, indexY + 1), Capacity)); // bottom left
            result.SetSeriesAtIndex(5, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1 + offset, indexY + 1), Capacity)); // bottom right
            return result;
        }

		/// <summary>
        /// Creates a fitted hex grid with a calculated number of rows, a polyshape renderer with the correct radius, and items count that matches the grid.
        /// </summary>
        /// <param name="bounds">The bounds to fit the grid into.</param>
        /// <param name="columns">The set number of columns. The number of rows is calculated based on this and the height.</param>
        /// <returns>A fitted composite hex grid.</returns>
        public static Container CreateBestFit(RectFSeries bounds, int columns, out int rows)
		{
			float totalWidth = bounds.Width;
            float w = bounds.Width / (columns - 1f); // calculating spacing, from centers, so subtract 1
            float h = w * (float)(2.0 / Math.Sqrt(3));
			float vSpacing = h * .75f;
	        rows = (int)(bounds.Height / vSpacing);
	        float totalHeight = vSpacing * (rows - 1f); // calculating spacing, from centers, so subtract 1

	        bounds[0] += w / 2f;
	        bounds[1] += vSpacing * 0.25f;
			
	        bounds[2] = bounds.Left + totalWidth;
            bounds[3] = bounds.Top + totalHeight;

            var sampler = new HexagonSampler(new int[] { columns, rows });
	        var composite = new Container(Store.CreateItemStore(sampler.Capacity));
	        float overdraw = 1.00f;
            float radiusScale = 1f - 1f / (columns - 1f) * 0.5f; // rows are offset, and thus compressed when drawn by this much.
            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, h/2f * radiusScale * overdraw).Store);

	        Store loc = new Store(bounds, sampler);
	        composite.AddProperty(PropertyId.Location, loc);

	        composite.Renderer = new PolyShape(packHorizontal:true);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, sampler.NeighborCount).Store);

            return composite;
        }
    }
}