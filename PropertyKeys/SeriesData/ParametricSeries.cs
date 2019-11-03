using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.SeriesData
{
    public class ParametricSeries : FloatSeries
    {
	    public override SeriesType Type => SeriesType.Parametric;

        public ParametricSeries(int vectorSize, params float[] values) : base(vectorSize, values)
        {
	        if (_floatValues.Length > VectorSize)
	        {
		        var old = _floatValues;
				_floatValues = new float[VectorSize];
				Array.Copy(old, _floatValues, VectorSize);
	        }
        }

        public float this[int index]
        {
	        get => index < _floatValues.Length ? _floatValues[index] : _floatValues[_floatValues.Length - 1];
	        set => _floatValues[index < _floatValues.Length ? index : _floatValues.Length - 1] = value;
        }

        public override Series GetZeroSeries()
        {
	        return new ParametricSeries(VectorSize, SeriesUtils.GetFloatZeroArray(VectorSize));
        }

        public override Series GetZeroSeries(int elementCount)
        {
	        return SeriesUtils.GetZeroParametricSeries(VectorSize, elementCount);
        }

        public override Series GetMinSeries()
        {
	        return new ParametricSeries(VectorSize, new float[VectorSize]);
        }

        public override Series GetMaxSeries()
        {
	        var ar = new float[VectorSize];
	        for (int i = 0; i < ar.Length; i++)
	        {
		        ar[i] = 1f;
	        }
	        return new ParametricSeries(VectorSize, ar);
        }

        public override Series Copy()
        {
	        ParametricSeries result = new ParametricSeries(VectorSize, (float[])FloatDataRef.Clone());
	        return result;
        }
    }
}
