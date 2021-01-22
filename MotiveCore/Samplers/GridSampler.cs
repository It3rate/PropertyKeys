using System;
using System.Linq;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers.Utils
{
    /// <summary>
    /// Samples grid of SampleCount based on Strides, sampled along each element of VectorSize.
    /// </summary>
	public class GridSampler : Sampler
	{
		public AlignMode[] AlignmentTypes { get; protected set; } // left, right, centered, justified

        public GridSampler(int[] strides, Slot[] swizzleMap = null, GrowthMode growthMode = GrowthMode.Product) : base(swizzleMap)
        {
			GrowthMode = growthMode;
			Strides = strides;
			SampleCount = GrowthMode.GetCapacityOf(Strides);

            ClampModes = new ClampMode[strides.Length];
            for (int i = 0; i < strides.Length - 1; i++)
            {
	            ClampModes[i] = ClampMode.Wrap;
            }
            ClampModes[strides.Length - 1] = ClampMode.None;
            AlignmentTypes = new AlignMode[strides.Length];
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
	        var result = GetGridPositions(seriesT, out var positions);
            return Swizzle(result, seriesT);
        }

        protected ParametricSeries GetGridPositions(ParametricSeries seriesT, out int[] positions)
        {
            ParametricSeries result;
            switch (GrowthMode)
            {
                case GrowthMode.Product:
                    result = DistributeTBySampler(seriesT, this, out positions);
                    break;
                //case GrowthMode.Sum:
                //case GrowthMode.Widest:
                //case GrowthMode.Fixed:
                default:
                    result = DistributeTBySummedSampler(seriesT, this, out positions);
                    break;
            }
            return result;
        }
        private static ParametricSeries DistributeTBySummedSampler(ParametricSeries seriesT, Sampler sampler, out int[] positions)
        {
            // This is inherently 2D because is defines the row sizes.
            var result = new float[2];
            positions = new int[2];
            int maxStride = sampler.Strides.Max();
            int strideSum = sampler.Strides.Sum();
            var rows = sampler.Strides.Length;
            float minSampleSize = 1f / strideSum;
            int rowLength;
            bool hasMultipleT = seriesT.Count > 1;
            if (hasMultipleT)
            {
                positions[1] = (int)Math.Floor(seriesT[1] * rows);
                rowLength = sampler.Strides[positions[1]];
                positions[0] = (int)Math.Floor(seriesT[0] * rowLength);
            }
            else
            {
                float total = 0;
                for (int i = 0; i < rows; i++)
                {
                    rowLength = sampler.Strides[i];
                    if (total + minSampleSize * rowLength < seriesT[0])
                    {
                        total += minSampleSize * rowLength;
                    }
                    else
                    {
                        float rem = seriesT[0] - total;

                        positions[0] = (int)Math.Floor(rem * strideSum);
                        positions[1] = i;
                        result[0] = positions[0] / (float)maxStride;
                        result[1] = i / (rows - 1f);
                        break;
                    }
                }
            }
            // only makes sense to clamp element one when rows are manually defined.
            if (sampler.ClampModes?.Length > 0)
            {
                switch (sampler.ClampModes[0])
                {
                    case ClampMode.Clamp:
                        positions[0] = (positions[0] < 0) ? 0 : (positions[0] > 1) ? 1 : positions[0];
                        break;
                    case ClampMode.Mirror:
                        positions[0] = (positions[1] & 1) == 0 ? positions[0] : (maxStride - 1) - positions[0];
                        break;

                        // these don't make sense in Summed distribution as there is no wrapping
                        //case ClampMode.Wrap:
                        // positions[0] = positions[0];
                        // break;
                        //case ClampMode.ReverseWrap:
                        // positions[0] = (maxStride - 1) - positions[0];
                        // break;
                }
                result[0] = positions[0] / (float)maxStride;
            }
            return new ParametricSeries(2, result);
        }
        private static ParametricSeries DistributeTBySampler(ParametricSeries seriesT, Sampler sampler, out int[] positions)
        {
            var len = sampler.Strides.Length;
            var result = new float[len];
            positions = new int[len];
            bool hasMultipleT = seriesT.Count > 1;
            float minSegSize = 1f / (sampler.SampleCount - 1);
            float offsetT = seriesT[0] + minSegSize / 2f;
            for (int i = 0; i < len; i++)
            {
                // allow input of multiple parameters - use position per stride in this case.
                // if mismatched lengths, just use last value available (basically error condition).
                if (hasMultipleT && seriesT.Count > i)
                {
                    minSegSize = 1f / (sampler.Strides[i] - 1);
                    offsetT = seriesT[i] + minSegSize / 2f;
                }
                int curStride = sampler.Strides[i];
                float div = offsetT / minSegSize;
                int pos = (int)Math.Floor(div);
                int index = pos % curStride;
                if (sampler.ClampModes.Length > i)
                {
                    switch (sampler.ClampModes[i])
                    {
                        case ClampMode.None:
                            break;
                        case ClampMode.Clamp:
                            pos = (pos < 0) ? 0 : (pos > 1) ? 1 : index;
                            break;
                        case ClampMode.Wrap:
                            pos = index;
                            break;
                        case ClampMode.ReverseWrap:
                            pos = curStride - index;
                            break;
                        case ClampMode.Mirror:
                            pos = ((index / curStride) & 1) == 0 ? index : curStride - index;
                            break;
                    }
                }
                positions[i] = pos;
                result[i] = pos / (float)(curStride - 1);
                minSegSize *= curStride;
            }
            return new ParametricSeries(len, result);
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