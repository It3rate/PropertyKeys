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
using Motive.Stores;

namespace Motive.Tests.GraphicTests
{
	class BezierTest : ITestScreen
	{
		private readonly Runner _runner;

		public BezierTest(Runner runner)
		{
			_runner = runner;
		}

		public void NextVersion()
		{
			var bezSeries = new BezierSeries(new[] { 50f,300f, 100f,0f,400f,200f, 50f,650f,700f,150f, 400f,-110f,50f,300f },
				new BezierMove[] { BezierMove.MoveTo, BezierMove.QuadTo, BezierMove.QuadTo, BezierMove.QuadTo });
            //var bezSeries = new BezierSeries(new []{50f,200f, 100f,50f,700f,400f}, new BezierMove[]{ BezierMove.MoveTo, BezierMove.QuadTo });
			var bezSampler = new BezierSampler(bezSeries, null, 50);
			var bezStore = new Store(bezSeries, bezSampler);

			var tModStore = new FunctionalTStore(
				(t, internalT) => (t * .7f + internalT) % 1, // tail
				(time, it) => it + 0.01f); // delay
			var locStore = new MergingStore(tModStore, bezStore);

			IStore fillColor = new FloatSeries(3,  0.1f, 0.2f, 1f,  0.8f, 0.2f, 0.6f,  1f, 0.8f, 0.1f).Store();
			IStore radius = new FloatSeries(1, 5, 14, 4, 8).Store();
            IStore pointCount = new IntSeries(1, 6, 12).Store();
            IStore starness = new FloatSeries(1, -0.5f, 1.2f).Store();

            CommandCreateContainer cmdGrid = new CommandCreateContainer(
				Store.CreateItemStore(bezSampler.SampleCount),
				null,
				new PolyShape(true, true),
				new Dictionary<PropertyId, int>()
				{
					{PropertyId.Location, locStore.Id},
					{PropertyId.FillColor, fillColor.Id},
					{PropertyId.Radius, radius.Id},
					{PropertyId.PointCount, pointCount.Id},
					{PropertyId.Starness, starness.Id},
                });
			cmdGrid.Execute();
        }
	}
}
