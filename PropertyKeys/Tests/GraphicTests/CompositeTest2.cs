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
	        _player.AddElement(GetTest0());
        }


        public static Composite GetTest0()
        {
	        var composite = new Composite();

	        composite.AddProperty(PropertyId.Items, Store.CreateItemStore(53));
	        Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[]{7,9}) );
	        composite.AddProperty(PropertyId.Location, loc);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.3f, 0.1f, 0.2f).Store);
	        AddGraphic(composite);

	        return composite;
        }

        private static void AddGraphic(Composite composite)
        {
	        composite.Graphic = new PolyShape(10);
        }
    }
}
