using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Adapters.Color;
using Motive.Components;
using Motive.Components.ExternalInput;
using Motive.Components.Simulators.Automata;
using Motive.Graphic;
using Motive.Samplers;
using Motive.SeriesData;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Components.Simulators;
using Motive.Components.Transitions;

namespace Motive.Tests.GraphicTests
{
    public class AutomataTest : ITestScreen
    {
	    private readonly Runner _runner;
	    int _versionIndex = -1;
	    int _versionCount = 4;
	    //private Timer _timer;
	    private MouseInput _mouseInput;

        AutomataComposite _current;

	    GridSampler Grid => new GridSampler(new int[] { 85, 60 });
	    HexagonSampler Hex => new HexagonSampler(new int[] { 75, 50 });
	    //RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });

        public AutomataTest(Runner runner)
	    {
		    _runner = runner;
		    _runner.Pause();

		    _mouseInput = new MouseInput();
			_mouseInput.MouseClick = NextBlock;
		    _runner.ActivateComposite(_mouseInput.Id);
        }

        public void NextBlock()
        {
			_current.Runner.NextBlock();
        }
	    public void NextVersion()
	    {
		    _runner.Pause();

		    _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
		    _runner.Clear();

		    switch (_versionIndex)
		    {
			    case 0:
                    _current = GetAutomata(Hex);
                    //_current = GetAutomata(Grid);
                    //_currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
                    _runner.ActivateComposite(_current.Id);
				    break;
			    case 1:
				    break;
			    case 2:
				    break;
			    case 3:
				    break;
			    case 4:
				    break;
		    }

		    _runner.Unpause();
	    }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
        }

