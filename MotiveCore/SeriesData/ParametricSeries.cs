using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData.Utils;

namespace Motive.SeriesData
{
    public class ParametricSeries : FloatSeries
    {
	    public override SeriesType Type => SeriesType.Parametric;

        public ParametricSeries(int vectorSize, params float[] values) : base(vectorSize, values)
        {
            //if (_floatValues.Length > VectorSize)
            //{
            //    var old = _floatValues;
            //    _floatValues = new float[VectorSize];
            //    Array.Copy(old, _floatValues, VectorSize);
            //}
        }

        public float this[int index]
        {
	        get => FloatValueAt(index);// index < _floatValues.Length ? _floatValues[index] : _floatValues[_floatValues.Length - 1];
	        set => _floatValues[index < _floatValues.Length ? index : _floatValues.Length - 1] = value;
        }

        public float MinValue => _floatValues.Min();
        public float MaxValue => _floatValues.Max();
        public float AverageValue => _floatValues.Average();
        public float SumValue => _floatValues.Sum();

        public override ISeries Copy()
        {
	        ParametricSeries result = new ParametricSeries(VectorSize, (float[])FloatDataRef.Clone());
	        return result;
        }
    }
}
