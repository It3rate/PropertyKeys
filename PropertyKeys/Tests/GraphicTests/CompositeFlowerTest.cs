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

            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(56));
            Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 1f, 0.1f).Store);
            AddGraphic(composite);

            return composite;
        }

        private static void AddGraphic(Composite composite)
        {
            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store);
            composite.Graphic = new PolyShape();

            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.5f, 0, 0, 0f, 0, 0.5f).Store);
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, .2f, 5f, .2f).Store);
        }
    }
}
