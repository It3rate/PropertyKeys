using System;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;
using DataArcs.Transitions;

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
	        if (sender is BlendTransition bt)
	        {
	            bt.Reverse();
	            bt.Restart();
	        }
        }

        public IComposite GetComposite0()
        {
	        var composite = new Composite();
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(6));
	        Store loc = new Store(new FloatSeries(2, 100f, 100f, 400f, 400f), new RingSampler(new int[] { 6 }));
	        composite.AddProperty(PropertyId.Location, loc);
	        var graphic = GetRing();
	        composite.Graphic = graphic;
	        graphic.Parent = composite;

            return composite;
        }

        public Composite GetRing()
        {
            var composite = new Composite();

            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(6));
            Store loc = new Store(new FloatSeries(2, -0f, -0f, 50f, 50f), new RingSampler(new int[] { 6 }), CombineFunction.Replace);
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0f, 0.1f).Store);
            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 4f, 4f).Store);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
            composite.Graphic = new PolyShape();

            return composite;
        }
		
    }
}
