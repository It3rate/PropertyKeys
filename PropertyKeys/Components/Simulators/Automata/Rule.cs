using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;

namespace DataArcs.Components.Simulators.Automata
{
	public delegate bool Condition(Series currentValue, Series neighbors);
	public delegate Series ParameterizedFunction(Series currentValue, Series neighbors);
	public class Rule
	{
		public Condition Condition { get; }
		public ParameterizedFunction ParameterizedFunction { get; }

		public Rule(Condition condition, ParameterizedFunction parameterizedFunction)
		{
			Condition = condition;
			ParameterizedFunction = parameterizedFunction;
		}
	}

}
