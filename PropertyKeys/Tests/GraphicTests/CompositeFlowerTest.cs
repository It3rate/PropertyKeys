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
        int groupCount = 7;
        int starCount = 5;

        public CompositeFlowerTest(Player player)
        {
            _player = player;
        }

        public void NextVersion()
        {
            CreateTimer();
	        IComposite comp = GetComposite0();
	        //IComposite comp = GetRing();
            _player.AddActiveElement(comp);
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

        public IComposite GetComposite0()
        {
	        var composite = new Composite();
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(15));

            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SeriesUtils.X, new FloatSeries(1, 0f, 1f).Store);
            Store loc = new Store(new FloatSeries(2, 200f, 75f, 500f, 375f), new RingSampler(new int[] { 10,5 }));
            composite.AddProperty(PropertyId.Location, loc);

	        var graphic = GetRing();
	        composite.Graphic = graphic;
	        graphic.Parent = composite;

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
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(starCount));
            float r = 30f;
            float r2 = 15f;

            var ringSampler = new RingSampler(new int[] {starCount});
            
            // Link a custom property and multiply to generate an animated scaling transform.
			_timer.AddProperty(PropertyId.Custom1, new Store(new FloatSeries(2, .6f, .6f, 1.5f, 1.5f), new Easing(EasingType.EaseInOut3AndBack) ));
            var locStore = new Store(new FloatSeries(2, -r, -r, r, r, -r2, -r2, r2, r2), ringSampler, CombineFunction.Multiply);
            var loc = new LinkingStore(_timer.CompositeId, PropertyId.Custom1, SeriesUtils.XY, locStore);
            composite.AddProperty(PropertyId.Location, loc);
            
            composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 12f, 12f)));
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);

            BlendStore blendColors = GetBlendColor();
            LinkingStore col = new LinkingStore(_timer.CompositeId, PropertyId.EasedTCombined, SeriesUtils.X, blendColors);
            composite.AddProperty(PropertyId.FillColor, col);// new FunctionalStore(col, col2));

            var growStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Add);
            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.SampleAtT, SeriesUtils.X, growStore);
            composite.AddProperty(PropertyId.Orientation, ls);
            composite.AddProperty(PropertyId.Starness, ls);

            composite.Graphic = new PolyShape();

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
