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
	        var hexSampler = new HexagonSampler(new int[] { 15, 10, 13, 14, 7, 14 , 15, 10, 13, 14, 7, 4 }, null, GrowthType.Sum);
	        hexSampler.GrowthType = GrowthType.Sum;
            Store hexStore = new Store(new RectFSeries(250f, 100f, 650f, 400f), hexSampler);

	        var cmdMouseInput = new CommandCreateMouseInput();
	        cmdMouseInput.Execute();

            var mouseLink = new CommandCreateLinkSampler(cmdMouseInput.ContainerId, PropertyId.MouseLocationT, SlotUtils.XY);
            Store fillColor = new Store(new FloatSeries(3, 0,0,0.6f,1f,1f,0.6f), mouseLink.Sampler);

            var mouseClicks = new CommandCreateLinkSampler(cmdMouseInput.ContainerId, PropertyId.MouseClickCount);
            var fs = new FunctionSampler(mouseClicks.Sampler, (f) => (f % 16) / 16f);

            Store pointCount = new Store(new IntSeries(1, 10, 6, 4, 9, 7, 5, 3, 10, 4, 9, 7, 6, 8, 5, 8, 3), fs);
            Store radius = new Store(new FloatSeries(1, 8f, 18f), fs);

            CommandCreateContainer cmdGrid = new CommandCreateContainer(
				Store.CreateItemStore(hexStore.Capacity),
				null, 
				new PolyShape(true, true), 
				new Dictionary<PropertyId, int>()
				{
					{PropertyId.Location, hexStore.Id},
					{PropertyId.FillColor, fillColor.Id},
					{PropertyId.Radius, radius.Id},
					{PropertyId.PointCount, pointCount.Id}
                });
			cmdGrid.Execute();
        }
    }
}
