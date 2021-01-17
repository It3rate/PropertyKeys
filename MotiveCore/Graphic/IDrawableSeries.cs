using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;

namespace Motive.Graphic
{
    public interface IDrawableSeries : ISeries
    {
        // todo: look at passing around the GraphicsPath rather than generating a new one each time.
	    void AppendToGraphicsPath(GraphicsPath path);
    }
}
