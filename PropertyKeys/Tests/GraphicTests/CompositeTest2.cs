using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

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
	        _player.AddActiveElement(comp);

            comp.Runner.EndTransitionEvent += CompOnEndTransitionEvent;
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            IContinuous anim = (IContinuous)sender;
            anim.Reverse();
            anim.Restart();
        }

        public Container GetComposite0()
        {
            var composite = new Container(Store.CreateItemStore(56));
            
            Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 1f, 0.1f).Store);
            AddGraphic(composite);
            return composite;
        }
        public BlendTransition GetComposite1()
        {
            var compositeStart = GetComposite0();
            var compositeEnd = (Container)compositeStart.CreateChild();
            
            Store loc = new Store(new FloatSeries(2, 100f, 100f, 500f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            compositeEnd.AddProperty(PropertyId.Location, loc);
            compositeEnd.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.7f, 0.2f, 0.9f).Store);
            
            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            var result = new BlendTransition(compositeStart, compositeEnd, new Timer(0, 3000), easeStore);

            var radStore = new Store(new FloatSeries(1, 14f, 15f, 29f, 29f), new LineSampler(), CombineFunction.Multiply);
            var radiusLink = new LinkingStore(result.CompositeId, PropertyId.FillColor, SeriesUtils.Y, radStore);
            compositeStart.AddProperty(PropertyId.Radius, radiusLink);

            var starStore = new Store(new FloatSeries(1, 400f), new LineSampler(), CombineFunction.DivideFrom);
            var starnessLink = new LinkingStore(compositeEnd.CompositeId, PropertyId.Location, SeriesUtils.Y, starStore);
            compositeStart.AddProperty(PropertyId.Starness, starnessLink);

            return result;
        }

        private static void AddGraphic(Container container)
        {
	        container.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store);
	        container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store);
	        container.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.5f, 0, 0, 0f, 0, 0.5f).Store);

	        var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
	        var lineLink = new LinkingStore(container.CompositeId, PropertyId.Radius, SeriesUtils.X, lineStore);
            container.AddProperty(PropertyId.PenWidth, lineLink);
            container.Renderer = new PolyShape();
        }
    }
}
