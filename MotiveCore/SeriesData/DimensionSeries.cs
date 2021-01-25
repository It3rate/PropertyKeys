using System;
using System.Linq;
using Motive.Components;
using Motive.Samplers.Utils;

namespace Motive.SeriesData
{
	public class DimensionSeries : IntSeries
	{
		public GrowthMode GrowthMode { get; set; }

		private readonly int _fixedCapacity;
		private readonly IContainer _referenceCapacity;

        public DimensionSeries(GrowthMode growthMode, params int[] values) : base(values.Length, values)
		{
			GrowthMode = growthMode;
		}
		public DimensionSeries(IContainer reference, params int[] values) : base(values.Length, values)
		{
			_referenceCapacity = reference;
			GrowthMode = GrowthMode.Reference;
		}
		public DimensionSeries(int fixedCapacity, params int[] values) : base(values.Length, values)
		{
			_fixedCapacity = fixedCapacity;
			GrowthMode = GrowthMode.Reference;
		}

		public int Capacity
		{
			get
			{
				int result = 0;
				switch (GrowthMode)
				{
					case GrowthMode.Product:
						for (int i = 0; i < VectorSize; i++)
						{
							if (IntValueAt(i) != 0)
							{
								result *= IntValueAt(i);
							}
						}
						break;
					case GrowthMode.Widest:
						for (int i = 0; i < VectorSize; i++)
						{
							if (IntValueAt(i) > result)
							{
								result = IntValueAt(i);
							}
						}
                        result *= VectorSize;
						break;
					case GrowthMode.Sum:
						for (int i = 0; i < VectorSize; i++)
						{
							result += IntValueAt(i);
						}
						break;
					case GrowthMode.Fixed:
						result = _fixedCapacity;
						break;
					case GrowthMode.Reference:
						result = _referenceCapacity.Capacity;
						break;
                }

				return result;
			}
		}

		private ParametricSeries GetSummedJaggedT(int[] segments, int index, bool isLastSegmentDiscrete = false)
		{
			int row = 0;
			int col = index;
			foreach (var seg in segments)
			{
				if (col >= seg)
				{
					col -= seg;
					row++;
				}
				else
				{
					break;
				}
			}
			int colLength = segments[row];
			// the index could overflow the sum of segments, so finish the calculation regardless
			float indexT = segments.Length > 0 ? row / (float)segments.Length : 0;
			float discreteAdjust = isLastSegmentDiscrete ? 0f : 1f;
			float remainder = colLength > 1 ? col / (colLength - discreteAdjust) : 0;
			return new ParametricSeries(2, indexT, remainder);
		}

        public ParametricSeries PositionsFromT(float t)
		{
			return null;
		}
		public float TFromPositions(ParametricSeries series)
		{
			return 0;
		}
		public ParametricSeries PositionsFromIndex(int index)
		{
			return null;
		}
		public int IndexFromPositions(ParametricSeries series)
		{
			return 0;
		}
		public ParametricSeries MapFromSeries(DimensionSeries series, float t) // potential mismatched dimensions
		{
            // if series.vectorsize 1 use this.PositionsFromT(t)
            // if both vectorsizes are equal, use series.PositionsFromT(t) (and snap to nearest?)
            // if not equal, use t1=series.TFromPositions(series.PositionsFromT(t)); return this.PositionsFromT(t1);
            return null;
		}

        public ISeries PositionsFromSeries(ParametricSeries series)
		{
			return null;
        }
		public int[] GetPositionsForIndex(int index)
		{
			var result = new int[VectorSize];
			var count = Math.Max(0, Math.Min(Capacity - 1, index));
			for (var i = VectorSize - 1; i >= 0; i--)
			{
				int dimSize = 1;
				for (int j = 0; j < i; j++)
				{
					dimSize *= IntValueAt(j);
				}
				result[i] = count / dimSize;
				count -= result[i] * dimSize;
			}
			return result;
		}

		private int IndexFromT(float t)
		{
			return Math.Max(0, Math.Min(Capacity - 1, (int)Math.Round(t * (Capacity - 1f))));
		}
		private float TFromIndex(int index)
		{
			return index / (Capacity - 1f);
		}
    }
}