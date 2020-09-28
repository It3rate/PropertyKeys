using System;
using System.Linq;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
	public class GridSampler : Sampler
	{
        public GridSampler(int[] strides, Slot[] swizzleMap = null, GrowthType growthType = GrowthType.Product) : base(swizzleMap)
        {
			GrowthType = growthType;
			Strides = strides;
			SampleCount = StridesToSampleCount(Strides);

            ClampTypes = new ClampType[strides.Length];
            for (int i = 0; i < strides.Length - 1; i++)
            {
	            ClampTypes[i] = Samplers.ClampType.Wrap;
            }
            ClampTypes[strides.Length - 1] = Samplers.ClampType.None;
            AlignmentTypes = new AlignmentType[strides.Length];
        }

		public override Series GetValueAtIndex(Series series, int index)
        {
            // access special case directly to avoid index>t>index>t conversions
            var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, SampleCount, index);
            return GetSeriesSample(series, seriesT);
		}

		protected ParametricSeries GetSampledTsNoSwizzle(ParametricSeries seriesT, out int[] positions)
		{
			return GrowthType == GrowthType.Product ?
				SamplerUtils.DistributeTBySampler(seriesT, this, out positions) :
				SamplerUtils.DistributeTBySummedSampler(seriesT, this, out positions);
        }
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        var result = GetSampledTsNoSwizzle(seriesT, out var positions);
            return Swizzle(result, seriesT);
        }
        
        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < Strides.Length) ? series.GetVirtualValueAt(seriesT[i]).FloatDataAt(i) : 0;
			}

			return SeriesUtils.CreateSeriesOfType(series, result);
        }

        public override int NeighborCount => 4;
        private int WrappedIndexes(int x, int y) => (x >= Strides[0] ? 0 : x < 0 ? Strides[0] - 1 : x) +  Strides[0] * (y >= Strides[1] ? 0 : y < 0 ? Strides[1] - 1 : y);
        public override Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, SampleCount, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
            var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);

            result.SetRawDataAt(0, series.GetVirtualValueAt(WrappedIndexes(indexX + 1, indexY), SampleCount));
            result.SetRawDataAt(1, series.GetVirtualValueAt(WrappedIndexes(indexX, indexY - 1), SampleCount));
            result.SetRawDataAt(2, series.GetVirtualValueAt(WrappedIndexes(indexX - 1, indexY), SampleCount));;
            result.SetRawDataAt(3, series.GetVirtualValueAt(WrappedIndexes(indexX, indexY + 1), SampleCount));

            return result;
        }
    }
}