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
	        var mouseLink = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);

	        var composite = new Container(Store.CreateItemStore(20 * 11));
			//var dim = new FloatSeries(2, 40f, 40f, _mouseInput.MainFrameSize.FloatDataAt(2)+40f, _mouseInput.MainFrameSize.FloatDataAt(3) + 40f);
	        Store loc = new Store(_mouseInput.MainFrameSize, new HexagonSampler(new int[] { 20, 11 }));
	        composite.AppendProperty(PropertyId.Location, loc);

            ComparisonSampler csLoc = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.XY);
            var locMouseStore = new Store(new FloatSeries(2, 0f, 0f, 30f, 50f), csLoc, CombineFunction.Add);
            composite.AppendProperty(PropertyId.Location, locMouseStore);


            ComparisonSampler cs = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.X);
            var mouseRadius = new Store(new FloatSeries(2, 3f, 3f, 12f, 12f), cs);
            composite.AppendProperty(PropertyId.Radius, mouseRadius);

            ComparisonSampler cso = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.Y);
            var mouseOrient = new Store(new FloatSeries(1, 0f, 1f), cso);
            composite.AddProperty(PropertyId.Orientation, mouseOrient);

            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
            composite.AddProperty(PropertyId.Starness, new FloatSeries(1, 2.2f).Store);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.5f, 0.8f, 0.8f, 0.8f, 0.5f, 0.5f).Store);
            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.3f, 0.5f, 0.5f, 0.1f, 0.1f, 0.4f).Store);
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 2f).Store);
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
