using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.Components;
using MotiveCore.Components.ExternalInput;
using MotiveCore.Components.Simulators;
using MotiveCore.Components.Transitions;
using MotiveCore.Graphic;
using MotiveCore.Samplers;
using MotiveCore.SeriesData;
using MotiveCore.SeriesData.Utils;
using MotiveCore.Stores;

namespace MotiveCore.Tests.GraphicTests
{
	public class PhysicsTest : ITestScreen
	{
		private readonly Runner _runner;
		private PhysicsComposite _physicsComposite;
		private readonly Store _easeStore;
        int _versionIndex = -1;
        int _versionCount = 4;
        BlendTransition _currentBlend;
        IContainer _currentPhysics;
        private Timer _timer;


        public PhysicsTest(Runner runner)
		{
			_runner = runner;
			_runner.Pause();
            _easeStore = new Store(new FloatSeries(1, 0f, 1f), 
                new Easing(EasingType.EaseInOut3), CombineFunction.Replace);
        }

        HexagonSampler Hex => new HexagonSampler(new int[] { 10, 9 });
        RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });
        public void NextVersion()
        {
            _runner.Pause();

            _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
			_runner.Clear();

            switch (_versionIndex)
            {
	            case -3:
		            // only happens once
		            _runner.ActivateComposite(GetContainer(Ring, false).Id);
		            _timer = new Timer(0, 2500, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _runner.ActivateComposite(_timer.Id);
                    break;
	            case -2:
		            // only happens once
		            _currentBlend = new BlendTransition( GetContainer(Ring, false),GetContainer(Hex, false), new Timer(0, 1000), _easeStore);
		            _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            _runner.ActivateComposite(_currentBlend.Id);
		            break;
	            case -1:
                    // only happens once
                    _runner.ActivateComposite(GetContainer(Hex, false).Id);
                    _timer = new Timer(0, 2500, null);
                    _timer.EndTimedEvent += CompOnEndTransitionEvent;
                    _runner.ActivateComposite(_timer.Id);
                    break;
	            case 0:
		            // only happens once
		            _currentBlend = new BlendTransition(GetContainer(Hex, false), GetContainer(Ring, false), new Timer(0, 1000), _easeStore);
		            _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            _runner.ActivateComposite(_currentBlend.Id);
		            break;
                case 1:
	                _currentPhysics = _currentBlend.End;
                    AddPhysics(_currentPhysics);
                    _runner.ActivateComposite(_currentPhysics.Id);
                    _timer = new Timer(0, 2500, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _runner.ActivateComposite(_timer.Id);
                    break;
                case 2:
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Hex, false), new Timer(0, 2000), _easeStore);
	                _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
	                _runner.ActivateComposite(_currentBlend.Id);
                    break;
                case 3:
	                _currentPhysics = _currentBlend.End;
	                AddPhysics(_currentPhysics);
	                _runner.ActivateComposite(_currentPhysics.Id);
	                _timer = new Timer(0, 2500, null);
	                _timer.EndTimedEvent += CompOnEndTransitionEvent;
	                _runner.ActivateComposite(_timer.Id);
                    break;
                case 4:
	                _currentBlend = new BlendTransition(_currentPhysics, GetContainer(Ring, false), new Timer(0, 2000), _easeStore);
	                _currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
	                _runner.ActivateComposite(_currentBlend.Id);
	                break;
            }

            _runner.Unpause();
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
            Store loc = new Store(Runner.MainFrameRect.Outset(xOutset, -30f), sampler);
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
            _runner.ActivateComposite(_physicsComposite.Id);
            for (int i = 0; i < composite.Capacity; i++)
            {
	            _physicsComposite.CreateBezierBody(i, composite);
            }

            // todo: LinkingStore location causes recursive location lookup error when getting capacity.
            //         composite.AppendProperty(PropertyId.PenPressure, locStore);
            //         LinkingStore ls = new LinkingStore(composite.Id, PropertyId.PenPressure, SlotUtils.XY, null);
            //         composite.AddProperty(PropertyId.Location, ls);

            LinkingStore ls = new LinkingStore(_physicsComposite.Id, PropertyId.Location, SlotUtils.XY, null);
            composite.AddProperty(PropertyId.Location, ls);
            LinkingStore lso = new LinkingStore(_physicsComposite.Id, PropertyId.Orientation, SlotUtils.X, null);
            composite.AddProperty(PropertyId.Orientation, lso);

        }
	}
}
