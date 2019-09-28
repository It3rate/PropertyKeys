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
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(6));

            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.T, SeriesUtils.X, new FloatSeries(1, 0f, 1f));
            Store loc = new Store(new FloatSeries(2, 100f, 100f, 400f, 400f), new RingSampler(new int[] {6}, ls));
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

            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(6));
            float r = 30f;
            float r2 = 15f;

            var ringSampler = new RingSampler(new int[] { 6 });

            //         LinkingStore lsr = new LinkingStore(_timer.CompositeId, PropertyId.CurrentT, SeriesUtils.X, new FloatSeries(1, 0f, 1f));
            //         Store loc1 = new Store(new FloatSeries(2, -r, -r, r, r), ringSampler, CombineFunction.Interpolate);
            //         Store loc2 = new Store(new FloatSeries(2, -r2, -r2, r2, r2), ringSampler, CombineFunction.Interpolate);
            //var loc = new BlendStore(new IStore[]{loc1, loc2});

            LinkingStore loc = new LinkingStore(_timer.CompositeId, PropertyId.Easing, SeriesUtils.X, 
				new FloatSeries(2, -r, -r, r, r, -r2, -r2, r2, r2), ringSampler, CombineFunction.Multiply);

            //Store locx = new Store(new FloatSeries(2, -r, -r, r, r), ringSampler, CombineFunction.Replace);
            composite.AddProperty(PropertyId.Location, loc);

            //LinkingStore lsr = new LinkingStore(_timer.CompositeId, PropertyId.T, SeriesUtils.XY, new FloatSeries(2, 12f, 12f, 6f, 6f));
            composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(2, 12f, 12f)));
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);

            //LinkingStore col = new LinkingStore(_timer.CompositeId, PropertyId.T, SeriesUtils.XYZ,
            //    new FloatSeries(3, 1f, 0f, 0.1f,  1f, 1f, 0.1f,  0f, 0f, 1f), combineFunction: CombineFunction.Replace);
            var col2 = GetBlendColor();
            composite.AddProperty(PropertyId.FillColor, col2);// new FunctionalStore(col, col2));

            LinkingStore ls = new LinkingStore(_timer.CompositeId, PropertyId.Easing, SeriesUtils.X,
                new FloatSeries(1, 0f, 1f), combineFunction:CombineFunction.Replace);
            composite.AddProperty(PropertyId.Orientation, ls);
            composite.AddProperty(PropertyId.Starness, ls);

            composite.Graphic = new PolyShape();

            return composite;
        }

        private static BlendStore GetBlendColor()
        {
            var start = new float[] { 0.3f, 0.1f, 0.2f, 1f, 1f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f };
            var end = new float[] { 0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f };
            var colorStartStore = new Store(new FloatSeries(3, start));
            var colorEndStore = new Store(new FloatSeries(3, end));
            return new BlendStore(colorStartStore, colorEndStore);
        }
    }
}
