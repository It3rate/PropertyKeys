using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;

namespace DataArcs.Components.Simulators.Automata
{
	public class RuleSet
	{
		public List<Rule> Rules { get; } = new List<Rule>();
		public void AddRule(Condition condition, ParameterizedFunction fn) => Rules.Add(new Rule(condition, fn));
		public Action BeginPass { get; set; }
		public float TransitionSpeed { get; set; } = 1f;

        public Series InvokeRules(Series currentValue, Series neighbors)
		{
			Series result = currentValue;
			for (int i = 0; i < Rules.Count; i++)
			{
				if (Rules[i].Condition(currentValue, neighbors))
				{
					result = Rules[i].ParameterizedFunction(currentValue, neighbors);
					break;
				}
			}
			return result;
		}
		private Action _reset;
		public Action Reset { get => _reset; set { _reset = value; _reset.Invoke(); } }
	}

}
