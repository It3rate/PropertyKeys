using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class GridSampler : Sampler
	{
		protected int[] Strides { get; }

        public GridSampler(int[] strides, Slot[] swizzleMap = null) : base(swizzleMap)
        {
			Strides = strides;
            Capacity = strides[0];
            for (int i = 1; i < strides.Length; i++)
            {
                if (strides[i] != 0)
                {
                    Capacity *= strides[i];
                }
                else
                {
                    break;
                }
            }
        }

		public override Series GetValueAtIndex(Series series, int index)
        {
            // access special case directly to avoid index>t>index>t conversions
            var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, Capacity, index);
            return GetSeriesSample(series, seriesT);
		}
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        var result = seriesT.VectorSize == 1 ? SamplerUtils.GetMultipliedJaggedTFromT(Strides, Capacity, seriesT.X) : seriesT;
            return Swizzle(result, seriesT);
        }
        
        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < Strides.Length) ? series.GetValueAtT(seriesT[i]).FloatDataAt(i) : 0;
			}

			return SeriesUtils.CreateSeriesOfType(series, result);
        }

        protected override int NeighborCount => 4;
        private int WrappedIndexes(int x, int y) => (x >= Strides[0] ? 0 : x < 0 ? Strides[0] - 1 : x) +  Strides[0] * (y >= Strides[1] ? 0 : y < 0 ? Strides[1] - 1 : y);
        public override Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, Capacity, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
            var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);

            result.SetSeriesAtIndex(0, series.GetValueAtVirtualIndex(WrappedIndexes(indexX + 1, indexY), Capacity));
            result.SetSeriesAtIndex(1, series.GetValueAtVirtualIndex(WrappedIndexes(indexX, indexY - 1), Capacity));
            result.SetSeriesAtIndex(3, series.GetValueAtVirtualIndex(WrappedIndexes(indexX - 1, indexY), Capacity));;
            result.SetSeriesAtIndex(5, series.GetValueAtVirtualIndex(WrappedIndexes(indexX, indexY + 1), Capacity));

            return result;
        }
    }
}