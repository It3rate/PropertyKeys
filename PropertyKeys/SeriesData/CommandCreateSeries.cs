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
	    private SeriesType type;
	    private int vectorSize;
	    private float[] floatValues;
	    private int[] intValues;

        public CommandCreateSeries(SeriesType type, int vectorSize, params float[] values)
        {
	        this.type = type;
	        this.vectorSize = vectorSize;
            if (type == SeriesType.Int)
		    {
			    intValues = values.ToInt();
		    }
	    }
	    public CommandCreateSeries(SeriesType type, int vectorSize, params int[] values)
	    {
		    this.type = type;
		    this.vectorSize = vectorSize;
		    if (type != SeriesType.Int)
		    {
			    floatValues = values.ToFloat();
		    }
        }

        public override void Execute()
        {
	        switch (type)
	        {
		        case SeriesType.Float:
			        Series = new FloatSeries(vectorSize, floatValues);
			        break;
		        case SeriesType.Parametric:
			        Series = new ParametricSeries(vectorSize, floatValues);
			        break;
		        case SeriesType.RectF:
			        Series = new RectFSeries(floatValues);
			        break;
		        case SeriesType.Int:
			        Series = new IntSeries(vectorSize, intValues);
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
