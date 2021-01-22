using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Commands;
using Motive.Components;
using Motive.Graphic;
using Motive.Samplers.Utils;
using Motive.SeriesData;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Components.Transitions;

namespace Motive.Tests.GraphicTests
{
    public class CommandTest : ITestScreen
    {
        private readonly Runner _runner;

        public CommandTest(Runner runner)
        {
            _runner = runner;
        }

        public void NextVersion()
        {
	        //var hexSampler = new HexagonSampler(new int[] { 15, 10, 13, 14, 7, 14, 15, 10, 13, 14, 7, 4 }, null, GrowthMode.Widest);
	        var hexSampler = new HexagonSampler(new int[] { 15, 12 }, null, GrowthMode.Product);
	        Store hexStore = new Store(new RectFSeries(250f, 100f, 650f, 400f), hexSampler);

	        var cmdMouseInput = new CommandCreateMouseInput();
	        cmdMouseInput.Execute();

            var mouseLink = new CommandCreateLinkSampler(cmdMouseInput.ContainerId, PropertyId.MouseLocationT, SlotUtils.XY);
            Store fillColor = new Store(new FloatSeries(3, 0,0,0.6f,1f,1f,0.6f), mouseLink.Sampler);

            var mouseClicks = new CommandCreateLinkSampler(cmdMouseInput.ContainerId, PropertyId.MouseClickCount);
            var fs = new MutateTSampler(mouseClicks.Sampler, (f) => (f % 16) / 16f);
            Store pointCount = new Store(new IntSeries(1, 10, 6, 4, 9, 7, 5, 3, 10, 4, 9, 7, 6, 8, 5, 8, 3), fs);

            Store radius = new Store(_runner.ExternalValue0, new MutateValueSampler((a) => a * 10 + 8));
            Store starness = new Store(_runner.ExternalValue1, new MutateValueSampler((a) => a * 2f - .9f));

            CommandCreateContainer cmdGrid = new CommandCreateContainer(
				Store.CreateItemStore(hexStore.Capacity),
				null, 
				new PolyShape(true, true), 
				new Dictionary<PropertyId, int>()
				{
					{PropertyId.Location, hexStore.Id},
					{PropertyId.FillColor, fillColor.Id},
					{PropertyId.Radius, radius.Id},
					{PropertyId.PointCount, pointCount.Id},
					{PropertyId.Starness, starness.Id},
                });
			cmdGrid.Execute();
        }
    }
}
