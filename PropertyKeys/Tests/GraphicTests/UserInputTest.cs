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
	        var composite = new Container(Store.CreateItemStore(22 * 15));

	        Store loc = new Store(new FloatSeries(2, 10f, 10f, 750f, 450f), new HexagonSampler(new int[] { 22, 15 }));
	        composite.AppendProperty(PropertyId.Location, loc);

	        var mouseLinkLoc = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);
            ComparisonSampler csl = new ComparisonSampler(mouseLinkLoc, loc.Sampler, SeriesEquationType.SignedDistance);
            var locMouseStore = new Store(new FloatSeries(2, -180f, -180f, 180f, 180f), csl, CombineFunction.Add);
            composite.AppendProperty(PropertyId.Location, locMouseStore);

            var mouseLinkRadius = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);
            ComparisonSampler cs = new ComparisonSampler(mouseLinkRadius, loc.Sampler, SeriesEquationType.Distance);
            var mouseRadius = new Store(new FloatSeries(1, 25f, 10f), cs, CombineFunction.Replace);
            composite.AppendProperty(PropertyId.Radius, mouseRadius);

            var mouseOrientationStore = new Store(new FloatSeries(1, 0f, 1f));
            var mouseOrient = new LinkingStore(_mouseInput.CompositeId, PropertyId.SampleAtT, SlotUtils.Y, mouseOrientationStore);
            composite.AddProperty(PropertyId.Orientation, mouseOrient);


            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.5f, 0, 0, 0, 0.5f, 0.5f).Store);
            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.3f, 0.8f, 0.8f, 0.1f, 0.1f, 0.4f).Store);
            composite.Renderer = new PolyShape();


	        return composite;
        }

        private static void AddGraphic(Container container)
        {
	        container.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store);
	        container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store);
	        container.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.5f, 0, 0, 0f, 0, 0.5f).Store);

	        var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
	        var lineLink = new LinkingStore(container.CompositeId, PropertyId.Radius, SlotUtils.X, lineStore);
	        container.AddProperty(PropertyId.PenWidth, lineLink);
	        container.Renderer = new PolyShape();
        }

    }
}
