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

	        Store frame = new Store(new RectFSeries(150f, 50f, 550f, 350f), new HexagonSampler(new int[] { 20, 14 }));

	        var cmdMouseInput = new CommandCreateMouseInput();
	        cmdMouseInput.Execute();

            var mouseLink = new CommandCreateLinkSampler(cmdMouseInput.ContainerId, PropertyId.MouseLocationT, SlotUtils.XY);
            Store fillColor = new Store(new FloatSeries(2, 0,0,1f,1f), mouseLink.Sampler);
            CommandCreateContainer cmdGrid = new CommandCreateContainer(
				Store.CreateItemStore(frame.Capacity),
				null, 
				new PolyShape(true, true), 
				new Dictionary<PropertyId, int>(){ {PropertyId.Location, frame.Id}, { PropertyId.FillColor, fillColor.Id} });
			cmdGrid.Execute();
        }
    }
}
