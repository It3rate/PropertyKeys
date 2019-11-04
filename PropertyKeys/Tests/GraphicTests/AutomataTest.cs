using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Simulators;
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
		
        GridSampler Grid => new GridSampler(new int[] { 85, 60 });
        HexagonSampler Hex => new HexagonSampler(new int[] { 75, 50 });
        //RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });

        private IContainer GetAutomata(Sampler sampler)
	    {
		    Store itemStore = Store.CreateItemStore(sampler.Capacity);


            var automataStore = new Store(new FloatSeries(3, 0f,0f,0f), sampler);
			automataStore.BakeData();
            automataStore.GetFullSeries().SetSeriesAtIndex(575, new FloatSeries(3, 0f,0f,.7f));

		    var runner = new Runner(automataStore);
		    CreateBlock1(runner);
		    CreateBlock2(runner);

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

        private RuleSet CreateBlock2(Runner runner)
        {
            RuleSet rules = new RuleSet();
            Condition cond;

            ParameterizedFunction swapFn1 = Average01;
            ParameterizedFunction swapFn2 = RandomMid30_70;

            rules.BeginPass = () =>
            {
                //perPassRnd = (float)SeriesUtils.Random.NextDouble();
            };

            rules.Reset = () =>
            {
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetFullSeries().SetSeriesAtIndex(index, new FloatSeries(3, 0f, 0f, .7f));
            };

            cond = (currentValue, target) => runner.PassCount < 2 && SeriesUtils.Random.NextDouble() < 0.003;
            rules.AddRule(cond, RuleUtils.SetValueFn(0, new FloatSeries(3, 0, 0, 0.7f)));

            cond = (currentValue, target) => runner.PassCount < 40;
            rules.AddRule(cond, DarkenSmall);

            cond = (currentValue, target) => runner.PassCount < 42 && SeriesUtils.Random.NextDouble() < 0.0003;
            rules.AddRule(cond, RuleUtils.SetValueFn(0, new FloatSeries(3, 0, 0, 0.7f)));

            cond = (currentValue, target) => SeriesUtils.Random.NextDouble() < 0.00001;
            rules.AddRule(cond, RandomAny);

            cond = (currentValue, target) => target.Max().Z > 0.99;
            rules.AddRule(cond, RuleUtils.SetValueFn(0, Colors.Black));

            cond = (currentValue, target) => target.Max().Y > 0.99 && target.Min().Y < 0.01;
            rules.AddRule(cond, RandomAny);

            cond = (currentValue, target) => currentValue.Z < 0.1f;
            rules.AddRule(cond, AverageMax95);

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
            rules.AddRule(cond, RandomDark00_30);

            cond = (currentValue, target) => currentValue.Y < 0.3;
            rules.AddRule(cond, MaxNeighbor02);

            cond = (currentValue, target) => currentValue.Z > 0.2;
            rules.AddRule(cond, MinNeighbor99);

            runner.AddRuleSet(rules);

            return rules;
        }


        // experimental functions
        private static readonly ParameterizedFunction Darken = RuleUtils.InterpolateWithConstantFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = RuleUtils.InterpolateWithConstantFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = RuleUtils.InterpolateWithConstantFn(Colors.White, 0.1f);
        private static readonly ParameterizedFunction Average01 = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.1f), SeriesUtils.Average);
        private static readonly ParameterizedFunction AverageMax95 = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.95f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MaxNeighbor02 = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.2f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MinNeighbor99 = RuleUtils.ModifyNeighborsFn(RuleUtils.InterpolateFn(0.99f), SeriesUtils.Min);
        private static readonly ParameterizedFunction Mix1 = RuleUtils.ModifyResultsFn(
            RuleUtils.ModifyNeighborsFn(RuleUtils.MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.Average), SeriesUtils.ClampTo01);
        private static readonly ParameterizedFunction RandomAny = RuleUtils.RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid30_70 = RuleUtils.RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark00_30 = RuleUtils.RandomColorFn(0.0f, 0.3f);
    }
}
