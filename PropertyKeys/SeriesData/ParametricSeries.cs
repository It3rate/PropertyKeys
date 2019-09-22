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
        }
    }
}
