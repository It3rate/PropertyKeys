﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private Timer _timer;
        private int totalSteps = 0;
        private MouseInput _mouseInput;

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
            _mouseInput = new MouseInput();
            _mouseInput.MouseClick = CompOnEndTransitionEvent;
            _player.AddActiveElement(_mouseInput);

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

        private void CompOnEndTransitionEvent()
        {
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
            var rowCols = sampler.GetBakedStrideIndexes();

            var automataStore = new Store(SeriesUtils.MergeSeriesElements(colorSeries, radiusSeries, rowCols), sampler);
            automataStore.BakeData();

            var runner = new Runner(automataStore);
            CreateBlock1(runner);
            runner.ActiveIndex = 0;

            var automataComposite = new AutomataComposite(items, automataStore, runner);

            var ls = new LinkingStore(automataComposite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
            automataComposite.AddProperty(PropertyId.FillColor, ls);
			automataComposite.AddProperty(PropertyId.Location, hexContainer.GetStore(PropertyId.Location));

            var radStore = new Store(new FloatSeries(1, radius), new LineSampler(), CombineFunction.Multiply);
            var radiusLink = new LinkingStore(automataComposite.CompositeId, PropertyId.Automata, new[] { Slot.S3 }, radStore);
            automataComposite.AddProperty(PropertyId.Radius, radiusLink);

            automataComposite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store());
            automataComposite.Renderer = new PolyShape();
			
            return automataComposite;
        }

        private RuleSet CreateBlock1(Runner runnerParam)
        {
	        RuleSet rules = new RuleSet();
            rules.ResetFn = (runner) => { };

            ParameterizedFunction shrink = (currentValue, neighbors) =>
            {
                return SeriesUtils.InterpolateInto(currentValue,
                    new FloatSeries(4, 0, 0, 0, .2f),
                    new ParametricSeries(4, 0, 0, 0, 0.055f));
            };
            ParameterizedFunction grow = (currentValue, neighbors) =>
            {
                float minR = neighbors.MinSlots(Slot.S3).X;
                return SeriesUtils.InterpolateInto(currentValue,
                    new FloatSeries(4, 0, 0, 0, Math.Min(3f, 1.5f / minR)),
                    new ParametricSeries(4, 0, 0, 0, 0.01f));
            };

            bool flip = false;
            bool ignore = false;
            ParameterizedFunction fnA = (currentValue, neighbors) =>
            {
                if (ignore) return currentValue;
                return flip ? shrink(currentValue, neighbors) : grow(currentValue, neighbors);
            };
            ParameterizedFunction fnB = (currentValue, neighbors) =>
            {
                if (ignore) return currentValue;
                return !flip ? shrink(currentValue, neighbors) : grow(currentValue, neighbors);
            };

            int counter = 0;
            rules.BeginPass = () =>
            {
                if (counter == 100)
                {
                    flip = !flip;
                }
                else if (counter == 120)
                {
                    ignore = true;
                }
                else if (counter >= 130)
                {
                    flip = !flip;
                    counter = 0;
                    ignore = false;
                }
                counter++;
            };

            rules.AddRule(RuleCondition.NeighboursEvaluationIsOver(SeriesUtils.MaxSlots, Slot.S3, 1.1f), shrink);//fnB);//
            rules.AddRule(RuleCondition.NeighboursEvaluationIsUnder(SeriesUtils.MinSlots, Slot.S3, 0.9f), fnA);

            rules.AddRule(RuleCondition.NeighboursEvaluationIsNear(SeriesUtils.MaxSlots, Slot.Z, Slot.X, 0.092f), fnA);
            //rules.AddRule(Rule.CurrentValueIsOver(Slot.Y, 0.8f),grow);
            //rules.AddRule(Rule.CurrentValueIsOver(Slot.Average, 0.6f), Rule.ConstInterpFn(Colors.White, .005f));
            //rules.AddRule(Rule.AlwaysTrue(), Rule.EvalInterpFn(SeriesUtils.AverageSlots, 0.1f));

            runnerParam.AddRuleSet(rules);

	        return rules;
        }
    }
}
