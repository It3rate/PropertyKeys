using DataArcs.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
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

	    public UserInputTest(Player player)
	    {
		    _player = player;
	    }

	    public void NextVersion()
	    {
		    IComposite comp = GetHexGrid();
		    _player.AddActiveElement(comp);

		    //comp.EndTransitionEvent += CompOnEndTransitionEvent;
	    }

	    private void CompOnEndTransitionEvent(object sender, EventArgs e)
	    {
		    BlendTransition bt = (BlendTransition)sender;
		    bt.Reverse();
		    bt.Restart();
	    }

        IComposite GetHexGrid()
        {
	        var composite = new Composite(Store.CreateItemStore(56));

	        Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
	        composite.AddProperty(PropertyId.Location, loc);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 1f, 0.1f).Store);

	        composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 20f).Store);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.5f, 0, 0,  0, 0.5f, 0.5f).Store);
            composite.Renderer = new PolyShape();
           // AddGraphic(composite);
	        return composite;
        }

        private static void AddGraphic(Composite composite)
        {
	        composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store);
	        composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store);
	        composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.5f, 0, 0, 0f, 0, 0.5f).Store);

	        var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
	        var lineLink = new LinkingStore(composite.CompositeId, PropertyId.Radius, SeriesUtils.X, lineStore);
	        composite.AddProperty(PropertyId.PenWidth, lineLink);
	        composite.Renderer = new PolyShape();
        }

    }
}
