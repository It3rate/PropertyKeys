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
                    _player.AddActiveElement(containerA);

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
            int columns = 100;
            int width = 675;
            var bounds = new RectFSeries(0, 0, width, width * (bitmap.Height / (float)bitmap.Width));
            var hexContainer = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out float radius, out HexagonSampler sampler);
            IStore items = hexContainer.GetStore(PropertyId.Items);
            items.BakeData();
            int itemCount = items.Capacity;

            var colorSeries = bitmap.ToFloatSeriesHex(columns, rowCount);
            var radiusSeries = new FloatSeries(1, ArrayExtension.GetSizedFloatArray(itemCount, 1f));
            var prioritySeries = new FloatSeries(1, ArrayExtension.GetSizedFloatArray(itemCount, 0f));

            var automataStore = new Store(SeriesUtils.MergeSeriesElements(colorSeries, radiusSeries, prioritySeries));
            //automataStore.BakeData();
            //automataStore.GetSeriesRef().SetRawDataAt(575, new FloatSeries(3, 0f,0f,.7f));

            var runner = new Runner(automataStore);
            CreateBlock1(runner);
            runner.ActiveIndex = 0;

            var automataComposite = new AutomataComposite(items, automataStore, runner);
			automataComposite.AddProperty(PropertyId.Location, hexContainer.GetStore(PropertyId.Location));

            var ls = new LinkingStore(automataComposite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
            automataComposite.AddProperty(PropertyId.FillColor, ls);

            //composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 6.8f).Store);
            var radStore = new Store(new FloatSeries(1, radius), new LineSampler(), CombineFunction.Multiply);
            var radiusLink = new LinkingStore(automataComposite.CompositeId, PropertyId.Automata, new[] { Slot.S3 }, radStore);
            automataComposite.AddProperty(PropertyId.Radius, radiusLink);

            automataComposite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
            automataComposite.Renderer = new PolyShape();
			
            return automataComposite;
        }

        private RuleSet CreateBlock1(Runner runnerParam)
        {
	        RuleSet rules = new RuleSet();

	        float perPassRnd = 0;
	        rules.BeginPass = () =>
	        {
		        perPassRnd = (float)SeriesUtils.Random.NextDouble();
	        };

            rules.ResetFn = (Runner runner) =>
            {
                int index = (int)(runner.Automata.Capacity / 2.13f);
                runner.Automata.GetSeriesRef().SetRawDataAt(index, new FloatSeries(3, 0.1f, 0.1f, .7f));
            };


            //rules.AddRule(Rule.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.Z, 0.99f), Rule.SetSeriesAtIndexFn(0, Colors.Black));
            //var act = Rule.AllConditionsTrue(
            //    Rule.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.Y, 0.99f),
            //    Rule.NeighboursEvaluationIsUnder(SeriesUtils.MinSlots, Slot.Y, 0.01f)
            //);
            //rules.AddRule(act, RandomAny);
            rules.AddRule(Rule.RandomChance(0.0008f), Rule.ConstInterpFn(Colors.Red, 0.1f));
            rules.AddRule(Rule.CurrentValueIsUnder(Slot.Average, 0.3f), Rule.ConstInterpFn(Colors.Black, .005f));
            rules.AddRule(Rule.CurrentValueIsOver(Slot.Average, 0.6f), Rule.ConstInterpFn(Colors.White, .005f));
            rules.AddRule(Rule.AlwaysTrue(), Rule.EvalInterpFn(SeriesUtils.AverageSlots, 0.1f));

            runnerParam.AddRuleSet(rules);

	        return rules;
        }
    }
}
