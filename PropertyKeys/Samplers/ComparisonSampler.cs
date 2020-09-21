using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
	public delegate ParametricSeries SeriesEquation(ParametricSeries a, ParametricSeries b, ParametricSeries c = null);

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
        Difference,
        Average,
        RightIsGreater,
        RightIsLess,
        Bubble,
        SignedDistance, // returns xDist, yDist
	}

    public class ComparisonSampler : Sampler
    {
	    private Sampler _sampleA;
	    private Sampler _sampleB;
	    private SeriesEquation _seriesEquation;
		public ParametricSeries EffectRatio { get; set; }

        public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquation seriesEquation, Slot[] swizzleMap = null, int sampleCount = 1) : base(swizzleMap, sampleCount)
        {
		    _sampleA = sampleA;
		    _sampleB = sampleB;
		    _seriesEquation = seriesEquation;
		    SampleCount = sampleCount;
	    }
	    public ComparisonSampler(Sampler sampleA, Sampler sampleB, SeriesEquationType seriesEquationType, Slot[] swizzleMap = null, int sampleCount = 1) : 
		    this(sampleA, sampleB, GetSeriesEquationByType(seriesEquationType), swizzleMap, sampleCount){ }
		
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    var resultA = _sampleA.GetSampledTs(seriesT);
		    var resultB = _sampleB.GetSampledTs(seriesT);
		    var result = _seriesEquation(resultA, resultB, EffectRatio);
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
		        case SeriesEquationType.Difference:
			        result = DifferenceEquation;
			        break;
		        case SeriesEquationType.Average:
			        result = AverageEquation;
			        break;
		        case SeriesEquationType.RightIsGreater:
			        result = RightIsGreaterEquation;
			        break;
		        case SeriesEquationType.RightIsLess:
			        result = RightIsLessEquation;
			        break;
                case SeriesEquationType.Bubble:
			        result = BubbleEquation;
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

        private static ParametricSeries DefaultEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return seriesA;
        }

        private static ParametricSeries DistanceEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        var totals = GeneralEquation(seriesA, seriesB, (a, b) => (b - a) * (b - a));
			return new ParametricSeries(1, (float)Math.Sqrt(totals.FloatDataRef.Sum()));
        }

        private static ParametricSeries SignedDistanceEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) =>
	        {
		        float dif = b - a;
		        return 1f - ((dif * dif * dif + 1f) * 0.5f);
	        });
        }
        private static ParametricSeries DifferenceEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => b - a);
        }
        private static ParametricSeries AverageEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => (a + b) / 2.0f);
        }
        private static ParametricSeries RightIsGreaterEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => b > a ? 1f : 0);
        }
        private static ParametricSeries RightIsLessEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        return GeneralEquation(seriesA, seriesB, (a, b) => b < a ? 1f : 0);
        }
        private static ParametricSeries PolarEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries c = null)
        {
	        float a = seriesB[0] - seriesA[0];
	        float b = seriesB[1] - seriesA[1];
	        if (c != null)
	        {
		        a *= c.X;
		        b *= c.Y;
	        }
	        float radAngle = (float)Math.Atan2(b, a);
	        float normAngle = radAngle / (float)(2 * Math.PI);
	        normAngle = 1f - normAngle;
	        normAngle = normAngle > 1 ? normAngle - 1f : normAngle;
	        float dist = 1f - (float)Math.Sqrt(a * a + b * b);// * 0.7071f;
	        return new ParametricSeries(2, dist, normAngle);
        }

        private static ParametricSeries BubbleEquation(ParametricSeries seriesA, ParametricSeries seriesB, ParametricSeries effectRadius = null)
        {
	        float a = seriesB[0] - seriesA[0];
	        float b = seriesB[1] - seriesA[1];
	        float radAngle = (float)Math.Atan2(b, a);
	        float dist = 1f - (float)Math.Sqrt(a * a + b * b);
	        float xDist = dist;
	        float yDist = dist;

	        if (effectRadius != null)
	        {
		        float clampRatioX = effectRadius.X;
		        float clampRatioY = effectRadius.Y;
		        xDist = Math.Max(0, dist - (1f - clampRatioX)) * (1f / clampRatioX);
		        yDist = Math.Max(0, dist - (1f - clampRatioY)) * (1f / clampRatioY);
	        }

	        float x = (float)(Math.Cos(radAngle) * xDist) * 0.5f + 0.5f;
	        float y = (float)(Math.Sin(radAngle) * yDist) * 0.5f + 0.5f;

	        return new ParametricSeries(2, x, y);
        }




        private static IntSeries GeneralEquation(IntSeries seriesA, IntEquation intEquation)
        {
	        var ints = new int[seriesA.VectorSize];
	        for (int i = 0; i < ints.Length; i++)
	        {
		        ints[i] = intEquation(seriesA.IntDataAt(i));
	        }
	        return new IntSeries(ints.Length, ints);
        }
        private static FloatSeries GeneralEquation(FloatSeries seriesA, FloatEquation floatEquation)
        {
	        var floats = new float[seriesA.VectorSize];
	        for (int i = 0; i < floats.Length; i++)
	        {
		        floats[i] = floatEquation(seriesA.FloatDataAt(i));
	        }
	        return new FloatSeries(floats.Length, floats);
        }

        private static IntSeries GeneralEquation(IntSeries seriesA, IntSeries seriesB, BinaryIntEquation binaryIntEquation)
        {
	        int max = Math.Max(seriesA.VectorSize, seriesB.VectorSize);

	        var ints = new int[max];
	        for (int i = 0; i < max; i++)
	        {
		        ints[i] = i >= seriesA.VectorSize ? seriesB.IntDataAt(i) : i >= seriesB.VectorSize ? seriesA.IntDataAt(i) : binaryIntEquation(seriesB.IntDataAt(i), seriesA.IntDataAt(i));
	        }
	        return new IntSeries(max, ints);
        }

        private static ParametricSeries GeneralEquation(ParametricSeries seriesA, ParametricSeries seriesB, BinaryFloatEquation binaryFloatEquation)
        {
	        int max = Math.Max(seriesA.VectorSize, seriesB.VectorSize);

	        var floats = new float[max];
	        for (int i = 0; i < max; i++)
	        {
		        floats[i] = i >= seriesA.VectorSize ? seriesB[i] : i >= seriesB.VectorSize ? seriesA[i] : binaryFloatEquation(seriesB[i], seriesA[i]);
	        }
	        return new ParametricSeries(max, floats);
        }
    }
}
