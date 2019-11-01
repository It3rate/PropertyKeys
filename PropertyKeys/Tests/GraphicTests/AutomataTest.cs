using System;
using System.Collections.Generic;
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
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class AutomataTest : ITestScreen
    {
	    private readonly Player _player;
	    int _versionIndex = -1;
	    int _versionCount = 4;
	    private Timer _timer;
		
	    IContainer _current;

        public AutomataTest(Player player)
	    {
		    _player = player;
		    _player.Pause();
	    }

	    public void NextVersion()
	    {
		    _player.Pause();

		    _versionIndex = (_versionIndex > _versionCount - 1) ? 1 : _versionIndex + 1;
		    _player.Clear();

		    switch (_versionIndex)
		    {
			    case 0:
				    _current = GetGrid(Hex);
				    //_currentBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
				    _player.AddActiveElement(_current);
				    break;
			    case 1:
				    break;
			    case 2:
				    break;
			    case 3:
				    break;
			    case 4:
				    break;
		    }

		    _player.Unpause();
	    }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
        }

        HexagonSampler Hex => new HexagonSampler(new int[] { 50, 30 });
	    //RingSampler Ring => new RingSampler(new int[] { 30, 25, 20, 15 });

        private IContainer GetGrid(Sampler sampler)
	    {
		    Store itemStore = Store.CreateItemStore(sampler.Capacity);
		    //var composite = new Container(itemStore);
			
            var automataStore = new RandomSeries(3, SeriesType.Float, sampler.Capacity).BakedStore;
            automataStore.Sampler = sampler;
			var composite = new AutomataComposite(itemStore, automataStore);

			var ls = new LinkingStore(composite.CompositeId, PropertyId.Automata, SlotUtils.XYZ, null);
		    composite.AddProperty(PropertyId.FillColor, ls);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 8f).Store);
		    composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);


		    //composite.AddProperty(PropertyId.Orientation, new FloatSeries(1, 0f).Store);
		    //composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
		    //composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
		    composite.Renderer = new PolyShape();
			
		    Store loc = new Store(MouseInput.MainFrameSize.Outset(-30f), sampler);
		    composite.AppendProperty(PropertyId.Location, loc);
		    return composite;
        }
    }
}
