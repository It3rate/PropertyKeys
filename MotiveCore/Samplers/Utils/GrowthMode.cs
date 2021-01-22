using System;
using System.Linq;
using Motive.SeriesData;

namespace Motive.Samplers.Utils
{
    /// <summary>
    /// Determines how Strides are treated when calculating total possible virtual elements. A nxn grid is multiplies, but multiple rows of specified lengths are added.
    /// </summary>
    public enum GrowthMode
	{
		Product = 0,
		Widest,
		Sum,
		Fixed,
	}

    public static class GrowthModeExtension
    {
	    public static int GetCapacityOf(this GrowthMode growthMode, int[] strides)
	    {
		    int result = 0;
		    switch (growthMode)
		    {
			    case GrowthMode.Product:
				    result = strides.Aggregate(1, (a, b) => b != 0 ? a * b : a);
				    break;
			    case GrowthMode.Widest:
				    result = strides.Max() * strides.Length;
				    break;
			    case GrowthMode.Sum:
				    result = strides.Sum();
				    break;
		    }
		    return result;
        }

    }
}