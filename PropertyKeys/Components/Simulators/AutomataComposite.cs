using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators
{
    public class AutomataComposite : Container
    {
	    private readonly IStore _automata;
	    private readonly IStore _previousAutomata;
        public override int Capacity { get => _automata.Capacity; set { } }

        public Runner Runner { get; }

        public AutomataComposite(IStore itemStore, IStore automataStore, Runner runner) : base(itemStore)
	    {
		    _automata = automataStore;
		    _previousAutomata = _automata.Clone();
			AddProperty(PropertyId.Automata, _automata);

		    Runner = runner;
        }
		
        private int _delayCount = 0;
	    private bool _isBusy = false;
	    bool _block1 = false;
	    private int _blockIndex = 0;
	    private int _passCount = 50;

        public override void StartUpdate(float currentTime, float deltaTime)
	    {
		    if (!_isBusy)
		    {
			    _isBusy = true;
			    base.StartUpdate(currentTime, deltaTime);
			    _delayCount++;
			    if (true)
			    {
				    if (SeriesUtils.Random.NextDouble() < 0.01 && _passCount > 250)
                    {
                        Runner.Reset();
                        _block1 = !_block1;
                        _blockIndex = 0;
                        _passCount = 0;
					}

				    Runner.PassCount++;
				    Runner.GetRuleSet(0).BeginPass();
                    _passCount++;
				    _blockIndex += 100;

                    int capacity = Capacity;
                    _automata.CopySeriesDataInto(_previousAutomata);
                    for (int i = 0; i < capacity; i++)
				    {
					    var currentValue = _previousAutomata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
                        var neighbors = _previousAutomata.GetNeighbors(i);

                        Runner.ActiveRuleSetIndex = (_block1 && _blockIndex > i) ? 0 : 1;

                        currentValue = Runner.InvokeRuleSet(currentValue, neighbors, i);
                        _automata.GetFullSeries().SetSeriesAtIndex(i, currentValue);
                    }
			    }
			    _isBusy = false;
		    }

	    }

    }
}
