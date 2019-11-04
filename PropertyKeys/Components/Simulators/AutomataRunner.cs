﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators
{
    public delegate bool Condition(Series currentValue, Series neighbors);
    public delegate Series ParameterizedFunction(Series currentValue, Series neighbors);
    public class Rule
    {
        public Condition Condition { get; }
        public ParameterizedFunction ParameterizedFunction { get; }

        public Rule(Condition condition, ParameterizedFunction parameterizedFunction)
        {
            Condition = condition;
            ParameterizedFunction = parameterizedFunction;
        }
    }

    public class RuleSet
    {
        public List<Rule> Rules { get; } = new List<Rule>();
        public void AddRule(Condition condition, ParameterizedFunction fn) => Rules.Add(new Rule(condition, fn));
        public Action BeginPass { get; set; }
        public Series InvokeRules(Series currentValue, Series neighbors)
        {
            Series result = currentValue;
            for (int i = 0; i < Rules.Count; i++)
            {
                if (Rules[i].Condition(currentValue, neighbors))
                {
                    result = Rules[i].ParameterizedFunction(currentValue, neighbors);
                    break;
                }
            }
            return result;
        }
        public Action Reset { get; set; }
    }

    public class Runner
    {
        private readonly IStore _automata;
        private readonly IStore _previousAutomata;

        protected List<RuleSet> RuleSets { get; }
        public int ActiveRuleSetIndex { get; set; }
        public int PassCount { get; set; }

        public Runner(IStore automata, params RuleSet[] ruleSets)
        {
            _automata = automata;
            _previousAutomata = _automata;
            RuleSets = new List<RuleSet>();
            for (int i = 0; i < ruleSets.Length; i++)
            {
                AddRuleSet(ruleSets[i]);
            }
        }
        public void AddRuleSet(RuleSet ruleSet) => RuleSets.Add(ruleSet);

        public virtual RuleSet GetRuleSet(int automataIndex)
        {
            return RuleSets[ActiveRuleSetIndex];
        }

        public Series InvokeRuleSet(Series currentValue, Series neighbors, int automataIndex)
        {
            var ruleSet = GetRuleSet(automataIndex);
            return ruleSet.InvokeRules(currentValue, neighbors);
        }
        public void InvokePass()
        {
            int capacity = _automata.Capacity;
            _automata.CopySeriesDataInto(_previousAutomata);
            var ruleSet = GetRuleSet(0);
            ruleSet.BeginPass?.Invoke();
            for (int i = 0; i < capacity; i++)
            {
                var currentValue = _previousAutomata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
                var neighbors = _previousAutomata.GetNeighbors(i);
                ruleSet = GetRuleSet(0);
                var result = ruleSet.InvokeRules(currentValue, neighbors);
                _automata.GetFullSeries().SetSeriesAtIndex(i, result);
            }

            PassCount++;
        }

        public void Reset()
        {
            PassCount = 0;
            ActiveRuleSetIndex = 0;
            foreach (var ruleSet in RuleSets)
            {
                ruleSet.Reset?.Invoke();
            }
        }
    }

    public static class RuleUtils
    {
        public static ParameterizedFunction MixFn(ParametricSeries mix)
        {
            return (currentValue, target) => Mix(currentValue, target, mix);
        }
        public static ParameterizedFunction InterpolateFn(float interpolationAmount)
        {
            return (currentValue, target) => SeriesUtils.InterpolateInto(currentValue, target, interpolationAmount);
        }
        public static ParameterizedFunction SetValueFn(int index, Series value)
        {
	        return (currentValue, target) => SeriesUtils.SetSeriesAtIndex(currentValue, index, value);
        }
        public static ParameterizedFunction InterpolateWithConstantFn(Series constant, float amount)
        {
            return (currentValue, neighbors) => SeriesUtils.InterpolateInto(currentValue, constant, amount);// ignore neighbors
        }
        public static ParameterizedFunction ModifyNeighborsFn(ParameterizedFunction targetFn, params Func<Series, Series>[] neighborModifiers)
        {
            return (currentValue, neighbors) =>
            {
                for (int i = 0; i < neighborModifiers.Length; i++)
                {
                    neighbors = neighborModifiers[i](neighbors);
                }
                return targetFn(currentValue, neighbors);
            };
        }
        public static ParameterizedFunction ModifyResultsFn(ParameterizedFunction targetFn, params Func<Series, Series>[] resultsModifiers)
        {
            return (currentValue, neighbors) =>
            {
                var result = targetFn(currentValue, neighbors);
                for (int i = 0; i < resultsModifiers.Length; i++)
                {
                    result = resultsModifiers[i](result);
                }
                return result;
            };
        }
        public static ParameterizedFunction RandomColorFn(float min, float max)
        {
            return (currentValue, neighbors) => ColorAdapter.RandomColor(min, max, min, max, min, max);
        }

        public static Series Mix(Series source, Series target, ParametricSeries mix)
        {
            return new FloatSeries(source.VectorSize,
                source.X + (target.X - 0.5f) * mix.X,
                source.Y + (target.Y - 0.5f) * mix.Y,
                source.Z + (target.Z - 0.5f) * mix.Z);

        }
    }
}
