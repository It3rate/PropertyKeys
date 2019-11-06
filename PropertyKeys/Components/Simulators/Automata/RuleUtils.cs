using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;

namespace DataArcs.Components.Simulators.Automata
{
    public static class RuleUtils
    {
	    public static Series Mix(Series source, Series target, ParametricSeries mix)
        {
            return new FloatSeries(source.VectorSize,
                source.X + (target.X - 0.5f) * mix.X,
                source.Y + (target.Y - 0.5f) * mix.Y,
                source.Z + (target.Z - 0.5f) * mix.Z);
        }
    }
}
