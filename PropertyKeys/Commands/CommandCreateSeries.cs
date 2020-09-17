using System;
using System.Drawing;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Commands
{
    public class CommandCreateSeries : CommandBase
    {
	    private Series Series;
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
			        Series = new FloatSeries(_vectorSize, _floatValues);
			        break;
		        case SeriesType.Parametric:
			        Series = new ParametricSeries(_vectorSize, _floatValues);
			        break;
		        case SeriesType.RectF:
			        Series = new RectFSeries(_floatValues);
			        break;
		        case SeriesType.Int:
			        Series = new IntSeries(_vectorSize, _intValues);
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
