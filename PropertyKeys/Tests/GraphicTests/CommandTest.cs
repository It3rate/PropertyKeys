using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Commands;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class CommandTest : ITestScreen
    {
        private readonly Player _player;

        public CommandTest(Player player)
        {
            _player = player;
        }

        public void NextVersion()
        {

	        Store loc = new Store(new RectFSeries(150f, 50f, 550f, 350f), new HexagonSampler(new int[] { 20, 14 }));
	        Store fillColor = new Store(new FloatSeries(1, 1, 0.5f));

	        //var composite = new Container(Store.CreateItemStore(70));
         //   composite.AddProperty(PropertyId.Location, loc);
	        //composite.AddProperty(PropertyId.FillColor, fillColor);
         //   composite.Renderer = new PolyShape();
         //   _player.ActivateComposite(composite.Id);

			CommandCreateContainer ccc = new CommandCreateContainer(
				Store.CreateItemStore(loc.Capacity),
				null, 
				new PolyShape(), 
				new Dictionary<PropertyId, int>(){ {PropertyId.Location, loc.Id}, { PropertyId.FillColor, fillColor.Id} });
			ccc.Execute();
        }
    }
}
