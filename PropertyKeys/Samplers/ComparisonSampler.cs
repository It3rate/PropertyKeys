using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public delegate ParametricSeries SeriesEquation(ParametricSeries a, ParametricSeries b);

	public enum SeriesEquationType
	{
		Polar, // returns len, normalizedAngle
		Distance, // returns len
        SignedDistance, // returns xDist, yDist
	}

    public class ComparisonSampler : Sampler
    {
	    private Sampler _sampleA;
	    private Sampler _sampleB;
	    private SeriesEquation _seriesEquation;
	    private Slot[] SlotMapping; // uppercase name for conflict, as this will probably become a Sampler property

        public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquation seriesEquation, Slot[] slotMapping = null, int capacity = 1)
	    {
		    _sampleA = sampleA;
		    _sampleB = sampleB;
		    _seriesEquation = seriesEquation;
		    SlotMapping = slotMapping;
		    Capacity = capacity;
	    }
	    public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquationType seriesEquationType, Slot[] slotMapping = null, int capacity = 1) : 
		    this(sampleA, sampleB, GetSeriesEquationByType(seriesEquationType), slotMapping, capacity){ }

        public override Series GetValueAtIndex(Series series, int index)
	    {
		    var indexT = index / (Capacity - 1f);
		    return GetValuesAtT(series, indexT);
	    }

	    public override Series GetValuesAtT(Series series, float t)
	    {
		    var t2 = GetSampledTs(new ParametricSeries(1, t));

            float[] floats = new float[t2.VectorSize];
		    for (int i = 0; i < t2.VectorSize; i++)
		    {
			    floats[i] = series.GetValueAtT(t2[i]).FloatDataAt(i);
		    }
		    return SeriesUtils.CreateSeriesOfType(series, floats);
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    var resultA = _sampleA.GetSampledTs(seriesT);
		    var resultB = _sampleB.GetSampledTs(seriesT);
		    var result = _seriesEquation(resultA, resultB);
		    return (ParametricSeries)SeriesUtils.GetMappedSeries(SlotMapping, result);
	    }

        private delegate float FloatEquation(float a, float b); // todo: should be series's in the params
        private static ParametricSeries GeneralEquation(ParametricSeries seriesA, ParametricSeries seriesB, FloatEquation floatEquation)
	    {
			// note: so far parametricSeries can only have one vector, so this is redundant atm.
		    ParametricSeries result;
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
                result = (ParametricSeries)SeriesUtils.CreateSeriesOfType(seriesA, resultArray);
            }
		    else
		    {
                // These are differently shaped, so need to query each one as some values will be virtual.
                int widest = seriesA.VectorSize >= seriesB.VectorSize ? seriesA.VectorSize : seriesB.VectorSize;
			    var resultArray = new float[widest];
			    for (int j = 0; j < widest; j++)
			    {
				    float af = seriesA[j]; //a.FloatDataAt(j);
				    float bf = seriesB[j]; //b.FloatDataAt(j);

				    resultArray[j] = floatEquation(af, bf);
			    }

			    result = new ParametricSeries(widest, resultArray);
		    }

		    return result;
        }

        private static SeriesEquation GetSeriesEquationByType(SeriesEquationType seriesEquationType)
        {
	        SeriesEquation result;

	        switch (seriesEquationType)
	        {
		        case SeriesEquationType.Distance:
			        result = DistanceEquation;
			        break;
		        case SeriesEquationType.Polar:
			        result = PolarEquation;
			        break;
                case SeriesEquationType.SignedDistance:
			        result = SignedDistanceEquation;
			        break;
		        default:
			        result = DefaultEquation;
			        break;
	        }

	        return result;
        }

        private static ParametricSeries DefaultEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
	        return seriesA;
        }

        private static ParametricSeries DistanceEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => (float)Math.Sqrt(a * a + b * b));
        }

        private static ParametricSeries PolarEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
	        float a = seriesB[0] - seriesA[0];
	        float b = seriesB[1] - seriesA[1];
	        var array = new float[]{(float)Math.Sqrt(a * a + b * b), (float) (Math.Atan2(b, a) / (2 * Math.PI))};
	        return new ParametricSeries(2, array);
        }

        private static ParametricSeries SignedDistanceEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
	        var floats = new float[seriesA.VectorSize];
	        for (int i = 0; i < seriesA.VectorSize; i++)
	        {
		        float dif = seriesB[i] - seriesA[i];
		        floats[i] = 1f - ((dif * dif * dif + 1f) * 0.5f);
	        }
	        return new ParametricSeries(seriesA.VectorSize, floats);
        }

    }
}
