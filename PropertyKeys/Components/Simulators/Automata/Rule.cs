using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators.Automata
{
	public delegate bool Condition(Series currentValue, Series neighbors);
	public delegate Series ParameterizedFunction(Series currentValue, Series neighbors);
	public class Rule
	{
		public Condition Condition { get; }
		public ParameterizedFunction ParameterizedFunction { get; }

		private CombineFunction CombineFunction { get; set; } = CombineFunction.Final;

        public Rule(Condition condition, ParameterizedFunction parameterizedFunction)
		{
			Condition = condition;
			ParameterizedFunction = parameterizedFunction;
		}

        public bool Invoke(Series currentValue, Series neighbors)
        {
	        bool result = false;
	        if (Condition(currentValue, neighbors))
	        {
		        result = CombineFunction == CombineFunction.Final;
		        var vals = ParameterizedFunction(currentValue, neighbors);
				currentValue.CombineInto(vals, CombineFunction);
	        }
	        return result;
        }
	}

}
