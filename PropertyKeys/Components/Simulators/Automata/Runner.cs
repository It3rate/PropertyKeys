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
        private readonly IStore _previousAutomata;

        public int RuleSetCount => RuleSets.Count;
        protected List<RuleSet> RuleSets { get; }

        private int _activeIndex;
        public int ActiveIndex { get => _activeIndex; set => _activeIndex = Math.Max(0, Math.Min(RuleSets.Count - 1, value)); }
        public int AlternateIndex { get; set; }
        public int PassCount { get; set; }

        private float _transitionIndex;

        private bool _isBusy = false; // avoid updating until pass complete.
        private float _totalDeltaTime;

        public Runner(IStore automata, params RuleSet[] ruleSets)
        {
            Automata = automata;
            _previousAutomata = Automata;
            RuleSets = new List<RuleSet>();
            for (int i = 0; i < ruleSets.Length; i++)
            {
                AddRuleSet(ruleSets[i]);
            }
        }
        public void AddRuleSet(RuleSet ruleSet) => RuleSets.Add(ruleSet);
        public void NextBlock()
        {
	        AlternateIndex = ActiveIndex;
	        ActiveIndex = (ActiveIndex < RuleSets.Count - 1) ? ActiveIndex + 1 : 0;
	        PassCount = 0;
	        _transitionIndex = 0;
        }

        public virtual RuleSet GetRuleSet(int elementIndex)
        {
	        int currentIndex = elementIndex < _transitionIndex ? ActiveIndex : AlternateIndex;
            return RuleSets[currentIndex];
        }

        public virtual Series InvokeRuleSet(Series currentValue, Series neighbors, int elementIndex)
        {
            var ruleSet = GetRuleSet(elementIndex);
            return ruleSet.InvokeRules(currentValue, neighbors, this);
        }

        private void BeginPass()
        {
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
		        Automata.CopySeriesDataInto(_previousAutomata);
		        BeginPass();
		        for (int i = 0; i < capacity; i++)
		        {
			        var currentValue = _previousAutomata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
			        var neighbors = _previousAutomata.GetNeighbors(i);
			        var result = InvokeRuleSet(currentValue, neighbors, i);
			        Automata.GetFullSeries().SetSeriesAtIndex(i, result);
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
