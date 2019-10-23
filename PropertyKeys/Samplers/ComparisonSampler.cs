using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public delegate Series SeriesEquation(Series a, Series b);

	public enum SeriesEquationType
	{
		Distance,
	}

    public class ComparisonSampler : Sampler
    {
	    private IStore _storeA;
	    private IStore _storeB;
	    private SeriesEquation _seriesEquation;

	    public ComparisonSampler(IStore storeA, IStore storeB, SeriesEquation seriesEquation, int capacity = 1)
	    {
		    _storeA = storeA;
		    _storeB = storeB;
		    _seriesEquation = seriesEquation;
		    Capacity = capacity;
	    }
	    public ComparisonSampler(IStore storeA, IStore storeB, SeriesEquationType seriesEquationType, int capacity = 1) : 
		    this(storeA, storeB, GetSeriesEquationByType(seriesEquationType), capacity){ }

        public override Series GetValueAtIndex(Series series, int index)
	    {
		    var indexT = index / (Capacity - 1f);
		    return GetValuesAtT(series, indexT);
	    }

	    public override Series GetValuesAtT(Series series, float t)
	    {
		    var resultA = _storeA.GetValuesAtT(t);
		    var resultB = _storeB.GetValuesAtT(t);
		    Series t2 = _seriesEquation(resultA, resultB);
		    return series.GetValueAtT(t2.X); // todo: should this adjust to assume the series equation always returns a parametric t?
	    }

	    public override ParametricSeries GetSampledTs(float t)
	    {
		    var resultA = _storeA.GetSampledTs(t);
		    var resultB = _storeB.GetSampledTs(t);
		    var result = _seriesEquation(resultA, resultB);
			if(!(result is ParametricSeries))
			{
				result = new ParametricSeries(result.VectorSize, result.FloatData);
			}
			return (ParametricSeries) result;
	    }

	    private static SeriesEquation GetSeriesEquationByType(SeriesEquationType seriesEquationType)
	    {
		    SeriesEquation result;

            switch (seriesEquationType)
		    {
                case SeriesEquationType.Distance:
	                result = DistanceEquation;
                    break;
                default:
	                result = DefaultEquation;
	                break;
		    }

            return result;
	    }

        private static Series DefaultEquation(Series seriesA, Series seriesB)
	    {
		    return seriesA;
	    }

        private static Series DistanceEquation(Series seriesA, Series seriesB)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => (float) Math.Sqrt(a * a + b * b));
        }

        private delegate float FloatEquation(float a, float b); // todo: should be series's in the params
        private static Series GeneralEquation(Series seriesA, Series seriesB, FloatEquation floatEquation)
	    {
		    Series result;
		    var aVals = seriesA.FloatData;
		    var bVals = seriesB.FloatData;
            if (aVals.Length == bVals.Length)
            {
				// Faster probable version.
	            var resultArray = new float[seriesA.Count];
                for (int i = 0; i < seriesA.Count; i += 2) // todo: generalize to wider vectorSize
			    {
				    resultArray[i] = floatEquation(bVals[i] - aVals[i], bVals[i + 1] - aVals[i + 1]);
                }
                result = SeriesUtils.CreateSeriesOfType(seriesA, resultArray);
            }
		    else
		    {
				// These are differently shaped, so need to query each one as some values will be virtual.
			    Series widest = seriesA.VectorSize >= seriesB.VectorSize ? seriesA : seriesB;
			    Series longest = seriesA.Count >= seriesB.Count ? seriesA : seriesB;
			    result = widest.GetZeroSeries(longest.Count); 
			    for (int i = 0; i < result.Count; i++)
			    {
				    var a = seriesA.GetSeriesAtIndex(i);
				    var b = seriesB.GetSeriesAtIndex(i);
				    var resultArray = new float[result.VectorSize];
				    for (int j = 0; j < result.VectorSize; j++)
				    {
					    float af = a.FloatDataAt(j);
					    float bf = b.FloatDataAt(j);

					    resultArray[j] = floatEquation(af, bf);
				    }

				    result.SetSeriesAtIndex(i, new FloatSeries(result.VectorSize, resultArray));
			    }
		    }

		    return result;
	    }
    }
}
