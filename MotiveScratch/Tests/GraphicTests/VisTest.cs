
using System;
using Motive.Components;
using Motive.Components.Transitions;
using Motive.Graphic;
using Motive.Samplers;
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

        public void NextVersion()
        {
            //BlendTransition comp = GetComposite1();
            //_runner.ActivateComposite(comp.Id);

            //comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            ITimeable anim = (ITimeable)sender;
            anim.Reverse();
            anim.Restart();
        }

        public Container GetComposite0()
        {
            var composite = new Container(Store.CreateItemStore(56));

            Store loc = new Store(new RectFSeries(250f, 100f, 650f, 400f), new HexagonSampler(new int[] { 7, 9 }));
            composite.AddProperty(PropertyId.Location, loc);
            composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, .8f, .7f, 0.1f).Store());
            AddGraphic(composite);
            return composite;
        }

        private static void AddGraphic(Container container)
        {
            container.AddProperty(PropertyId.Radius, new FloatSeries(1, 30f).Store());
            container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store());
            //container.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store());
            container.AddProperty(PropertyId.PenColor, new FloatSeries(3, .2f, .1f, .2f, 0.3f, 0f, 0f).Store());

            var lineStore = new Store(new FloatSeries(1, .05f, .18f), new LineSampler(), CombineFunction.Multiply);
            var lineLink = new LinkingStore(container.Id, PropertyId.Radius, SlotUtils.X, lineStore);
            container.AddProperty(PropertyId.PenWidth, lineLink);
            container.Renderer = new PolyShape();
        }
    }
}
