using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Components.Simulators.Automata
{
	public class RuleSet
	{
		public List<Rule> Rules { get; } = new List<Rule>();
		public Slot[] SwizzleMap { get; set; }

		public float TransitionSpeed { get; set; } = 1f;
        public int InvokedRule { get; private set; }

        public void AddRule(Condition condition, ParameterizedFunction fn) => Rules.Add(new Rule(condition, fn));
		public Action BeginPass { get; set; }

        public Series InvokeRules(Series currentValue, Series neighbors, Runner runner)
        {
            InvokedRule = -1;
			for (int i = 0; i < Rules.Count; i++)
			{
				if (!Rules[i].Invoke(currentValue, neighbors, runner))
                {
                    InvokedRule = i;
                    break;
				}
			}
			return SeriesUtils.SwizzleSeries(SwizzleMap, currentValue);
        }
		private Action<Runner> _reset;
		public Action<Runner> ResetFn
		{
			get => _reset;
			set => _reset = value;
		}

		public void Reset(Runner runner)
		{
			_reset?.Invoke(runner);
		}

	}

}
