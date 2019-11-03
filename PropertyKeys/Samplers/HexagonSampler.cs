using System;
using System.Diagnostics;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class HexagonSampler : GridSampler
	{
		public HexagonSampler(int[] strides, Slot[] swizzleMap = null) : base(strides, swizzleMap) { }

        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
		{
			var result = base.GetSeriesSample(series, seriesT);
			var curRow = SamplerUtils.IndexFromT(Strides[1], seriesT.Y);// (int)Math.Round(seriesT.Y * (Strides[1] - 1));
            if ((curRow & 1) == 1)
            {
	            var data = result.FloatDataRef;
	            data[0] += series.Size.X / (Strides[0] - 1f) * 0.5f;
            }
	        return result;
        }
        protected override int NeighborCount => 6;
        private int WrappedIndexes(int x, int y) => (x >= Strides[0] ? 0 : x < 0 ? Strides[0] - 1 : x) + Strides[0] * (y >= Strides[1] ? 0 : y < 0 ? Strides[1] - 1 : y);
        public override Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, Capacity, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
	        var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);
            int offset = (indexY & 1) == 1 ? 0 : -1;

            //var a = WrappedIndexes(indexX + 1, indexY);
            //var b = WrappedIndexes(indexX, indexY - 1);
            //var c = WrappedIndexes(indexX - 1, indexY - 1);
            //var d = WrappedIndexes(indexX - 1, indexY);
            //var e = WrappedIndexes(indexX - 1, indexY + 1);
            //var f = WrappedIndexes(indexX, indexY + 1);
            //Debug.WriteLine(index + " * " + indexX + " : " + indexY + "      :" + a + " : " + b + " : " + c + " : " + d + " : " + e + " : " + f + " ::: " + offset);

            result.SetSeriesAtIndex(0, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1, indexY), Capacity)); // right
	        result.SetSeriesAtIndex(1, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1 + offset, indexY - 1), Capacity)); // top right
	        result.SetSeriesAtIndex(2, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 0 + offset, indexY - 1), Capacity)); // top Left
            result.SetSeriesAtIndex(3, series.GetValueAtVirtualIndex(WrappedIndexes(indexX - 1, indexY), Capacity)); // left
            result.SetSeriesAtIndex(4, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 0 + offset, indexY + 1), Capacity)); // bottom left
            result.SetSeriesAtIndex(5, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1 + offset, indexY + 1), Capacity)); // bottom right
            return result;
        }
    }
}