        private AutomataComposite GetAutomata(Sampler sampler)
	    {
		    Store itemStore = Store.CreateItemStore(sampler.SampleCount);
			itemStore.BakeData();
			
            var automataStore = new Store(new FloatSeries(3, 0f,0f,0f), sampler);
			automataStore.BakeData();
            //automataStore.GetSeriesRef().SetSeriesAt(575, new FloatSeries(3, 0f,0f,.7f));

		    var runner = new Components.Simulators.Automata.Runner(automataStore);
		    CreateHealingCrystal(runner);
		    CreateAlternatingGrid(runner);
            CreateBlock1(runner);
            CreateBlock2(runner);
            CreateBlock3(runner);
            runner.ActiveIndex = 0;

			var composite = new AutomataComposite(itemStore, automataStore, runner);

			var ls = new LinkingStore(composite.Id, PropertyId.Automata, SlotUtils.XYZ, null);
		    composite.AddProperty(PropertyId.FillColor, ls);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 6.8f).Store());
		    //var radStore = new Store(new FloatSeries(1, 6.5f, 6.5f, 9f, 9f), new LinearSampler(), CombineFunction.Multiply);
		    //var radiusLink = new LinkingStore(composite.Id, PropertyId.Automata, new[] { Slot.MaxSlots }, radStore);
		    //composite.AddProperty(PropertyId.Radius, radiusLink);

            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store());

            //composite.AddProperty(PropertyId.Orientation, new FloatSeries(1, 0f).Store);
            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store());
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store());
            composite.Renderer = new PolyShape();
			
		    Store loc = new Store(Runner.MainFrameRect.Outset(-0f), sampler);
		    composite.AppendProperty(PropertyId.Location, loc);
		    return composite;
        }


        private RuleSet CreateHealingCrystal(Components.Simulators.Automata.Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            
            rules.BeginPass = () => {  };

            rules.ResetFn = (Components.Simulators.Automata.Runner runner) =>
            {
	            runnerParam.UpdateValuesAfterPass = false;
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetSeriesRef().SetSeriesAt(index, new FloatSeries(3, 1f, 1f, 1f));
            };

            rules.AddRule(RuleCondition.PassCountIsUnder(30), RuleProduction.ConstInterpFn(Colors.Black, 0.8f));
            rules.AddRule(RuleCondition.PassCountIsUnder(31), RuleProduction.RandomColorFn(0, 1));

            //rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.AverageSlots, Slot.X, 0.51f),
            //    Rule.ConstInterpFn(Colors.DarkRed, .4f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.AverageSlots, Slot.X, 0.49f),
            //    Rule.ConstInterpFn(Colors.Pink, .6f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.AverageSlots, Slot.X, 1f),
            //    Rule.ConstInterpFn(Colors.MidBlue, 1f));

            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.AverageSlots, Slot.X, 0.51f),
                RuleProduction.ConstInterpFn(Colors.Black, 1f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.AverageSlots, Slot.X, 0.49f),
                RuleProduction.ConstInterpFn(Colors.LightGray, .1f));
            rules.AddRule(RuleCondition.AlwaysTrue(),
                RuleProduction.ConstInterpFn(Colors.White, 1f));

            runnerParam.AddRuleSet(rules);

            return rules;
        }
        private RuleSet CreateAlternatingGrid(Components.Simulators.Automata.Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            rules.BeginPass = () => {};
            rules.ResetFn = (Components.Simulators.Automata.Runner runner) => {
	            int index = (int)(runner.Automata.Capacity / 2.13f);
	            runner.Automata.GetSeriesRef().SetSeriesAt(index, new FloatSeries(3, 1f, 1f, 1f));
            };

            rules.AddRule(RuleCondition.PassCountIsUnder(30), RuleProduction.ConstInterpFn(Colors.Black, 0.8f));

            rules.AddRule(RuleCondition.RandomChance(0.0004f), RuleProduction.ConstInterpFn(Colors.Cyan, 0.1f));

            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.SumSlots, Slot.Y, 5.99f),
                RuleProduction.ConstInterpFn(Colors.Red, 1f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.MinSlots, Slot.X, .0001f),
                RuleProduction.ConstInterpFn(Colors.White, .1f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.MaxDiffSlots, Slot.X, .15f),
	            (currentValue, neighbors) => 
		            SeriesUtils.InterpolateInto(currentValue, Colors.Black, .208f));// Rule.ConstInterpFn(Colors.Black, .208f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.X, 0.9f),
                RuleProduction.ConstInterpFn(Colors.DarkBlue, .6f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.AverageSlots, Slot.X, 1f),
                RuleProduction.ConstInterpFn(Colors.Yellow, .19f));
            
            runnerParam.AddRuleSet(rules);

            return rules;
        }


        private RuleSet CreateBlock1(Components.Simulators.Automata.Runner runnerParam)
        {
	        RuleSet rules = new RuleSet();

	        float perPassRnd = 0;
	        rules.BeginPass = () =>
	        {
		        perPassRnd = (float) SeriesUtils.Random.NextDouble();
	        };

            rules.AddRule(RuleCondition.PassCountIsUnder(30), Darken);
	        rules.AddRule(RuleCondition.RandomChance(0.00002f), RuleProduction.RandomColorFn(0.1f, .9f, .4f,.7f, .4f, .7f) );
            var isBrightTest = RuleCondition.CurrentValueIsOver(Slot.Max, 0.9f);
	        var dimStars = RuleCondition.AllConditionsTrue(
	            RuleCondition.RandomChance(0.005f),
		        isBrightTest
            );
            rules.AddRule(dimStars, RuleProduction.ConstInterpFn(Colors.Black, 0.05f));
            rules.AddRule(isBrightTest, RuleProduction.NopFn());
            rules.AddRule(RuleCondition.NeighboursEvaluationIsNear(SeriesUtils.MaxSlots, Slot.Y, Slot.Z, 0.09f), RuleProduction.CombineFn(RuleProduction.ShuffleValuesFn(), RuleProduction.ConstInterpFn(Colors.Black, 0.005f)));
            rules.AddRule(RuleCondition.CurrentValueIsUnder(Slot.Min, 0.35f), RuleProduction.EvalInterpFn(SeriesUtils.MaxSlots, 0.07f));
            rules.AddRule(RuleCondition.RandomChance(0.004f), RuleProduction.ConstInterpFn(Colors.Cyan, 0.1f));
            rules.AddRule(RuleCondition.NeighboursEvaluationIsNear(SeriesUtils.MaxSlots, Slot.X, Slot.Y, 0.06f), RuleProduction.ConstInterpFn(Colors.Red, 0.07f));
            rules.AddRule(RuleCondition.CurrentValueIsOver(Slot.Average, 0.2f), RuleProduction.EvalInterpFn(SeriesUtils.MinSlots, 0.5f));

	        runnerParam.AddRuleSet(rules);

	        return rules;
        }

        private RuleSet CreateBlock2(Components.Simulators.Automata.Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            rules.TransitionSpeed = 0.05f;
            float perPassRnd = 0;

            rules.BeginPass = () =>
            {
                perPassRnd = (float)SeriesUtils.Random.NextDouble();
            };
            rules.ResetFn = (Components.Simulators.Automata.Runner runner) => perPassRnd = 0;
            rules.AddRule(RuleCondition.PassCountIsUnder(30), Darken);
            rules.AddRule(RuleCondition.RandomChance(0.001f), RandomAny);
            rules.AddRule((currentValue, target, runner) => perPassRnd < 0.01, DarkenSmall);
            rules.AddRule(RuleCondition.CurrentValueIsNear(Slot.X, Slot.Y, 0.005f), RandomDark00_30);
            rules.AddRule(RuleCondition.CurrentValueIsUnder(Slot.Y, 0.3f), MaxNeighbor02);
            rules.AddRule(RuleCondition.CurrentValueIsOver(Slot.Z, 0.2f), MinNeighbor99);

            runnerParam.AddRuleSet(rules);

            return rules;
        }

        private RuleSet CreateBlock3(Components.Simulators.Automata.Runner runnerParam)
        {
            RuleSet rules = new RuleSet();

            ParameterizedFunction swapFn1 = Average01;
            ParameterizedFunction swapFn2 = RandomMid30_70;

            rules.BeginPass = () =>
            {
                //perPassRnd = (float)SeriesUtils.Random.NextDouble();
            };

            rules.ResetFn = (Components.Simulators.Automata.Runner runner) =>
            {
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetSeriesRef().SetSeriesAt(index, new FloatSeries(3, 0.1f, 0.1f, .7f));
            };
            rules.AddRule(RuleCondition.PassCountIsUnder(40), DarkenSmall);
            rules.AddRule(RuleCondition.AllConditionsTrue(RuleCondition.PassCountIsUnder(42), RuleCondition.RandomChance(0.003f)), 
	            RuleProduction.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));
            rules.AddRule(RuleCondition.PassCountIsUnder(45), DarkenSmall);
            rules.AddRule(RuleCondition.AllConditionsTrue(RuleCondition.PassCountIsUnder(46), RuleCondition.RandomChance(0.0003f)),
	            
	            RuleProduction.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));
            rules.AddRule(RuleCondition.RandomChance(0.00001f), RandomAny);
            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.Z, 0.99f), RuleProduction.SetSeriesAtIndexFn(0, Colors.Black));
            var act = RuleCondition.AllConditionsTrue(
	            RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.Y, 0.99f),
	            RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.MinSlots, Slot.Y, 0.01f)
            );
            rules.AddRule(act, RandomAny);
            rules.AddRule(RuleCondition.CurrentValueIsUnder(Slot.Z, 0.1f), AverageMax95);
            rules.AddRule(RuleCondition.CurrentValueIsOver(Slot.X, 0.9f), swapFn1);
            rules.AddRule(RuleCondition.CurrentValueIsFar(Slot.X, Slot.Y, 0.9f), swapFn2);
            var oct = RuleCondition.OneConditionTrue(
	            RuleCondition.CurrentValueIsUnder(Slot.Z, 0.1f),
	            RuleCondition.CurrentValueIsOver(Slot.Z, 0.94f)
            );
            rules.AddRule(oct, (currentValue, target) => {
                var temp = swapFn1;
                swapFn1 = swapFn2;
                swapFn2 = temp;
                return currentValue;
            });
            rules.AddRule(RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.AverageSlots, Slot.Y, 0.001f), Mix1);
            rules.AddRule(RuleCondition.AlwaysTrue(), Mix1);

            runnerParam.AddRuleSet(rules);

            return rules;
        }

        // experimental functions
        private static readonly ParameterizedFunction Darken = RuleProduction.ConstInterpFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = RuleProduction.ConstInterpFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = RuleProduction.ConstInterpFn(Colors.White, 0.1f);

        private static readonly ParameterizedFunction Average01 = RuleProduction.EvalInterpFn(SeriesUtils.AverageSlots, 0.1f);
        private static readonly ParameterizedFunction AverageMax95 = RuleProduction.EvalInterpFn(SeriesUtils.MaxSlots, 0.95f);
        private static readonly ParameterizedFunction MaxNeighbor02 = RuleProduction.EvalInterpFn(SeriesUtils.MaxSlots, 0.2f);
        private static readonly ParameterizedFunction MinNeighbor99 = RuleProduction.EvalInterpFn(SeriesUtils.MinSlots, 0.99f);

        private static readonly ParameterizedFunction Mix1 = RuleProduction.ModifyResultsFn(
	        RuleProduction.EvaluateNeighborsFn(RuleProduction.MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.AverageSlots), SeriesUtils.ClampTo01Slots);

        private static readonly ParameterizedFunction RandomAny = RuleProduction.RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid30_70 = RuleProduction.RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark00_30 = RuleProduction.RandomColorFn(0.0f, 0.3f);
    }
}
