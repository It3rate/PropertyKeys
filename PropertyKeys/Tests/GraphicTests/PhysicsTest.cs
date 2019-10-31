using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
	public class PhysicsTest : ITestScreen
	{
		private readonly Player _player;
		private PhysicsComposite _physicsComposite;
		private readonly Store _easeStore;
        int _versionIndex = -1;
        int _versionCount = 4;
        BlendTransition _currentBlend;
        IContainer _currentPhysics;
        private Timer _timer;


        public PhysicsTest(Player player)
		{
			_player = player;
			_player.Pause();


            _easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3), CombineFunction.Multiply, CombineTarget.T);

        }

        HexagonSampler Hex => new HexagonSampler(new int[] { 10, 9 });
        RingSampler Ring => new RingSampler(new int[] { 40, 25, 15, 10 });
        public void NextVersion()
        {
            _player.Pause();

            _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
			_player.Clear();

            switch (_versionIndex)
            {
                case 0:
					// only happens once
	                _currentBlend = new BlendTransition(GetContainer(Hex, false), GetContainer(Ring, false), new Timer(0, 1000), _easeStore);
	                _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_currentBlend);
                    break;
                case 1:
	                _currentPhysics = _currentBlend.End;
                    AddPhysics(_currentPhysics);
                    _player.AddActiveElement(_currentPhysics);
                    _timer = new Timer(0, 2500, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_timer);
                    break;
                case 2:
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Hex, false), new Timer(0, 1000), _easeStore);
	                _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
	                _player.AddActiveElement(_currentBlend);
                    break;
                case 3:
	                _currentPhysics = _currentBlend.End;
	                AddPhysics(_currentPhysics);
	                _player.AddActiveElement(_currentPhysics);
	                _timer = new Timer(0, 2500, null);
	                _timer.EndTimedEvent += CompOnEndTransitionEvent;
	                _player.AddActiveElement(_timer);
                    break;
                case 4:
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Ring, false), new Timer(0, 1000), _easeStore);
	                _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
	                _player.AddActiveElement(_currentBlend);
	                break;
            }

            _player.Unpause();
        }


    private void CompOnEndTransitionEvent(object sender, EventArgs e)
    {
		NextVersion();
    }

    IComposite AddTargetBody()
        {
            var body = new Container(new IntSeries(1, 0).Store);
            body.AddProperty(PropertyId.Location, new FloatSeries(2, 200f, 100f).Store);
            body.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
            body.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
            body.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.1f, 0.1f).Store);
            body.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
            body.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            body.Renderer = new PolyShape();
            return body;
        }
        IComposite AddCompositeBody()
		{
			var body = new Container(new IntSeries(1, 0).Store);
			LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
			body.AddProperty(PropertyId.Location, ls);

			body.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
			body.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
			body.AddProperty(PropertyId.FillColor, new FloatSeries(3,  0.4f, 0.3f, 0.4f).Store);
			body.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			body.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
			body.Renderer = new PolyShape();
            return body;
		}

		IContainer GetContainer(Sampler sampler, bool is2D)
		{
			var composite = new Container(Store.CreateItemStore(sampler.Capacity));
			composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5, 8, 3, 6).Store);
			composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.3f, 0.4f, 0.3f, 0.4f, 1f).Store);
			composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            composite.Renderer = new PolyShape();

            float xOutset = sampler is RingSampler ? -150f : -50f;
            Store loc = new Store(MouseInput.MainFrameSize.Outset(xOutset, -50f), sampler);
			composite.AppendProperty(PropertyId.Location, loc);
			if (is2D)
			{
                AddPhysics(composite);
			}
			return composite;
		}

        private void AddPhysics(IComposite composite)
        {
	        var locStore = composite.GetStore(PropertyId.Location);
	        var sampler = locStore.Sampler;

            _physicsComposite = new PhysicsComposite();
            _player.AddActiveElement(_physicsComposite);

            for (int i = 0; i < composite.Capacity; i++)
            {
                float tIndex = i / (composite.Capacity - 1f);
                var pos = locStore.GetValuesAtIndex(i);
                var pointCount = composite.GetSeriesAtT(PropertyId.PointCount, tIndex, null);
                var radius = composite.GetSeriesAtT(PropertyId.Radius, tIndex, null);

                var bezier = PolyShape.GeneratePolyShape(0f, (int)pointCount.X, 0, radius.X, radius.Y, 0);
                _physicsComposite.CreateBezierBody(pos.X, pos.Y, bezier, false);
            }

            LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
            composite.AddProperty(PropertyId.Location, ls);
            LinkingStore lso = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Orientation, SlotUtils.X, null);
            composite.AddProperty(PropertyId.Orientation, lso);

        }
	}
}
