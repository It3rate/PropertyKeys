using System;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.ExternalInput.Serial;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    class SerialInputTest : ITestScreen
    {
        private readonly Player _player;
        private SensorSerialPort _sensorInput;

        public SerialInputTest(Player player)
        {
            _player = player;
        }

        public void NextVersion()
        {
	        _sensorInput = new SensorSerialPort();
            _player.ActivateComposite(_sensorInput.Id);

            IComposite comp = GetHexGrid();
            _player.ActivateComposite(comp.Id);


            //comp.EndTimedEvent += CompOnEndTransitionEvent;
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            BlendTransition bt = (BlendTransition)sender;
            bt.Runner.Reverse();
            bt.Runner.Restart();
        }

        IComposite GetHexGrid()
        {
            var mouseLink = new LinkSampler(_sensorInput.Id, PropertyId.MouseLocationT, SlotUtils.YX);

            var composite = new Container(Store.CreateItemStore(20 * 11));
            Store loc = new Store(MouseInput.MainFrameRect, new HexagonSampler(new int[] { 20, 11 }));
            composite.AppendProperty(PropertyId.Location, loc);

            var csLoc = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Bubble, SlotUtils.XY);
            csLoc.EffectRatio = new ParametricSeries(2, 0.1f, 0.4f);
            var chained = new ChainedSampler(csLoc, new Easing(EasingType.EaseInOut, EasingType.EaseInOut));

            var locMouseStore = new Store(new FloatSeries(2, 1.1f, 1.1f, 0.9f, 0.9f), chained, CombineFunction.Multiply);
            composite.AppendProperty(PropertyId.Location, locMouseStore);

            var penColorSampler = new LinkSampler(_sensorInput.Id, PropertyId.MpuGyroscope, SlotUtils.XYZ);
            var penColorStore = new Store(new FloatSeries(3, 0.4f, 1.0f, 0.4f, 1.0f, 0.4f, 1.0f), penColorSampler);
            composite.AddProperty(PropertyId.PenColor, penColorStore);

            var colorSampler = new LinkSampler(_sensorInput.Id, PropertyId.MpuAcceleration, SlotUtils.XYZ);
            var colorStore = new Store(new FloatSeries(3, 0.0f, 1.0f, 0.0f, 0.6f, 0.0f, 0.6f), colorSampler);
            composite.AddProperty(PropertyId.FillColor, colorStore);

            var starnessSampler = new LinkSampler(_sensorInput.Id, PropertyId.MpuAccelerationZ, SlotUtils.X);
            var starnessStore = new Store(new FloatSeries(1, -1.0f, 2.0f), starnessSampler);
            composite.AddProperty(PropertyId.Starness, starnessStore);

            var pressureSampler = new LinkSampler(_sensorInput.Id, PropertyId.PenPressure, SlotUtils.X);
            var pressureStore = new Store(new FloatSeries(1, 6.0f, 3f), pressureSampler);
            composite.AddProperty(PropertyId.PointCount, pressureStore);

            ComparisonSampler cs = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.X);
            cs.EffectRatio = new ParametricSeries(2, 2.5f, 1.2f);
            var chained2 = new ChainedSampler(cs, new Easing(EasingType.SmoothStart2, clamp: true));
            var mouseRadius = new Store(new FloatSeries(2, 10f, 10f, 9f, 9f, 3f, 3f), cs);
            composite.AppendProperty(PropertyId.Radius, mouseRadius);

            ComparisonSampler cso = new ComparisonSampler(loc.Sampler, mouseLink, SeriesEquationType.Polar, SlotUtils.Y);
            var mouseOrient = new Store(new FloatSeries(1, 0f, 1f), cso);
            composite.AddProperty(PropertyId.Orientation, mouseOrient);

            //composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store());
            //composite.AddProperty(PropertyId.Starness, new FloatSeries(1, 2.2f).Store());
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 2f).Store());
            //composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.3f, 0.6f, 0.3f, 0.7f, 0.7f, 0.2f).Store());
            //composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.9f, 0.9f, 0.7f, 0.99f, 0.99f, 0.9f).Store());
            composite.Renderer = new PolyShape();

            return composite;
        }

        private static void AddGraphic(Container container)
        {
            container.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store());
            container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store());

            var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
            var lineLink = new LinkingStore(container.Id, PropertyId.Radius, SlotUtils.X, lineStore);
            container.AddProperty(PropertyId.PenWidth, lineLink);
            container.Renderer = new PolyShape();
        }
    }
}
