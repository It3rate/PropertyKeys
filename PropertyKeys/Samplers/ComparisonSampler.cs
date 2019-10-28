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
		/// <summary>
        /// Returns two values, first is length, the second is a normalized angle - East 0, North 0.25, West 0.5, South 0.75, and East 1.0.
        /// </summary>
		Polar,
        /// <summary>
        /// Returns the length between the two Parametric series in the X slot.
        /// </summary>
        Distance,
        Bell,
        SignedDistance, // returns xDist, yDist
	}

    public class ComparisonSampler : Sampler
    {
	    private Sampler _sampleA;
	    private Sampler _sampleB;
	    private SeriesEquation _seriesEquation;

        public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquation seriesEquation, Slot[] swizzleMap = null, int capacity = 1) : base(swizzleMap, capacity)
        {
		    _sampleA = sampleA;
		    _sampleB = sampleB;
		    _seriesEquation = seriesEquation;
		    Capacity = capacity;
	    }
	    public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquationType seriesEquationType, Slot[] swizzleMap = null, int capacity = 1) : 
		    this(sampleA, sampleB, GetSeriesEquationByType(seriesEquationType), swizzleMap, capacity){ }

        public override Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            Series result;
            if (_seriesEquation == PolarEquation && SwizzleMap == SlotUtils.XY)
            {
                float clampRatiox = 0.9f;
                float clampRatioy = 0.8f;
                var dist = seriesT[0];
                var xdist = Math.Max(0, dist - clampRatiox) * (1f / (1f - clampRatiox));
                var ydist = Math.Max(0, dist - clampRatioy) * (1f / (1f - clampRatioy));
                float x = (float)(Math.Cos(seriesT[1] * Math.PI * 2.0) * (-series.Frame.Width * xdist));
                float y = (float)(Math.Sin(seriesT[1] * Math.PI * 2.0) * (series.Frame.Height * ydist));
                result = new FloatSeries(2, x, y);
            }
            else
            {
                result = base.GetSeriesSample(series, seriesT);
            }

            return result;
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    var resultA = _sampleA.GetSampledTs(seriesT);
		    var resultB = _sampleB.GetSampledTs(seriesT);
		    var result = _seriesEquation(resultA, resultB);
		    return Swizzle(result, seriesT);
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
		        case SeriesEquationType.Bell:
			        result = BellEquation;
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
	        float total = 0;
	        int minLen = Math.Min(seriesA.VectorSize, seriesB.VectorSize);
	        for (int i = 0; i < minLen; i++)
	        {
		        total += (seriesB[i] - seriesA[i]) * (seriesB[i] - seriesA[i]);
            }
	        float result = (float)Math.Sqrt(total);

            return new ParametricSeries(minLen, result);
        }

        private static ParametricSeries PolarEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
	        float a = seriesB[0] - seriesA[0];
	        float b = seriesB[1] - seriesA[1];
	        float radAngle = (float)Math.Atan2(b, a);
	        float normAngle = radAngle / (float)(2 * Math.PI);
	        normAngle = 1f - normAngle;
	        normAngle = normAngle > 1 ? normAngle - 1f : normAngle;
            float dist = 1f - (float) Math.Sqrt(a * a + b * b);// * 0.7071f;
	        return new ParametricSeries(2, dist, normAngle);
        }

        private static ParametricSeries BellEquation(ParametricSeries seriesA, ParametricSeries seriesB)
        {
            var floats = new float[seriesA.VectorSize];
	        float a = seriesB[0] - seriesA[0];
	        float b = seriesB[1] - seriesA[1];
	        float radAngle = (float)Math.Atan2(b, a);

            floats[0] = (float)(Math.Sin(radAngle) / Math.PI + 0.5f);
	        floats[1] = (float)(Math.Cos(radAngle) / Math.PI + 0.5f);
	        return new ParametricSeries(seriesA.VectorSize, floats);
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


        //private delegate float FloatEquation(float a, float b); // todo: should be series's in the params
        //private static ParametricSeries GeneralEquation(ParametricSeries seriesA, ParametricSeries seriesB, FloatEquation floatEquation)
        //{
	       // // note: so far parametricSeries can only have one vector, so this is redundant atm.
	       // ParametricSeries result;
	       // var aVals = seriesA.FloatData;
	       // var bVals = seriesB.FloatData;
	       // if (aVals.Length == bVals.Length)
	       // {
		      //  // Faster probable version.
		      //  var resultArray = new float[seriesA.Count];
		      //  for (int i = 0; i < seriesA.Count; i += 2) // todo: generalize to wider vectorSize
		      //  {
			     //   resultArray[i] = floatEquation(bVals[i] - aVals[i], bVals[i + 1] - aVals[i + 1]);
		      //  }
		      //  result = new ParametricSeries(seriesA.VectorSize, resultArray);
	       // }
	       // else
	       // {
		      //  // These are differently shaped, so need to query each one as some values will be virtual.
		      //  int widest = seriesA.VectorSize >= seriesB.VectorSize ? seriesA.VectorSize : seriesB.VectorSize;
		      //  var resultArray = new float[widest];
		      //  for (int j = 0; j < widest; j++)
		      //  {
			     //   float af = seriesA[j]; //a.FloatDataAt(j);
			     //   float bf = seriesB[j]; //b.FloatDataAt(j);

			     //   resultArray[j] = floatEquation(af, bf);
		      //  }

		      //  result = new ParametricSeries(widest, resultArray);
	       // }

	       // return result;
        //}

    }
}
