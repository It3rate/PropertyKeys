
using System;
using System.Collections.Generic;
using Motive.Commands;
using Motive.Components;
using Motive.Components.Transitions;
using Motive.Graphic;
using Motive.Samplers.Utils;
using Motive.SeriesData;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Vis;

namespace Motive.Tests.GraphicTests
{
    public class VisTest : ITestScreen
    {
        private readonly Runner _runner;
        private VisAgent _agent;

        public VisTest(Runner runner)
        {
            _runner = runner;
            //_agent = new VisAgent();
        }

        public float halfM = 0.5f;

        public Quad LetterA(VisPad<Point> focusPad, VisPad<Stroke> viewPad)
        {
	        var letterbox = new Quad(halfM * .9f, 0.5f, 0f, 0f);
	        focusPad.Paths.Add(letterbox);

	        var topLine = letterbox.GetLine(CompassDirection.N);
	        var bottomLine = letterbox.GetLine(CompassDirection.S);

	        var leftStroke = new Stroke(topLine.MidNode, bottomLine.StartNode);
	        viewPad.Paths.Add(leftStroke);

	        var rightStroke = new Stroke(topLine.MidNode, bottomLine.EndNode);
	        viewPad.Paths.Add(rightStroke);

	        var midStroke = new Stroke(leftStroke.NodeAt(.6f), rightStroke.NodeAt(.6f));
	        viewPad.Paths.Add(midStroke);

	        return letterbox;
        }

        public void NextVersion()
        {
	        var bezSeries = new BezierSeries(new[] { 50f, 300f, 100f, 0f, 400f, 200f, 50f, 650f, 700f, 150f, 400f, -110f, 50f, 300f },
		        new BezierMove[] { BezierMove.MoveTo, BezierMove.QuadTo, BezierMove.QuadTo, BezierMove.QuadTo });
	        //var bezSeries = new BezierSeries(new []{50f,200f, 100f,50f,700f,400f}, new BezierMove[]{ BezierMove.MoveTo, BezierMove.QuadTo });
	        var bezSampler = new BezierSampler(bezSeries, null, 50);
	        var bezStore = new Store(bezSeries, bezSampler);

	        var tModStore = new FunctionalTStore(
		        (t, internalT) => (t * .7f + internalT) % 1,
		        (time, it) => it + 0.01f);//time/16000f);
	        var locStore = new MergingStore(tModStore, bezStore);

	        IStore fillColor = new FloatSeries(3, 0.1f, 0.2f, 1f, 0.8f, 0.2f, 0.6f, 1f, 0.8f, 0.1f).Store();
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

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            ITimeable anim = (ITimeable)sender;
            anim.Reverse();
            anim.Restart();
        }

    }
}
