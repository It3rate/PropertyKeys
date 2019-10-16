using System;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
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

        public void NextVersion()
        {
            CreateTimer();
            IComposite ring = GetRing();
            IComposite comp = GetComposite0();
            IComposite hex = GetHex();
            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3), CombineFunction.Replace, CombineTarget.T);
            var blend = new BlendTransition(comp, hex, 0, _player.CurrentMs, 4000, easeStore);
            //IComposite comp = GetRing();
	        //_player.AddActiveElement(comp);
            //_player.AddActiveElement(hex);
            //_player.AddActiveElement(ring);
	        _player.AddActiveElement(blend);
            if (comp is BlendTransition bt)
            {
	            bt.EndTransitionEvent += CompOnEndTransitionEvent;
            }
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
	        if (sender is Animation anim)
	        {
	            anim.Reverse();
	            anim.Restart();
	        }
        }

        public Composite GetHex()
        {
	        var composite = new DrawableComposite(Store.CreateItemStore(70));

	        Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] {10, 7}));
	        composite.AddProperty(PropertyId.Location, loc);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, .5f, 0.1f).Store);
	        composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 8f, 8f)));
	        composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
	        composite.Renderer = new PolyShape();

	        return composite;
        }

        public IComposite GetComposite0()
        {
			int groupCount = 7;
	        var composite = new DrawableComposite(Store.CreateItemStore(groupCount));

            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SeriesUtils.X, new FloatSeries(1, 0f, 1f).Store);
            Store loc = new Store(new FloatSeries(2, 200f, 75f, 500f, 375f), new RingSampler(new int[] { groupCount }, ls));
            composite.AddProperty(PropertyId.Location, loc);

            //composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, .5f, 0.1f).Store);
            //composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 8f, 8f)));
            //composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
            //composite.Renderer = new PolyShape();

            var flower = GetRing();
            composite.Background = flower;
            composite.Renderer = flower.Renderer;
            composite.AddChild(flower);

            composite.Name = "comp0";

            return composite;
        }

        private Animation _timer;
        public void CreateTimer()
        {
            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack));
            _timer = _timer ?? new Animation(0, Player.GetPlayerById(0).CurrentMs, 3500, easeStore);
            _timer.EndTransitionEvent += CompOnEndTimerEvent;
            _player.AddActiveElement(_timer);
        }
        private void CompOnEndTimerEvent(object sender, EventArgs e)
        {
            if (sender is Animation anim)
            {
                anim.Restart();
            }
        }

        public DrawableComposite GetRing()
        {
            var composite = new DrawableComposite();
            starCount = 10;// 22;
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(starCount));
            float r = 30f;
            float r2 = 15f;
            float os = 0;
            var ringSampler = new RingSampler(new int[] { 6, 4 });// 7,6,5,4});
            
            // Link a custom property and multiply to generate an animated scaling transform.
			_timer.AddProperty(PropertyId.Custom1, new Store(new FloatSeries(2, .6f, .6f, 1.5f, 1.5f), new Easing(EasingType.EaseInOut3AndBack) ));
            var locStore = new Store(new FloatSeries(2, -r + os, -r + os, r + os, r + os, -r2 + os, -r2 + os, r2 + os, r2+os), ringSampler, CombineFunction.Multiply);
            var loc = new LinkingStore(_timer.CompositeId, PropertyId.Custom1, SeriesUtils.XY, locStore);
            loc.CombineFunction = CombineFunction.Add;
            composite.AddProperty(PropertyId.Location, loc);

            composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 8f, 8f)));
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);

            BlendStore blendColors = GetBlendColor();
            LinkingStore col = new LinkingStore(_timer.CompositeId, PropertyId.EasedTCombined, SeriesUtils.X, blendColors);
            composite.AddProperty(PropertyId.FillColor, col);// new FunctionalStore(col, col2));

            var growStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply);
            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SeriesUtils.X, growStore);
            composite.AddProperty(PropertyId.Orientation, ls);
            composite.AddProperty(PropertyId.Starness, ls);

            composite.Renderer = new PolyShape();

            composite.Name = "Ring";
            return composite;
        }

        private static BlendStore GetBlendColor()
        {
            var start = new float[] { 0.5f, 0.1f, 0.2f,  .9f, .5f, 0,      0, 0.15f, 1f,     0, 0.5f, 0.1f };
            var end = new float[] { 0, 0.2f, 0.7f,       0.8f, 0, 0.3f,  0.7f, 1f, 0.1f,   0.4f, 0, 1f };
            var colorStartStore = new Store(new FloatSeries(3, start));
            var colorEndStore = new Store(new FloatSeries(3, end));
            return new BlendStore(colorStartStore, colorEndStore);
        }
    }
}
