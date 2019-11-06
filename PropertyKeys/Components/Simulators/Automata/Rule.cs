using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components.Simulators.Automata
{
	public delegate bool Condition(Series currentValue, Series neighbors, Runner runner);
	public delegate Series ParameterizedFunction(Series currentValue, Series neighbors);
	public delegate Series SeriesModifier(Series elements);

    public class Rule
	{
		public Condition Condition { get; }
		public ParameterizedFunction ParameterizedFunction { get; }

		private CombineFunction CombineFunction { get; set; } = CombineFunction.Final;

        public Rule(Condition condition, ParameterizedFunction parameterizedFunction)
		{
			Condition = condition;
			ParameterizedFunction = parameterizedFunction;
		}

		/// <summary>
        /// Invokes the Rule with a value and evaluation series if the Rule's condition evaluates to true.
        /// Modifies the currentValue to hold the result of the Rule evaluation if executed.
        /// Returns true if able to continue chaining the output.
        /// </summary>
        /// <param name="currentValue">Value to adjust.</param>
        /// <param name="neighbors">Parameters used in the potiential adjustment.</param>
        /// <returns>Returns true if not a final value.</returns>
        public bool Invoke(Series currentValue, Series neighbors, Runner runner)
        {
	        bool canContinue = true;
	        if (Condition(currentValue, neighbors, runner))
	        {
		        canContinue = CombineFunction != CombineFunction.Final;
		        var values = ParameterizedFunction(currentValue, neighbors);
				currentValue.CombineInto(values, CombineFunction);
	        }
	        return canContinue;
        }

		// Evaluators

        public static ParameterizedFunction MixFn(ParametricSeries mix)
        {
            return (currentValue, target) => RuleUtils.Mix(currentValue, target, mix);
        }
        public static ParameterizedFunction InterpolateFn(float interpolationAmount)
        {
            return (currentValue, target) => SeriesUtils.InterpolateInto(currentValue, target, interpolationAmount);
        }
        public static ParameterizedFunction InterpolateWithConstantFn(Series constant, float amount)
        {
            return (currentValue, neighbors) => SeriesUtils.InterpolateInto(currentValue, constant, amount);// ignore neighbors
        }
        public static ParameterizedFunction SetSeriesAtIndexFn(int index, Series value)
        {
            return (currentValue, target) => SeriesUtils.SetSeriesAtIndex(currentValue, index, value);
        }
        public static ParameterizedFunction EvaluateNeighborsFn(ParameterizedFunction targetFn, params SeriesModifier[] neighborModifiers)
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
        public static ParameterizedFunction ModifyResultsFn(ParameterizedFunction targetFn, params SeriesModifier[] resultsModifiers)
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

        public static ParameterizedFunction EvalAndInterpolateFn(SeriesModifier evaluator, float interpolationAmount) => EvaluateNeighborsFn(InterpolateFn(interpolationAmount), evaluator);


        // Conditions
        public static Condition RandomChance(float chance) => (currentValue, neighbors, runner) => SeriesUtils.Random.NextDouble() < chance;
        public static Condition PassCountIsUnder(int minCount) => (currentValue, neighbors, runner) => runner.PassCount < minCount;
        public static Condition PassCountIsOver(int maxCount) => (currentValue, neighbors, runner) => runner.PassCount > maxCount;

        public static Condition CurrentValueIsUnder(Slot slot, float value) => (currentValue, neighbors, runner) => SlotUtils.GetFloatAt(currentValue, slot) < value;
        public static Condition CurrentValueIsOver(Slot slot, float value) => (currentValue, neighbors, runner) => SlotUtils.GetFloatAt(currentValue, slot) > value;
        public static Condition CurrentValueIsNear(Slot slotA, Slot slotB, float maxDelta) => (currentValue, neighbors, runner) => Math.Abs(SlotUtils.GetFloatAt(currentValue, slotA) - SlotUtils.GetFloatAt(currentValue, slotB)) < maxDelta;
		
    }

}
