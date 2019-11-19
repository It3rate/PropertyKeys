using DataArcs.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class UserInputTest : ITestScreen
    {
	    private readonly Player _player;
	    private MouseInput _mouseInput;

        public UserInputTest(Player player)
	    {
		    _player = player;
	    }

	    public void NextVersion()
	    {
		    _mouseInput = new MouseInput();
			_player.AddActiveElement(_mouseInput);

            IComposite comp = GetHexGrid();
		    _player.AddActiveElement(comp);


		    //comp.EndTimedEvent += CompOnEndTransitionEvent;
	    }

	    private void CompOnEndTransitionEvent(object sender, EventArgs e)
	    {
		    BlendTransition bt = (BlendTransition)sender;
		    bt.Runner.Reverse();
		    bt.Runner.Restart();
	    }

        IComposite GetHexGrid()
        {
	        var mouseLink = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);

	        var composite = new Container(Store.CreateItemStore(20 * 11));
	        Store loc = new Store(MouseInput.MainFrameRect, new HexagonSampler(new int[] { 20, 11 }));
	        composite.AppendProperty(PropertyId.Location, loc);

            var csLoc = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Bubble, SlotUtils.XY);
			csLoc.EffectRatio = new ParametricSeries(2, 0.1f, 0.4f);
			var chained = new ChainedSampler(csLoc, new Easing(EasingType.EaseInOut, EasingType.EaseInOut));

            var locMouseStore = new Store(new FloatSeries(2, 1.1f,1.1f, 0.9f,0.9f), chained, CombineFunction.Multiply);
            composite.AppendProperty(PropertyId.Location, locMouseStore);


            ComparisonSampler cs = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.X);
            cs.EffectRatio = new ParametricSeries(2, 2.5f, 1.2f);
            var chained2 = new ChainedSampler(cs, new Easing(EasingType.SmoothStart2, clamp:true));
            var mouseRadius = new Store(new FloatSeries(2, 10f, 10f, 9f, 9f, 3f, 3f), cs);
            composite.AppendProperty(PropertyId.Radius, mouseRadius);

            ComparisonSampler cso = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.Y);
            var mouseOrient = new Store(new FloatSeries(1, 0f, 1f), cso);
            composite.AddProperty(PropertyId.Orientation, mouseOrient);

            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store());
            composite.AddProperty(PropertyId.Starness, new FloatSeries(1, 2.2f).Store());
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.3f, 0.6f, 0.3f, 0.7f, 0.7f, 0.2f).Store());
            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.9f, 0.9f, 0.7f, 0.99f, 0.99f, 0.9f).Store());
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.6f).Store());
            composite.Renderer = new PolyShape();

	        return composite;
        }

        private static void AddGraphic(Container container)
        {
	        container.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store());
	        container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store());

	        var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
	        var lineLink = new LinkingStore(container.CompositeId, PropertyId.Radius, SlotUtils.X, lineStore);
	        container.AddProperty(PropertyId.PenWidth, lineLink);
	        container.Renderer = new PolyShape();
        }

    }
}
