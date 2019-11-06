using System;
using System.Collections.Generic;
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
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class AutomataTest : ITestScreen
    {
	    private readonly Player _player;
	    int _versionIndex = -1;
	    int _versionCount = 4;
	    private Timer _timer;
		
	    IContainer _current;

	    GridSampler Grid => new GridSampler(new int[] { 85, 60 });
	    HexagonSampler Hex => new HexagonSampler(new int[] { 75, 50 });
	    //RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });

        public AutomataTest(Player player)
	    {
		    _player = player;
		    _player.Pause();
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

        private IContainer GetAutomata(Sampler sampler)
	    {
		    Store itemStore = Store.CreateItemStore(sampler.Capacity);


            var automataStore = new Store(new FloatSeries(3, 0f,0f,0f), sampler);
			automataStore.BakeData();
            automataStore.GetFullSeries().SetSeriesAtIndex(575, new FloatSeries(3, 0f,0f,.7f));

		    var runner = new Runner(automataStore);
		    CreateBlock1(runner);
		    CreateBlock2(runner);
		    runner.ActiveIndex = 1;

			var composite = new AutomataComposite(itemStore, automataStore, runner);

			var ls = new LinkingStore(composite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
		    composite.AddProperty(PropertyId.FillColor, ls);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 6.8f).Store);
		    composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);


		    //composite.AddProperty(PropertyId.Orientation, new FloatSeries(1, 0f).Store);
		    //composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
		    //composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
		    composite.Renderer = new PolyShape();
			
		    Store loc = new Store(MouseInput.MainFrameRect.Outset(-0f), sampler);
		    composite.AppendProperty(PropertyId.Location, loc);
		    return composite;
        }

        private RuleSet CreateBlock1(Runner runnerParam)
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

        private RuleSet CreateBlock2(Runner runnerParam)
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
                runner.Automata.GetFullSeries().SetSeriesAtIndex(index, new FloatSeries(3, 0f, 0f, .7f));
            };

            cond = (currentValue, target, runner) => runner.PassCount < 2 && SeriesUtils.Random.NextDouble() < 0.003;
            rules.AddRule(cond, Rule.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));

            cond = (currentValue, target, runner) => runner.PassCount < 50;
            rules.AddRule(cond, DarkenSmall);

            cond = (currentValue, target, runner) => runner.PassCount < 52 && SeriesUtils.Random.NextDouble() < 0.0003;
            rules.AddRule(cond, Rule.SetSeriesAtIndexFn(0, new FloatSeries(3, 0, 0, 0.7f)));

            cond = (currentValue, target, runner) => SeriesUtils.Random.NextDouble() < 0.00001;
            rules.AddRule(cond, RandomAny);

            cond = (currentValue, target, runner) => target.Max().Z > 0.99;
            rules.AddRule(cond, Rule.SetSeriesAtIndexFn(0, Colors.Black));

            cond = (currentValue, target, runner) => target.Max().Y > 0.99 && target.Min().Y < 0.01;
            rules.AddRule(cond, RandomAny);

            cond = (currentValue, target, runner) => currentValue.Z < 0.1f;
            rules.AddRule(cond, AverageMax95);

            cond = (currentValue, target, runner) => currentValue.X > 0.90f;
            rules.AddRule(cond, swapFn1);

            cond = (currentValue, target, runner) => Math.Abs(currentValue.X - currentValue.Y) > 0.9f;
            rules.AddRule(cond, swapFn2);

            cond = (currentValue, target, runner) => currentValue.Z < 0.1f || currentValue.Z > 0.94f;
            rules.AddRule(cond, (currentValue, target) => {
                var temp = swapFn1;
                swapFn1 = swapFn2;
                swapFn2 = temp;
                return currentValue;
            });

            cond = (currentValue, target, runner) => target.Average().Y < 0.001f;
            rules.AddRule(cond, Mix1);

            cond = (currentValue, target, runner) => true;
            rules.AddRule(cond, Mix1);

            runnerParam.AddRuleSet(rules);
            rules.Reset(runnerParam);

            return rules;
        }

        // experimental functions
        private static readonly ParameterizedFunction Darken = Rule.InterpolateWithConstantFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = Rule.InterpolateWithConstantFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = Rule.InterpolateWithConstantFn(Colors.White, 0.1f);

        private static readonly ParameterizedFunction Average01 = Rule.EvalAndInterpolateFn(SeriesUtils.Average, 0.1f);
        private static readonly ParameterizedFunction AverageMax95 = Rule.EvalAndInterpolateFn(SeriesUtils.Max, 0.95f);
        private static readonly ParameterizedFunction MaxNeighbor02 = Rule.EvalAndInterpolateFn(SeriesUtils.Max, 0.2f);
        private static readonly ParameterizedFunction MinNeighbor99 = Rule.EvalAndInterpolateFn(SeriesUtils.Min, 0.99f);

        private static readonly ParameterizedFunction Mix1 = Rule.ModifyResultsFn(
	        Rule.EvaluateNeighborsFn(Rule.MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.Average), SeriesUtils.ClampTo01);

        private static readonly ParameterizedFunction RandomAny = Rule.RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid30_70 = Rule.RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark00_30 = Rule.RandomColorFn(0.0f, 0.3f);
    }
}
