using System;
using System.Linq;
using Motive.Samplers.Utils;

namespace Motive.SeriesData
{
	public class StrideSeries : IntSeries
	{
		public GrowthMode GrowthMode { get; set; }

		public StrideSeries(GrowthMode growthMode, int vectorSize, params int[] values) : base(vectorSize, values)
		{
			GrowthMode = growthMode;
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
				}

				return result;
			}
		}

		public ISeries PositionsFromT(float t)
		{
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

    }
}