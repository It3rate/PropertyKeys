﻿using System;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class CompositeFlowerTest : ITestScreen
    {
        private readonly Player _player;
        int starCount = 5;

        public CompositeFlowerTest(Player player)
        {
            _player = player;
        }

        private Timer _timer;
        public void CreateTimer()
        {
	        var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack));
	        _timer = _timer ?? new Timer(0, 3500, easeStore);
	        _timer.EndTimedEvent += CompOnEndTimerEvent;
	        _player.AddActiveElement(_timer);
        }
        private void CompOnEndTimerEvent(object sender, EventArgs e)
        {
	        if (sender is Timer anim)
	        {
		        anim.Restart();
	        }
        }

        public void NextVersion()
        {
            CreateTimer();
            IContainer ring = GetRing();
            IContainer comp = GetComposite0();
            IContainer hex = GetHex();
            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3), CombineFunction.Replace);
            var blend = new BlendTransition(comp, hex, new Timer(0, 3500), easeStore);
            //var blend = new BlendTransition(comp, comp, 0, _player.CurrentMs, 4000, easeStore);
            blend.Runner.EndTimedEvent += CompOnEndTransitionEvent;

            //IComposite itemToAdd = comp;
            //IComposite itemToAdd = hex;
            //IComposite itemToAdd = ring;
            IComposite itemToAdd = blend;
			
            _player.AddActiveElement(itemToAdd);
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
	        if (sender is ITimeable anim)
	        {
	            anim.Reverse();
	            anim.Restart();
	        }
        }

        public Container GetHex()
        {
	        var composite = new Container(Store.CreateItemStore(70));

	        Store loc = new Store(new RectFSeries(150f, 50f, 550f, 350f), new HexagonSampler(new int[] {10, 7}));
            composite.AddProperty(PropertyId.Location, loc);
	        composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store());

	        Store radiusStore = new Store(new FloatSeries(2, 8f, 8f), new LineSampler(70));
	        composite.AddProperty(PropertyId.Radius, radiusStore);
	        radiusStore.BakeData();
	        radiusStore.GetSeriesRef().SetRawDataAt(standOutStar, new FloatSeries(2, 16f,16f));
	        composite.Renderer = new PolyShape();

            IStore blendColors = GetBlendColor(2);
			//blendColors.ReverseEachElement();
            composite.AddProperty(PropertyId.FillColor, blendColors);

            return composite;
        }

        public IContainer GetComposite0()
        {
			int groupCount = 7;
	        var composite = new Container(Store.CreateItemStore(groupCount));

            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SlotUtils.X, new FloatSeries(1, 0f, 1f).Store());
            Store loc = new Store(new RectFSeries(200f, 75f, 500f, 375f), new RingSampler(new int[] { groupCount }, ls));
            composite.AddProperty(PropertyId.Location, loc);

            //composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, .5f, 0.1f).Store);
            //composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 8f, 8f)));
            //composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
            //composite.Renderer = new PolyShape();

            var flower = GetRing();
            composite.Renderer = flower.Renderer;
            composite.AddChild(flower);

            composite.Name = "comp0";

            return composite;
        }

        public Container GetRing()
        {
	        var composite = new Container();
	        starCount = 10; // 22;
	        composite.AddProperty(PropertyId.Items, Store.CreateItemStore(starCount));
	        float r = 40f;
	        float r2 = 15f;
	        float os = 0;
	        var ringSampler = new RingSampler(new int[] {6, 4}); // 7,6,5,4});
	        ringSampler.MinRadius = 0f;

	        // Link a custom property and multiply to generate an animated scaling transform.
	        _timer.AddProperty(PropertyId.User1, new Store(new FloatSeries(2, .6f, .6f, 1.5f, 1.5f), new Easing(EasingType.EaseInOut3AndBack)));
	        var locStore = new Store(new FloatSeries(2, -r + os, -r + os, r + os, r + os, -r2 + os, -r2 + os, r2 + os, r2 + os), ringSampler, CombineFunction.Multiply);
	        var loc = new LinkingStore(_timer.CompositeId, PropertyId.User1, SlotUtils.XY, locStore);
	        loc.CombineFunction = CombineFunction.Add;
	        composite.AddProperty(PropertyId.Location, loc);

	        composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 11f, 11f)));
	        composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store());
	        composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0f, 0f, 0f).Store());

	        IStore blendColors = GetBlendColor(0);
	        LinkingStore col = new LinkingStore(_timer.CompositeId, PropertyId.EasedTCombined, SlotUtils.X, blendColors);
	        composite.AddProperty(PropertyId.FillColor, col); // new FunctionalStore(col, col2));

	        var growStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply);
	        LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SlotUtils.X, growStore);
	        composite.AddProperty(PropertyId.Orientation, ls);
	        composite.AddProperty(PropertyId.Starness, ls);

	        composite.Renderer = new PolyShape();

	        composite.Name = "Ring";
	        return composite;
        }

        private static int standOutStar = 35;
        private static IStore GetBlendColor(int index)
        {
	        //var start = new float[] { 0.5f, 0.1f, 0.2f, .9f, .5f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f };
	        //var end = new float[] { 0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f };

	        var ringSampler = new RingSampler(new int[] { 6, 4 });
	        var hexSampler = new HexagonSampler(new int[] { 10, 7 });
	        var lineSampler = new LineSampler(70);
	        var blurSampler = new CenterBlurSampler(new int[] { 10, 7 }, false);

	        var start = new float[] { 0.5f, 0.3f, 0.3f, 1f, .95f, .95f };
            var colorStartStore = new Store(new FloatSeries(3, start), lineSampler);// );

            var end1 = new float[] { 1f, 1f, 0f, 0f, 1f, 1f };
            var colorEndStore1 = new Store(new FloatSeries(3, end1), hexSampler);
            var end2 = new float[] { 0.2f, 0.2f, 0.2f };
            var colorEndStore2 = new Store(new FloatSeries(3, end2), hexSampler);

            colorEndStore1.BakeData();
            colorEndStore1.GetSeriesRef().SetRawDataAt(standOutStar, new FloatSeries(3, 1f,0f,0f)); // Red star

            if (index == 0) return colorStartStore;
            else if (index == 1) return colorEndStore1;
            else return new BlendStore(new IStore[] { colorEndStore1, colorEndStore2 }, blurSampler);
        }
    }
}
