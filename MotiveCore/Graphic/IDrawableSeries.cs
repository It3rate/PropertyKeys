using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.SeriesData;

namespace MotiveCore.Graphic
{
    public interface IDrawableSeries : ISeries
    {
	    GraphicsPath Path();
    }
}
