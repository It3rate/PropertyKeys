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
		public HexagonSampler(int[] strides, Slot[] swizzleMap = null) : base(strides, swizzleMap)
		{
			ClampType = new ClampType[strides.Length];
			for (int i = 0; i < strides.Length - 1; i++)
			{
				ClampType[i] = Samplers.ClampType.Wrap;
			}
			ClampType[strides.Length - 1] = Samplers.ClampType.None;
			AlignmentType = new AlignmentType[strides.Length];
		}

		private static ParametricSeries DistributeTBySamplerX(float t, Sampler sampler, out int[] positions)
		{
			var len = sampler.Strides.Length;
			var result = new float[len];
			positions = new int[len];
			float segSize = 1f / sampler.SampleCount;
			float rem = (t > 1f) ? t : 1f + segSize / 2f;
			float ct = t;
			for (int i = len - 1; i >= 0; i--)
			{
				rem /= (float)sampler.Strides[i];
				int pos = (int)Math.Floor(ct / rem);
				ct -= pos * rem;
				positions[i] = pos;
				result[i] = pos / (float)(sampler.Strides[i]);
			}
			return new ParametricSeries(len, result);
		}
		private static ParametricSeries DistributeTBySampler(float t, Sampler sampler, out int[] positions)
		{
			var len = sampler.Strides.Length;
			var result = new float[len];
			positions = new int[len];
			float segSize = 1f / (sampler.SampleCount - 1);
			float offsetT = t + segSize / 2f;
			for (int i = 0; i < len; i++)
			{
				int curStride = sampler.Strides[i];
				float div = offsetT / segSize;
				int pos = (int)Math.Floor(div);
				int index = pos % curStride;
				if (sampler.ClampType.Length > i)
				{
					switch (sampler.ClampType[i])
					{
						case Samplers.ClampType.None:
							break;
						case Samplers.ClampType.Wrap:
							pos = index;
							break;
						case Samplers.ClampType.WrapRight:
							pos = curStride - index;
                            break;
						case Samplers.ClampType.Mirror:
							pos = ((index / curStride) & 1) == 0 ? index : curStride - index;
                            break;
						case Samplers.ClampType.ClampAtZero:
							pos = (pos < 0) ? 0 : index;
                            break;
						case Samplers.ClampType.ClampAtOne:
							pos = (pos > 1) ? 1 : index;
                            break;
						case Samplers.ClampType.Clamp:
							pos = (pos < 0) ? 0 : (pos > 1) ? 1 : index;
                            break;
                    }
				}
				positions[i] = pos;
				result[i] = pos / (float)(curStride - 1);
				segSize *= curStride;
			}
			return new ParametricSeries(len, result);
		}
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            var result = DistributeTBySampler(seriesT.X, this, out var positions);
            bool isOddRow = (positions[1] & 1) == 1;
            //var result = seriesT.VectorSize == 1 ? SamplerUtils.GetMultipliedJaggedTFromT(Strides, SampleCount, seriesT.X) : seriesT;
            //bool isOddRow = (SamplerUtils.IndexFromT(Strides[1], result.Y) & 1) == 1;
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
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, SampleCount, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
	        var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);
            int offset = (indexY & 1) == 1 ? 0 : -1;
            
            result.SetRawDataAt(0, series.GetVirtualValueAt(WrappedIndexes(indexX + 1, indexY), SampleCount)); // right
	        result.SetRawDataAt(1, series.GetVirtualValueAt(WrappedIndexes(indexX + 1 + offset, indexY - 1), SampleCount)); // top right
	        result.SetRawDataAt(2, series.GetVirtualValueAt(WrappedIndexes(indexX + 0 + offset, indexY - 1), SampleCount)); // top Left
            result.SetRawDataAt(3, series.GetVirtualValueAt(WrappedIndexes(indexX - 1, indexY), SampleCount)); // left
            result.SetRawDataAt(4, series.GetVirtualValueAt(WrappedIndexes(indexX + 0 + offset, indexY + 1), SampleCount)); // bottom left
            result.SetRawDataAt(5, series.GetVirtualValueAt(WrappedIndexes(indexX + 1 + offset, indexY + 1), SampleCount)); // bottom right
            return result;
        }

		/// <summary>
        /// Creates a fitted hex grid with a calculated number of rows, a polyshape renderer with the correct radius, and items count that matches the grid.
        /// </summary>
        /// <param name="bounds">The bounds to fit the grid into.</param>
        /// <param name="columns">The set number of columns. The number of rows is calculated based on this and the height.</param>
        /// <returns>A fitted composite hex grid.</returns>
        public static Container CreateBestFit(RectFSeries bounds, int columns, out int rows, out float radius, out HexagonSampler sampler)
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

            sampler = new HexagonSampler(new int[] { columns, rows });
	        var composite = new Container(Store.CreateItemStore(sampler.SampleCount));
	        float overdraw = 1.00f;
            float radiusScale = 1f - 1f / (columns - 1f) * 0.5f; // rows are offset, and thus compressed when drawn by this much.
            radius = h / 2f * radiusScale * overdraw;
            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, radius).Store());

            Store loc = new Store(bounds, sampler);
	        composite.AddProperty(PropertyId.Location, loc);

	        composite.Renderer = new PolyShape(packHorizontal:true);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, sampler.NeighborCount).Store());

            return composite;
        }
    }
}