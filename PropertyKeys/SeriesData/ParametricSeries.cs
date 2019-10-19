using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.SeriesData
{
    public class ParametricSeries : FloatSeries
    {
        public ParametricSeries(int vectorSize, params float[] values) : base(vectorSize, values)
        {
	        if (_floatValues.Length > VectorSize)
	        {
		        var old = _floatValues;
				_floatValues = new float[VectorSize];
				Array.Copy(old, _floatValues, VectorSize);
	        }
        }

        public float this[int index] => _floatValues[index];
    }
}
