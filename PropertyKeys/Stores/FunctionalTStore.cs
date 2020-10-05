using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Stores
{
    public class FunctionalTStore : StoreBase
    {
	    private readonly BinaryFloatEquation _floatEquation;
	    private readonly BinaryFloatEquation _updateEquation;
        private float _internalT;
	    public FunctionalTStore(BinaryFloatEquation floatEquation, BinaryFloatEquation updateEquation = null) : base(FloatSeries.NormSeries)
	    {
		    CombineFunction = CombineFunction.ModifyT;
		    _floatEquation = floatEquation;
            _updateEquation = updateEquation;
        }

	    public override Series GetSeriesRef()
	    {
		    return Series;
	    }

	    public override void SetFullSeries(Series value)
	    {
		    Series = value;
	    }

	    public override Series GetValuesAtT(float t)
	    {
		    return new FloatSeries(1, _floatEquation.Invoke(t, _internalT));
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    int len = seriesT.VectorSize;
		    ParametricSeries result = new ParametricSeries(len, new float[len]);
		    for (int i = 0; i < len; i++)
		    {
			    result[i] = _floatEquation.Invoke(seriesT[i], _internalT);
		    }
		    return result;
        }

	    public override void Update(double currentTime, double deltaTime)
	    {
		    if (_updateEquation != null)
		    {
			    _internalT = _updateEquation.Invoke((float)deltaTime, _internalT);
		    }
	    }

	    public override void ResetData()
	    {
	    }

	    public override void BakeData()
	    {
	    }

	    public override IStore Clone()
	    {
			return new FunctionalTStore(_floatEquation);
	    }

	    public override void CopySeriesDataInto(IStore target)
	    {
		    throw new NotImplementedException();
	    }
    }
}
