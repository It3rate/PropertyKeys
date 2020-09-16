using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Commands;
using DataArcs.SeriesData.Utils;

namespace DataArcs.SeriesData
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
	    }
	    public CommandCreateSeries(SeriesType type, int vectorSize, params int[] values)
	    {
		    this._type = type;
		    this._vectorSize = vectorSize;
		    if (type != SeriesType.Int)
		    {
			    _floatValues = values.ToFloat();
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
			// add to series library in Store (with option to 'export').
        }

        public override void UnExecute()
        {
            throw new NotImplementedException();
            // remove from series library in Store
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
