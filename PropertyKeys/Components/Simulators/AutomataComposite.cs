using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators
{
    public class AutomataComposite : Container
    {
	    private IStore _automata;
	    public override int Capacity { get => _automata.Capacity; set { } }

	    public AutomataComposite(IStore itemStore, IStore automataStore) : base(itemStore)
	    {
		    _automata = automataStore;
			if (_automata != null)
			{
				AddProperty(PropertyId.Automata, _automata);
			}
        }

	    private int _delayCount = 0;
	    public override void StartUpdate(float currentTime, float deltaTime)
	    {
		    base.StartUpdate(currentTime, deltaTime);
		    _delayCount++;
		    if (_delayCount % 10 == 8)
		    {
			    int capacity = Capacity;
			    for (int i = 0; i < capacity; i++)
			    {
				    var org = _automata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
				    var neighbors = _automata.GetNeighbors(i).Average();
					org.InterpolateInto(neighbors, 0.1f);

					if (org.X > 0.4999f && org.X < .51f)
					{
						org = new FloatSeries(org.VectorSize, (float)SeriesUtils.Random.NextDouble(), (float)SeriesUtils.Random.NextDouble(), (float)SeriesUtils.Random.NextDouble());
					}
				    _automata.GetFullSeries().SetSeriesAtIndex(i, org);
			    }
		    }
	    }

    }
}
