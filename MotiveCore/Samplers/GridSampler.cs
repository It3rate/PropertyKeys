using System;
using System.Linq;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers
{
    /// <summary>
    /// Samples grid of SampleCount based on Strides, sampled along each element of VectorSize.
    /// </summary>
	public class GridSampler : Sampler
	{
		public AlignmentType[] AlignmentTypes { get; protected set; } // left, right, centered, justified

        public GridSampler(int[] strides, Slot[] swizzleMap = null, GrowthType growthType = GrowthType.Product) : base(swizzleMap)
        {
			GrowthType = growthType;
			Strides = strides;
			SampleCount = SamplerUtils.StridesToSampleCount(Strides, GrowthType);

            ClampTypes = new ClampMode[strides.Length];
            for (int i = 0; i < strides.Length - 1; i++)
            {
	            ClampTypes[i] = ClampMode.Wrap;
            }
            ClampTypes[strides.Length - 1] = ClampMode.None;
            AlignmentTypes = new AlignmentType[strides.Length];
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
        
        protected override ISeries GetSeriesSample(ISeries series, ParametricSeries seriesT)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < Strides.Length) ? series.GetInterpolatedSeriesAt(seriesT[i]).FloatValueAt(i) : 0;
			}

			return SeriesUtils.CreateSeriesOfType(series, result);
        }

        public override int NeighborCount => 4;
        private int WrappedIndexes(int x, int y) => (x >= Strides[0] ? 0 : x < 0 ? Strides[0] - 1 : x) +  Strides[0] * (y >= Strides[1] ? 0 : y < 0 ? Strides[1] - 1 : y);
        public override ISeries GetNeighbors(ISeries series, int index, bool wrapEdges = true)
        {
	        var seriesT = SamplerUtils.GetMultipliedJaggedT(Strides, SampleCount, index);
	        int indexX = SamplerUtils.IndexFromT(Strides[0], seriesT[0]);
	        int indexY = SamplerUtils.IndexFromT(Strides[1], seriesT[1]);
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
            var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount]);

            result.SetSeriesAt(0, series.GetSeriesAt(WrappedIndexes(indexX + 1, indexY)));
            result.SetSeriesAt(1, series.GetSeriesAt(WrappedIndexes(indexX, indexY - 1)));
            result.SetSeriesAt(2, series.GetSeriesAt(WrappedIndexes(indexX - 1, indexY)));;
            result.SetSeriesAt(3, series.GetSeriesAt(WrappedIndexes(indexX, indexY + 1)));

            return result;
        }
    }
}