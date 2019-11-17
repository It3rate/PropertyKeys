using System;
using System.Collections.Generic;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators.Automata
{
	public class Runner
    {
        public IStore Automata { get; }
        private readonly IStore _workingAutomata;

        public int RuleSetCount => RuleSets.Count;
        protected List<RuleSet> RuleSets { get; }

        private int _activeIndex;
        public int ActiveIndex { get => _activeIndex; set => _activeIndex = Math.Max(0, Math.Min(RuleSets.Count - 1, value)); }
        public int AlternateIndex { get; set; }
        public int PassCount { get; set; }

        private float _transitionIndex;

        private bool _isBusy = false; // avoid updating until pass complete.
        private float _totalDeltaTime;

        public int LastInvokedRuleSet { get; private set; }
        public int LastInvokedRule { get; private set; }
        public bool UpdateValuesAfterPass { get; set; } = true;

        public Runner(IStore automata, params RuleSet[] ruleSets)
        {
            Automata = automata;
            _workingAutomata = Automata.Clone();
            RuleSets = new List<RuleSet>();
            for (int i = 0; i < ruleSets.Length; i++)
            {
                AddRuleSet(ruleSets[i]);
            }
        }
        public void AddRuleSet(RuleSet ruleSet) => RuleSets.Add(ruleSet);
        public void NextBlock()
        {
	        UpdateValuesAfterPass = true;
	        AlternateIndex = ActiveIndex;
	        ActiveIndex = (ActiveIndex < RuleSets.Count - 1) ? ActiveIndex + 1 : 0;
	        PassCount = 0;
	        _transitionIndex = 0;
	        RuleSets[ActiveIndex].Reset(this);
        }

        public virtual RuleSet GetRuleSet(int elementIndex)
        {
	        int currentIndex = elementIndex < _transitionIndex ? ActiveIndex : AlternateIndex;
            LastInvokedRuleSet = currentIndex;
            return RuleSets[currentIndex];
        }

        public virtual Series InvokeRuleSet(Series currentValue, Series neighbors, int elementIndex)
        {
            var ruleSet = GetRuleSet(elementIndex);
            Series result = ruleSet.InvokeRules(currentValue, neighbors, this);
            LastInvokedRule = ruleSet.InvokedRule;
            return result;
        }

        private bool _isFirstRun = true;
        private void BeginPass()
        {
	        if (_isFirstRun)
	        {
		        _isFirstRun = false;
		        RuleSets[ActiveIndex].Reset(this);

            }
	        RuleSets[ActiveIndex].BeginPass();
	        if (ActiveIndex != AlternateIndex)
	        {
				RuleSets[AlternateIndex].BeginPass();
	        }
        }
        public virtual void StartUpdate(float currentTime, float deltaTime)
        {
	        _totalDeltaTime += deltaTime;
	        if (!_isBusy)
	        {
		        _isBusy = true;

		        int capacity = Automata.Capacity;
		        _transitionIndex += capacity * RuleSets[ActiveIndex].TransitionSpeed;// * (16f / (int)_totalDeltaTime);
		        IStore workingStore;
		        if (UpdateValuesAfterPass)
		        {
					Automata.CopySeriesDataInto(_workingAutomata);
					workingStore = _workingAutomata;

		        }
		        else
		        {
			        workingStore = Automata;
		        }

		        BeginPass();
		        for (int i = 0; i < capacity; i++)
		        {
			        var currentValue = workingStore.GetSeriesRef().GetVirtualValueAt(i, capacity);
			        var neighbors = workingStore.GetNeighbors(i);
			        var result = InvokeRuleSet(currentValue, neighbors, i);
			        Automata.GetSeriesRef().SetRawDataAt(i, result);
		        }

		        PassCount++;

		        _totalDeltaTime = 0;
		        _isBusy = false;
	        }
        }

        public virtual void EndUpdate(float currentTime, float deltaTime){ }

        public void Reset()
        {
            PassCount = 0;
            ActiveIndex = 0;
            foreach (var ruleSet in RuleSets)
            {
                ruleSet?.Reset(this);
            }
        }
    }

}
