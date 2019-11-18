using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Simulators.Automata;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Properties;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class BitmapAutomataTest : ITestScreen
    {
        private readonly Player _player;
        private Bitmap[] bitmaps;
        private Timer _timer;
        private int totalSteps = 0;

        public BitmapAutomataTest(Player player)
        {
            _player = player;
            bitmaps = new[] { Resources.face, Resources.face2, Resources.face3 };
        }

        private int _step = 0;
        private int _bitmapIndex = 0;

        public void NextVersion()
        {
            _player.Clear();
            Container containerA = GetImage(bitmaps[_bitmapIndex]);
            switch (_step)
            {
                case 0:
                    //_player.AddActiveElement(containerA);

                    //_timer = new Timer(0, 1000, null);
                    //_timer.EndTimedEvent += CompOnEndTransitionEvent;
                    //_player.AddActiveElement(_timer);
                    break;
            }
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            ITimeable anim = (ITimeable)sender;
            _step++;
            if (_step > totalSteps)
            {
                _step = 0;
                _bitmapIndex = _bitmapIndex < bitmaps.Length - 1 ? _bitmapIndex + 1 : 0;
            }

            NextVersion();
        }


        public Container GetImage(Bitmap bitmap)
        {
            int columns = 20;
            int width = 675;
            int rows;
            var bounds = new RectFSeries(0, 0, width, width * (bitmap.Height / (float)bitmap.Width));
            //var sampler = new GridSampler(new int[] { w, h });
            var container = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out HexagonSampler sampler);
            rows = rowCount;
            var colorStore = new Store(bitmap.ToFloatSeriesHex(columns, rows));
            container.AddProperty(PropertyId.FillColor, colorStore);
            colorStore.BakeData();
            IStore items = container.GetStore(PropertyId.Items);
            items.BakeData();

            var automata = GetAutomata(sampler);
			_player.AddActiveElement(automata);

            return container;
        }

        private AutomataComposite GetAutomata(Sampler sampler)
        {
            Store itemStore = Store.CreateItemStore(sampler.Capacity);
            itemStore.BakeData();

            var automataStore = new Store(new FloatSeries(4, 0f, 0f, 0f, 1f, 1f), sampler);
            automataStore.BakeData();
            //automataStore.GetSeriesRef().SetRawDataAt(575, new FloatSeries(3, 0f,0f,.7f));

            var runner = new Runner(automataStore);
            CreateBlock1(runner);
            runner.ActiveIndex = 0;

            var composite = new AutomataComposite(itemStore, automataStore, runner);

            var ls = new LinkingStore(composite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
            composite.AddProperty(PropertyId.FillColor, ls);

            //composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 6.8f).Store);
            //var radStore = new Store(new FloatSeries(1, 6.5f, 6.5f, 9f, 9f), new LineSampler(), CombineFunction.Multiply);
            //var radiusLink = new LinkingStore(composite.CompositeId, PropertyId.Automata, new[] { Slot.MaxSlots }, radStore);
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

        private RuleSet CreateBlock1(Runner runnerParam)
        {
	        RuleSet rules = new RuleSet();

	        float perPassRnd = 0;
	        rules.BeginPass = () =>
	        {
		        perPassRnd = (float)SeriesUtils.Random.NextDouble();
	        };

	        rules.AddRule(Rule.PassCountIsUnder(30), Darken);
	        rules.AddRule(Rule.RandomChance(0.00002f), Rule.RandomColorFn(0.1f, .9f, .4f, .7f, .4f, .7f));
	        var isBrightTest = Rule.CurrentValueIsOver(Slot.Max, 0.9f);
	        var dimStars = Rule.AllConditionsTrue(
		        Rule.RandomChance(0.005f),
		        isBrightTest
	        );
	        rules.AddRule(dimStars, Rule.ConstInterpFn(Colors.Black, 0.05f));
	        rules.AddRule(isBrightTest, Rule.NopFn());
	        rules.AddRule(Rule.NeighboursEvaluationIsNear(SeriesUtils.MaxSlots, Slot.Y, Slot.Z, 0.09f), Rule.CombineFn(Rule.ShuffleValuesFn(), Rule.ConstInterpFn(Colors.Black, 0.005f)));
	        rules.AddRule(Rule.CurrentValueIsUnder(Slot.Min, 0.35f), Rule.EvalInterpFn(SeriesUtils.MaxSlots, 0.07f));
	        rules.AddRule(Rule.RandomChance(0.004f), Rule.ConstInterpFn(Colors.Cyan, 0.1f));
	        rules.AddRule(Rule.NeighboursEvaluationIsNear(SeriesUtils.MaxSlots, Slot.X, Slot.Y, 0.06f), Rule.ConstInterpFn(Colors.Red, 0.07f));
	        rules.AddRule(Rule.CurrentValueIsOver(Slot.Average, 0.2f), Rule.EvalInterpFn(SeriesUtils.MinSlots, 0.5f));

	        runnerParam.AddRuleSet(rules);

	        return rules;
        }
        private static readonly ParameterizedFunction Darken = Rule.ConstInterpFn(Colors.Black, 0.3f);
    }
}
