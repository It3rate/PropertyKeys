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

            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(53));
            Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.3f, 0.1f, 0.2f).Store);
            AddGraphic(composite);

            return composite;
        }
        public BlendTransition GetComposite1()
        {
            var compositeStart = GetComposite0();
            var compositeEnd = (Composite)compositeStart.CreateChild();
            
            Store loc = new Store(new FloatSeries(2, 100f, 100f, 500f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            compositeEnd.AddProperty(PropertyId.Location, loc);
            compositeEnd.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.9f, 0.2f, 0.9f).Store);
            
            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            var result = new BlendTransition(compositeStart, compositeEnd, 0, _player.CurrentMs, 3000, easeStore);

            var radiusLink = new LinkingStore(result.CompositeId, PropertyId.FillColor, new float[] { 16f, 32f }, new LineSampler(), CombineFunction.Multiply);
            ((PolyShape)compositeStart.Graphic).Radius = radiusLink;

            var starnessLink = new LinkingStore(compositeEnd.CompositeId, PropertyId.Location, new float[] { 500f }, new LineSampler(), CombineFunction.DivideFrom);
            ((PolyShape)compositeStart.Graphic).Starness = starnessLink;
            return result;
        }

        private static void AddGraphic(Composite composite)
        {
	        composite.Graphic = new PolyShape(10, pointCount:5);
        }
    }
}
