using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Simulators;
using DataArcs.Components.Simulators.Automata;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class AutomataTest : ITestScreen
    {
	    private readonly Player _player;
	    int _versionIndex = -1;
	    int _versionCount = 4;
	    private Timer _timer;
	    private MouseInput _mouseInput;

        AutomataComposite _current;

	    GridSampler Grid => new GridSampler(new int[] { 85, 60 });
	    HexagonSampler Hex => new HexagonSampler(new int[] { 75, 50 });
	    //RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });

        public AutomataTest(Player player)
	    {
		    _player = player;
		    _player.Pause();

		    _mouseInput = new MouseInput();
			_mouseInput.MouseClick = NextBlock;
		    _player.AddActiveElement(_mouseInput);
        }

        public void NextBlock()
        {
			_current.Runner.NextBlock();
        }
	    public void NextVersion()
	    {
		    _player.Pause();

		    _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
		    _player.Clear();

		    switch (_versionIndex)
		    {
			    case 0:
                    _current = GetAutomata(Hex);
                    //_current = GetAutomata(Grid);
                    //_currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
                    _player.AddActiveElement(_current);
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

		    _player.Unpause();
	    }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
        }

        private AutomataComposite GetAutomata(Sampler sampler)
	    {
		    Store itemStore = Store.CreateItemStore(sampler.Capacity);


            var automataStore = new Store(new FloatSeries(3, 0f,0f,0f), sampler);
			automataStore.BakeData();
            //automataStore.GetSeriesRef().SetRawDataAt(575, new FloatSeries(3, 0f,0f,.7f));

		    var runner = new Runner(automataStore);
            CreateBlock0(runner);
            CreateBlock1(runner);
            CreateBlock2(runner);
            CreateBlock3(runner);
            runner.ActiveIndex = 0;

			var composite = new AutomataComposite(itemStore, automataStore, runner);

			var ls = new LinkingStore(composite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
		    composite.AddProperty(PropertyId.FillColor, ls);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 6.8f).Store);
		    //var radStore = new Store(new FloatSeries(1, 6.5f, 6.5f, 9f, 9f), new LineSampler(), CombineFunction.Multiply);
		    //var radiusLink = new LinkingStore(composite.CompositeId, PropertyId.Automata, new[] { Slot.Max }, radStore);
		    //composite.AddProperty(PropertyId.Radius, radiusLink);

            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);

            //composite.AddProperty(PropertyId.Orientation, new FloatSeries(1, 0f).Store);
            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            composite.Renderer = new PolyShape();
			
		    Store loc = new Store(MouseInput.MainFrameRect.Outset(-0f), sampler);
		    composite.AppendProperty(PropertyId.Location, loc);
		    return composite;
        }


        private RuleSet CreateBlock0(Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            
            rules.BeginPass = () =>
            {
                //Debug.WriteLine(runnerParam.LastInvokedRule);
            };

            rules.ResetFn = (Runner runner) =>
            {
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetSeriesRef().SetRawDataAt(index, new FloatSeries(3, 1f, 1f, 1f));
            };


            rules.AddRule(Rule.PassCountIsUnder(30), Rule.ConstInterpFn(Colors.Black, 0.8f));

            rules.AddRule(Rule.RandomChance(0.0004f), Rule.ConstInterpFn(Colors.Cyan, 0.1f));

            rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.Sum, Slot.Y,5.99f),
                Rule.ConstInterpFn(Colors.Red, 1f));
            rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Min, Slot.X, .0001f),
                Rule.ConstInterpFn(Colors.White, .1f));
            rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.MaxDiff, Slot.X, .15f),
                Rule.ConstInterpFn(Colors.Black, .208f));
            rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.Max, Slot.X, 0.9f),
                Rule.ConstInterpFn(Colors.DarkBlue, .6f));
            rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.X, 1f),
                Rule.ConstInterpFn(Colors.Yellow, .19f));



            //rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.Average, Slot.X, 0.51f),
            //    Rule.ConstInterpFn(Colors.DarkRed, .4f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.X, 0.49f),
            //    Rule.ConstInterpFn(Colors.Pink, .6f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.X, 1f),
            //    Rule.ConstInterpFn(Colors.MidBlue, 1f));

            //rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.Average, Slot.X, 0.51f),
            //    Rule.ConstInterpFn(Colors.Black, 1f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.X, 0.49f),
            //    Rule.ConstInterpFn(Colors.LightGray, .1f));
            //rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.X, 1f),
            //    Rule.ConstInterpFn(Colors.White, 1f));


            runnerParam.AddRuleSet(rules);
            rules.Reset(runnerParam);

            return rules;
        }


        private RuleSet CreateBlock1(Runner runnerParam)
        {
	        RuleSet rules = new RuleSet();

	        float perPassRnd = 0;
	        rules.BeginPass = () => { perPassRnd = (float) SeriesUtils.Random.NextDouble(); };

            rules.AddRule(Rule.PassCountIsUnder(30), Darken);
	        rules.AddRule(Rule.RandomChance(0.00002f), Rule.RandomColorFn(0.1f, .9f, .4f,.7f, .4f, .7f) );
            var isBrightTest = Rule.CurrentValueIsOver(Slot.Max, 0.9f);
	        var dimStars = Rule.AllConditionsTrue(
	            Rule.RandomChance(0.005f),
		        isBrightTest
            );
            rules.AddRule(dimStars, Rule.ConstInterpFn(Colors.Black, 0.05f));
            rules.AddRule(isBrightTest, Rule.NopFn());
            rules.AddRule(Rule.NeighboursEvaluationIsNear(SeriesUtils.Max, Slot.Y, Slot.Z, 0.09f), Rule.CombineFn(Rule.ShuffleValuesFn(), Rule.ConstInterpFn(Colors.Black, 0.005f)));
            rules.AddRule(Rule.CurrentValueIsUnder(Slot.Min, 0.35f), Rule.EvalInterpFn(SeriesUtils.Max, 0.07f));
            rules.AddRule(Rule.RandomChance(0.004f), Rule.ConstInterpFn(Colors.Cyan, 0.1f));
            rules.AddRule(Rule.NeighboursEvaluationIsNear(SeriesUtils.Max, Slot.X, Slot.Y, 0.06f), Rule.ConstInterpFn(Colors.Red, 0.07f));
            rules.AddRule(Rule.CurrentValueIsOver(Slot.Average, 0.2f), Rule.EvalInterpFn(SeriesUtils.Min, 0.5f));

	        runnerParam.AddRuleSet(rules);
	        rules.Reset(runnerParam);

	        return rules;
        }

        private RuleSet CreateBlock2(Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            rules.TransitionSpeed = 0.05f;
            float perPassRnd = 0;

            rules.BeginPass = () =>
            {
                perPassRnd = (float)SeriesUtils.Random.NextDouble();
            };
            rules.ResetFn = (Runner runner) => perPassRnd = 0;
            rules.AddRule(Rule.PassCountIsUnder(30), Darken);
            rules.AddRule(Rule.RandomChance(0.001f), RandomAny);
            rules.AddRule((currentValue, target, runner) => perPassRnd < 0.01, DarkenSmall);
            rules.AddRule(Rule.CurrentValueIsNear(Slot.X, Slot.Y, 0.005f), RandomDark00_30);
            rules.AddRule(Rule.CurrentValueIsUnder(Slot.Y, 0.3f), MaxNeighbor02);
            rules.AddRule(Rule.CurrentValueIsOver(Slot.Z, 0.2f), MinNeighbor99);

            runnerParam.AddRuleSet(rules);
			rules.Reset(runnerParam);

            return rules;
        }

        private RuleSet CreateBlock3(Runner runnerParam)
        {
            RuleSet rules = new RuleSet();
            Condition cond;

            ParameterizedFunction swapFn1 = Average01;
            ParameterizedFunction swapFn2 = RandomMid30_70;

            rules.BeginPass = () =>
            {
                //perPassRnd = (float)SeriesUtils.Random.NextDouble();
            };

            rules.ResetFn = (Runner runner) =>
            {
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetSeriesRef().SetRawDataAt(index, new FloatSeries(3, 0.1f, 0.1f, .7f));
            };
            rules.AddRule(Rule.PassCountIsUnder(40), DarkenSmall);
            rules.AddRule(Rule.AllConditionsTrue(Rule.PassCountIsUnder(42), Rule.RandomChance(0.003f)), 
	            Rule.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));
            rules.AddRule(Rule.PassCountIsUnder(45), DarkenSmall);
            rules.AddRule(Rule.AllConditionsTrue(Rule.PassCountIsUnder(46), Rule.RandomChance(0.0003f)),
	            
	            Rule.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));
            rules.AddRule(Rule.RandomChance(0.00001f), RandomAny);
            rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.Max, Slot.Z, 0.99f), Rule.SetSeriesAtIndexFn(0, Colors.Black));
            var act = Rule.AllConditionsTrue(
	            Rule.NeighboursEvaluationIsOver(SeriesUtils.Max, Slot.Y, 0.99f),
	            Rule.NeighboursEvaluationIsUnder(SeriesUtils.Min, Slot.Y, 0.01f)
            );
            rules.AddRule(act, RandomAny);
            rules.AddRule(Rule.CurrentValueIsUnder(Slot.Z, 0.1f), AverageMax95);
            rules.AddRule(Rule.CurrentValueIsOver(Slot.X, 0.9f), swapFn1);
            rules.AddRule(Rule.CurrentValueIsFar(Slot.X, Slot.Y, 0.9f), swapFn2);
            var oct = Rule.OneConditionTrue(
	            Rule.CurrentValueIsUnder(Slot.Z, 0.1f),
	            Rule.CurrentValueIsOver(Slot.Z, 0.94f)
            );
            rules.AddRule(oct, (currentValue, target) => {
                var temp = swapFn1;
                swapFn1 = swapFn2;
                swapFn2 = temp;
                return currentValue;
            });
            rules.AddRule(Rule.NeighboursEvaluationIsUnder(SeriesUtils.Average, Slot.Y, 0.001f), Mix1);
            rules.AddRule(Rule.AlwaysTrue(), Mix1);

            runnerParam.AddRuleSet(rules);
            rules.Reset(runnerParam);

            return rules;
        }

        // experimental functions
        private static readonly ParameterizedFunction Darken = Rule.ConstInterpFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = Rule.ConstInterpFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = Rule.ConstInterpFn(Colors.White, 0.1f);

        private static readonly ParameterizedFunction Average01 = Rule.EvalInterpFn(SeriesUtils.Average, 0.1f);
        private static readonly ParameterizedFunction AverageMax95 = Rule.EvalInterpFn(SeriesUtils.Max, 0.95f);
        private static readonly ParameterizedFunction MaxNeighbor02 = Rule.EvalInterpFn(SeriesUtils.Max, 0.2f);
        private static readonly ParameterizedFunction MinNeighbor99 = Rule.EvalInterpFn(SeriesUtils.Min, 0.99f);

        private static readonly ParameterizedFunction Mix1 = Rule.ModifyResultsFn(
	        Rule.EvaluateNeighborsFn(Rule.MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.Average), SeriesUtils.ClampTo01);

        private static readonly ParameterizedFunction RandomAny = Rule.RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid30_70 = Rule.RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark00_30 = Rule.RandomColorFn(0.0f, 0.3f);
    }
}
