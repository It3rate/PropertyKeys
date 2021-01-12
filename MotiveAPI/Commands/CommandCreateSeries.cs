using System;
using System.Drawing;
using MotiveCore.SeriesData.Utils;
using MotiveCore.SeriesData;

namespace MotiveCore.Commands
{
    public class CommandCreateSeries : CommandBase
    {
	    private Series _series;
	    private readonly SeriesType _type;
	    private readonly int _vectorSize;
	    private readonly float[] _floatValues;
	    private readonly int[] _intValues;

        public CommandCreateSeries(SeriesType type, int vectorSize, params float[] values)
        {
	        this._type = type;
	        this._vectorSize = vectorSize;
            if (type == SeriesType.Int)
		    {
			    _intValues = values.ToInt();
		    }
            else
            {
	            _floatValues = values;
            }
	    }
	    public CommandCreateSeries(SeriesType type, int vectorSize, params int[] values)
	    {
		    this._type = type;
		    this._vectorSize = vectorSize;
		    if (type != SeriesType.Int)
		    {
			    _floatValues = values.ToFloat();
		    }
		    else
		    {
			    _intValues = values;
		    }
        }

        public override void Execute()
        {
	        switch (_type)
	        {
		        case SeriesType.Float:
			        _series = new FloatSeries(_vectorSize, _floatValues);
			        break;
		        case SeriesType.Parametric:
			        _series = new ParametricSeries(_vectorSize, _floatValues);
			        break;
		        case SeriesType.RectF:
			        _series = new RectFSeries(_floatValues);
			        break;
		        case SeriesType.Int:
			        _series = new IntSeries(_vectorSize, _intValues);
			        break;
	        }
        }

        public override void UnExecute()
        {
            throw new NotImplementedException();
        }

        public override void Update(double time)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Graphics graphics)
        {
            throw new NotImplementedException();
        }
    }
}
