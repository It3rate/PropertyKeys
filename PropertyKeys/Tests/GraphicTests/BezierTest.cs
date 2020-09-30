using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Commands;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
	class BezierTest : ITestScreen
	{
		private readonly Player _player;

		public BezierTest(Player player)
		{
			_player = player;
		}

		public void NextVersion()
		{
			var bezSeries = new BezierSeries(new[] { 50f,200f, 100f,50f,300f,200f, 150f,650f,700f,150f },
				new BezierMove[] { BezierMove.MoveTo, BezierMove.QuadTo, BezierMove.QuadTo });
            //var bezSeries = new BezierSeries(new []{50f,200f, 100f,50f,700f,400f}, new BezierMove[]{ BezierMove.MoveTo, BezierMove.QuadTo });
			var bezSampler = new BezierSampler(bezSeries, null, 50);
			var bezStore = new Store(bezSeries, bezSampler);

			IStore fillColor = new FloatSeries(2, 0.1f, 0.5f, 0.8f, 0.9f, 0.4f, 0.1f).Store();
			IStore radius = new FloatSeries(1, 5, 12, 4, 8).Store();
            IStore pointCount = new IntSeries(1, 6, 12).Store();
            IStore starness = new FloatSeries(1, -0.5f, 1.2f).Store();

            CommandCreateContainer cmdGrid = new CommandCreateContainer(
				Store.CreateItemStore(bezSampler.SampleCount),
				null,
				new PolyShape(true, true),
				new Dictionary<PropertyId, int>()
				{
					{PropertyId.Location, bezStore.Id},
					{PropertyId.FillColor, fillColor.Id},
					{PropertyId.Radius, radius.Id},
					{PropertyId.PointCount, pointCount.Id},
					{PropertyId.Starness, starness.Id},
                });
			cmdGrid.Execute();
        }
	}
}
