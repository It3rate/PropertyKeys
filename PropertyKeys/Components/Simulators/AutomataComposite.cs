using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators
{
    public class AutomataComposite : Container
    {
	    private IStore _automata;
	    private IStore _previousAutomata;
        public override int Capacity { get => _automata.Capacity; set { } }

        private Runner _runner1;
        private RuleSet _ruleSet1;
        private RuleSet _ruleSet2;

        public AutomataComposite(IStore itemStore, IStore automataStore) : base(itemStore)
	    {
		    _automata = automataStore;
		    _previousAutomata = _automata.Clone();
			AddProperty(PropertyId.Automata, _automata);

            _runner1 = new Runner(_automata);
            _ruleSet1 = CreateBlock1(_runner1);
            _ruleSet2 = CreateBlock2(_runner1);
        }

	    private int _delayCount = 0;
	    private ParameterizedFunction currentFn1 = Average1;
	    private ParameterizedFunction currentFn2 = RandomMid;
	    private bool isBusy = false;

	    bool block1 = false;
	    private int blockIndex = 0;
	    private int count = 50;


        public override void StartUpdate(float currentTime, float deltaTime)
	    {
		    if (!isBusy)
		    {
			    isBusy = true;
			    base.StartUpdate(currentTime, deltaTime);
			    _delayCount++;
			    if (true)//(_delayCount % 10 == 8)
			    {
				    if (SeriesUtils.Random.NextDouble() < 0.006 && count > 100)
                    {
                        _runner1.Reset();
                        block1 = !block1;
                        blockIndex = 0;
                        count = 0;
					    currentFn1 = Average1;
						currentFn2 = RandomMid;
					}

				    _runner1.PassCount++;
				    _runner1.GetRuleSet(0).BeginPass();
                    count++;
				    blockIndex += 100;

                    int capacity = Capacity;
                    _automata.CopySeriesDataInto(_previousAutomata);
				    float rnd0 = (float)SeriesUtils.Random.NextDouble();
                    for (int i = 0; i < capacity; i++)
				    {
					    var currentValue = _previousAutomata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
                        var neighbors = _previousAutomata.GetNeighbors(i);

                        _runner1.ActiveRuleSetIndex = (block1 && blockIndex > i) ? 0 : 1;

                        currentValue = _runner1.InvokeRuleSet(currentValue, neighbors, i);
                        _automata.GetFullSeries().SetSeriesAtIndex(i, currentValue);
                    }
			    }

			    isBusy = false;
		    }

	    }

        private RuleSet CreateBlock2(Runner runner)
        {
	        RuleSet rules = new RuleSet();
	        Condition cond;

			ParameterizedFunction swapFn1 = Average1;
			ParameterizedFunction swapFn2 = RandomMid;

			rules.BeginPass = () =>
	        {
		        //perPassRnd = (float)SeriesUtils.Random.NextDouble();
	        };

	        //rules.Reset = () => perPassRnd = 0;

	        cond = (currentValue, target) => runner.PassCount < 40;
	        rules.AddRule(cond, DarkenSmall);

	        cond = (currentValue, target) => count < 42 && SeriesUtils.Random.NextDouble() < 0.0003;
	        rules.AddRule(cond, RuleUtils.SetValueFn(0, new FloatSeries(3, 0, 0, 0.7f)) );

	        cond = (currentValue, target) => SeriesUtils.Random.NextDouble() < 0.00001;
	        rules.AddRule(cond, RandomAny);

	        cond = (currentValue, target) => target.Max().Z > 0.99;
	        rules.AddRule(cond, RuleUtils.SetValueFn(0, Colors.Black));

	        cond = (currentValue, target) => target.Max().Y > 0.99 && target.Min().Y < 0.01;
	        rules.AddRule(cond, RandomAny);

	        cond = (currentValue, target) => currentValue.Z < 0.1f;
	        rules.AddRule(cond, AverageMaxHigh);

	        cond = (currentValue, target) => currentValue.X > 0.90f;
	        rules.AddRule(cond, swapFn1);

	        cond = (currentValue, target) => Math.Abs(currentValue.X - currentValue.Y) > 0.9f;
	        rules.AddRule(cond, swapFn2);

	        cond = (currentValue, target) => currentValue.Z < 0.1f || currentValue.Z > 0.94f;
	        rules.AddRule(cond, (currentValue, target) => {
		        var temp = swapFn1;
		        swapFn1 = swapFn2;
		        swapFn2 = temp;
		        return currentValue;
	        });

	        cond = (currentValue, target) => target.Average().Y < 0.001f;
	        rules.AddRule(cond, Mix1);

	        cond = (currentValue, target) => true;
	        rules.AddRule(cond, Mix1);
			
            runner.AddRuleSet(rules);

	        return rules;
        }
        private RuleSet CreateBlock1(Runner runner)
        {
	        RuleSet rules = new RuleSet();
	        Condition cond;
	        float perPassRnd = 0;

	        rules.BeginPass = () =>
	        {
		        perPassRnd = (float)SeriesUtils.Random.NextDouble();
	        };
	        rules.Reset = () => perPassRnd = 0;

	        cond = (currentValue, target) => runner.PassCount < 20;
	        rules.AddRule(cond, Darken);

	        cond = (currentValue, target) => SeriesUtils.Random.NextDouble() < 0.001;
	        rules.AddRule(cond, RandomAny);

	        cond = (currentValue, target) => perPassRnd < 0.01;
	        rules.AddRule(cond, DarkenSmall);

	        cond = (currentValue, target) => Math.Abs(currentValue.X - currentValue.Y) < 0.005f;
	        rules.AddRule(cond, RandomDark);

	        cond = (currentValue, target) => currentValue.Y < 0.3;
	        rules.AddRule(cond, MaxNeighborPart);

	        cond = (currentValue, target) => currentValue.Z > 0.2;
	        rules.AddRule(cond, MinNeighborPart);

	        runner.AddRuleSet(rules);

	        return rules;
        }


        // experimental functions
        private static readonly ParameterizedFunction Darken = RuleUtils.InterpolateWithConstantFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = RuleUtils.InterpolateWithConstantFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = RuleUtils.InterpolateWithConstantFn(Colors.White, 0.1f);
        private static readonly ParameterizedFunction Average1 = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.1f), SeriesUtils.Average);
        private static readonly ParameterizedFunction AverageMaxHigh = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.95f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MaxNeighborPart = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.2f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MinNeighborPart = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.99f), SeriesUtils.Min);
        private static readonly ParameterizedFunction Mix1 = RuleUtils.ModifyResultsFn(
            RuleUtils.ModifyNeighborsFn(RuleUtils.MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.Average),SeriesUtils.ClampTo01);
        private static readonly ParameterizedFunction RandomAny = RuleUtils.RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid = RuleUtils.RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark = RuleUtils.RandomColorFn(0.0f, 0.3f);
    }
}
