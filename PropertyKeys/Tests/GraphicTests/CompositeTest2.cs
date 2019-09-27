using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;
using DataArcs.Transitions;

namespace DataArcs.Tests.GraphicTests
{
    public class CompositeTest2 : ITestScreen
    {
	    private readonly Player _player;

	    public CompositeTest2(Player player)
	    {
		    _player = player;
	    }

        public void NextVersion()
        {
            BlendTransition comp = GetComposite1();
	        _player.AddElement(comp);

            comp.EndTransitionEvent += CompOnEndTransitionEvent;
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            BlendTransition bt = (BlendTransition)sender;
            bt.Reverse();
            bt.Restart();
        }

        public Composite GetComposite0()
        {
            var composite = new Composite();

            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(56));
            Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 1f, 0.1f).Store);
            AddGraphic(composite);

            return composite;
        }
        public BlendTransition GetComposite1()
        {
            var compositeStart = GetComposite0();
            var compositeEnd = (Composite)compositeStart.CreateChild();
            
            Store loc = new Store(new FloatSeries(2, 100f, 100f, 500f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            compositeEnd.AddProperty(PropertyId.Location, loc);
            compositeEnd.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.7f, 0.2f, 0.9f).Store);
            
            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            var result = new BlendTransition(compositeStart, compositeEnd, 0, _player.CurrentMs, 3000, easeStore);

            var radiusLink = new LinkingStore(result.CompositeId, PropertyId.FillColor, SeriesUtils.XY,
                new float[] { 14f, 15f, 22f, 23f }, new LineSampler(), CombineFunction.Multiply);
            compositeStart.AddProperty(PropertyId.Radius, radiusLink);

            var starnessLink = new LinkingStore(compositeEnd.CompositeId, PropertyId.Location, SeriesUtils.Y,
                new float[] { 500f }, new LineSampler(), CombineFunction.DivideFrom);
            compositeStart.AddProperty(PropertyId.Starness, starnessLink);
            compositeEnd.AddProperty(PropertyId.Starness, starnessLink);
            return result;
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
