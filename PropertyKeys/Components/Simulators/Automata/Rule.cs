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
        /// <returns>Returns true if not a final compareValue.</returns>
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
        public static ParameterizedFunction ConstInterpFn(Series constant, float amount)
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
        public static ParameterizedFunction RandomColorFn(float minR, float maxR, float minG, float maxG, float minB, float maxB)
        {
	        return (currentValue, neighbors) => ColorAdapter.RandomColor(minR, maxR, minG, maxG, minB, maxB);
        }
        public static ParameterizedFunction ShuffleValuesFn()
        {
	        return (currentValue, neighbors) =>
	        {
		        var floats = currentValue.FloatDataRef;
		        for (int i = 0; i < floats.Length; i++)
		        {
			        int indexA = SeriesUtils.Random.Next(floats.Length);
			        int indexB = SeriesUtils.Random.Next(floats.Length);
			        float temp = floats[indexA];
			        floats[indexA] = floats[indexB];
			        floats[indexB] = temp;
		        }
		        return currentValue;
	        };
        }

        public static ParameterizedFunction EvalInterpFn(SeriesModifier evaluator, float interpolationAmount) => EvaluateNeighborsFn(InterpolateFn(interpolationAmount), evaluator);
        public static ParameterizedFunction NopFn() => (currentValue, neighbors) => currentValue;

        public static ParameterizedFunction CombineFn(params ParameterizedFunction[] functions)
        {
	        return (currentValue, neighbors) =>
	        {
		        var result = currentValue;
		        for (int i = 0; i < functions.Length; i++)
		        {
			        result = functions[i](result, neighbors);
		        }
		        return result;
	        };
        }


        // Conditions
        public static Condition AlwaysTrue() => (currentValue, neighbors, runner) => true;
        public static Condition AlwaysFalse() => (currentValue, neighbors, runner) => false;
        public static Condition RandomChance(float chance) => (currentValue, neighbors, runner) => SeriesUtils.Random.NextDouble() < chance;
        public static Condition PassCountIsUnder(int minCount) => (currentValue, neighbors, runner) => runner.PassCount < minCount;
        public static Condition PassCountIsOver(int maxCount) => (currentValue, neighbors, runner) => runner.PassCount > maxCount;

        public static Condition CurrentValueIsUnder(Slot slot, float value) => (currentValue, neighbors, runner) => SlotUtils.GetFloatAt(currentValue, slot) < value;
        public static Condition CurrentValueIsOver(Slot slot, float value) => (currentValue, neighbors, runner) => SlotUtils.GetFloatAt(currentValue, slot) > value;
        public static Condition CurrentValueIsNear(Slot slotA, Slot slotB, float maxDelta) => (currentValue, neighbors, runner) => 
	        Math.Abs(SlotUtils.GetFloatAt(currentValue, slotA) - SlotUtils.GetFloatAt(currentValue, slotB)) < maxDelta;
        public static Condition CurrentValueIsFar(Slot slotA, Slot slotB, float minDelta) => (currentValue, neighbors, runner) => 
	        Math.Abs(SlotUtils.GetFloatAt(currentValue, slotA) - SlotUtils.GetFloatAt(currentValue, slotB)) > minDelta;

        public static Condition NeighboursEvaluationIsOver(SeriesModifier neighborModifier, Slot slot, float compareValue)
        {
	        return (currentValue, neighbors, runner) =>
	        {
		        var mod = neighborModifier(neighbors);
		        float val = SlotUtils.GetFloatAt(mod, slot);
		        return val > compareValue;
	        };
        }
        public static Condition NeighboursEvaluationIsUnder(SeriesModifier neighborModifier, Slot slot, float compareValue)
        {
	        return (currentValue, neighbors, runner) =>
	        {
		        var mod = neighborModifier(neighbors);
		        float val = SlotUtils.GetFloatAt(mod, slot);
		        return val < compareValue;
	        };
        }
        public static Condition NeighboursEvaluationIsNear(SeriesModifier neighborModifier, Slot slotA, Slot slotB, float maxDelta)
        {
	        return (currentValue, neighbors, runner) =>
	        {
		        var mod = neighborModifier(neighbors);
		        float valA = SlotUtils.GetFloatAt(mod, slotA);
		        float valB = SlotUtils.GetFloatAt(mod, slotB);
		        return Math.Abs(valA - valB) < maxDelta;
	        };
        }
        public static Condition NeighboursEvaluationIsFar(SeriesModifier neighborModifier, Slot slotA, Slot slotB, float minDelta)
        {
	        return (currentValue, neighbors, runner) =>
	        {
		        var mod = neighborModifier(neighbors);
		        float valA = SlotUtils.GetFloatAt(mod, slotA);
		        float valB = SlotUtils.GetFloatAt(mod, slotB);
		        return Math.Abs(valA - valB) > minDelta;
	        };
        }

        public static Condition AllConditionsTrue(params Condition[] conditions) => (currentValue, neighbors, runner) =>
        {
	        bool result = true;
	        for (int i = 0; i < conditions.Length; i++)
	        {
		        if (!conditions[i].Invoke(currentValue, neighbors, runner))
		        {
			        result = false;
			        break;
		        }
	        }

	        return result;
        };
        public static Condition AllConditionsFalse(params Condition[] conditions) => (currentValue, neighbors, runner) =>
        {
	        bool result = true;
	        for (int i = 0; i < conditions.Length; i++)
	        {
		        if (conditions[i].Invoke(currentValue, neighbors, runner))
		        {
			        result = false;
			        break;
		        }
	        }

	        return result;
        };
        public static Condition OneConditionTrue(params Condition[] conditions) => (currentValue, neighbors, runner) =>
        {
	        bool result = false;
	        for (int i = 0; i < conditions.Length; i++)
	        {
		        if (conditions[i].Invoke(currentValue, neighbors, runner))
		        {
			        result = true;
			        break;
		        }
	        }

	        return result;
        };
	}

}
