using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Simulators;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
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
            _easeStore = new Store(new FloatSeries(1, 0f, 1f), 
                new Easing(EasingType.EaseInOut3), CombineFunction.Replace);
        }

        HexagonSampler Hex => new HexagonSampler(new int[] { 10, 9 });
        RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });
        public void NextVersion()
        {
            _player.Pause();

            _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
			_player.Clear();

            switch (_versionIndex)
            {
	            case -3:
		            // only happens once
		            _player.AddActiveElement(GetContainer(Ring, false));
		            _timer = new Timer(0, 2500, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_timer);
                    break;
	            case -2:
		            // only happens once
		            _currentBlend = new BlendTransition( GetContainer(Ring, false),GetContainer(Hex, false), new Timer(0, 1000), _easeStore);
		            _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_currentBlend);
		            break;
	            case -1:
                    // only happens once
                    _player.AddActiveElement(GetContainer(Hex, false));
                    _timer = new Timer(0, 2500, null);
                    _timer.EndTimedEvent += CompOnEndTransitionEvent;
                    _player.AddActiveElement(_timer);
                    break;
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
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Hex, false), new Timer(0, 2000), _easeStore);
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
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Ring, false), new Timer(0, 2000), _easeStore);
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
        
		IContainer GetContainer(Sampler sampler, bool is2D)
        {
            Store itemStore = Store.CreateItemStore(sampler.SampleCount);
            var composite = new Container(itemStore);

            composite.AddProperty(PropertyId.Orientation, new FloatSeries(1, 0f).Store());
            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store());
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6, 3).Store());//, 3, 6).Store);
			composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.3f, 0.4f, 0.3f, 0.4f, 1f).Store());
			composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 1f,1f,1f,  0.05f, 0.05f, 0.2f,  1f, 1f, 1f).Store());
			composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.8f).Store());
            composite.Renderer = new PolyShape();

            float xOutset = sampler is RingSampler ? -100f : -30f;
            Store loc = new Store(MouseInput.MainFrameRect.Outset(xOutset, -30f), sampler);
			composite.AppendProperty(PropertyId.Location, loc);
			if (is2D)
			{
                AddPhysics(composite);
			}
			return composite;
		}

        private void AddPhysics(IContainer composite)
        {
            _physicsComposite = new PhysicsComposite();
            _player.AddActiveElement(_physicsComposite);
            for (int i = 0; i < composite.Capacity; i++)
            {
	            _physicsComposite.CreateBezierBody(i, composite);
            }

            // todo: LinkingStore location causes recursive location lookup error when getting capacity.
            //         composite.AppendProperty(PropertyId.PenPressure, locStore);
            //         LinkingStore ls = new LinkingStore(composite.CompositeId, PropertyId.PenPressure, SlotUtils.XY, null);
            //         composite.AddProperty(PropertyId.Location, ls);

            LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
            composite.AddProperty(PropertyId.Location, ls);
            LinkingStore lso = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Orientation, SlotUtils.X, null);
            composite.AddProperty(PropertyId.Orientation, lso);

        }
	}
}
