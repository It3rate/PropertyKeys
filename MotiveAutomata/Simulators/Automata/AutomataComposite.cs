using Motive.Stores;
using Motive.SeriesData;

namespace Motive.Components.Simulators.Automata
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


        public override void StartUpdate(double currentTime, double deltaTime)
        {
	        base.StartUpdate(currentTime, deltaTime);
	        //if (SeriesUtils.Random.NextDouble() < 0.01 && Runner.PassCount > 250)
	        //{
		       // Runner.NextBlock();
	        //}

	        Runner.StartUpdate(currentTime, deltaTime);
        }
    }
}
