using System;
using System.Collections.Generic;
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
	    private IStore _automata;
	    private IStore _previousAutomata;
        public override int Capacity { get => _automata.Capacity; set { } }

	    public AutomataComposite(IStore itemStore, IStore automataStore) : base(itemStore)
	    {
		    _automata = automataStore;
		    _previousAutomata = _automata.Clone();
			if (_automata != null)
			{
				AddProperty(PropertyId.Automata, _automata);
			}
        }

	    private int _delayCount = 0;
	    private ParameterizedFunction currentFn1 = Average1;
	    private ParameterizedFunction currentFn2 = RandomMid;
	    private bool isBusy = false;

	    bool block1 = false;
	    private int blockIndex = 0;
	    private int count = 50;

        public override void StartUpdate(float currentTime, float deltaTime)
	    {
		    if (!isBusy)
		    {
			    isBusy = true;
			    base.StartUpdate(currentTime, deltaTime);
			    _delayCount++;
			    if (true)//(_delayCount % 10 == 8)
			    {
				    if (SeriesUtils.Random.NextDouble() < 0.006 && count > 100)
				    {
                        block1 = !block1;
                        blockIndex = 0;
                        count = 0;
					    currentFn1 = Average1;
						currentFn2 = RandomMid;
					}
				    count++;
				    blockIndex += 100;

                    int capacity = Capacity;
                    _automata.CopySeriesDataInto(_previousAutomata);
				    float rnd0 = (float)SeriesUtils.Random.NextDouble();
                    for (int i = 0; i < capacity; i++)
				    {
					    var currentValue = _previousAutomata.GetFullSeries().GetValueAtVirtualIndex(i, capacity);
                        var neighbors = _previousAutomata.GetNeighbors(i);
                        if (block1 && blockIndex > i)
                        {
	                        if (count < 20)
	                        {
		                        DarkenSmall(currentValue, neighbors);
	                        }
                            else if (SeriesUtils.Random.NextDouble() < 0.001)
                            {
                                currentValue = RandomAny(currentValue, neighbors);
                            }
                            else if (rnd0 < 0.01)
                            {
	                            currentValue = DarkenSmall(currentValue, neighbors);
                            }
                            else if (Math.Abs(currentValue.X - currentValue.Y) < 0.005f)
                            {
	                            currentValue = RandomDark(currentValue, neighbors);
                            }
                            else if (currentValue.Y < 0.3)
                            {
                                currentValue = MaxNeighborPart(currentValue, neighbors);
                            }
                            else if (currentValue.Z > 0.2)
                            {
                                currentValue = MinNeighborPart(currentValue, neighbors);
                            }
                        }
                        else
                        {
	                        if (count < 40)
	                        {
		                        DarkenSmall(currentValue, neighbors);
                            }
	                        else if (count < 42 && SeriesUtils.Random.NextDouble() < 0.0003)
	                        {
		                        currentValue.SetSeriesAtIndex(0, new FloatSeries(currentValue.VectorSize, 0, 0, 0.7f));
                            }
                            else if (SeriesUtils.Random.NextDouble() < 0.00001)
	                        {
		                        currentValue = RandomAny(currentValue, neighbors);
	                        }
	                        else if (neighbors.Max().Z > 0.99)
	                        {
		                        currentValue.SetSeriesAtIndex(0, new FloatSeries(currentValue.VectorSize, 0, 0, 0));
	                        }
	                        else if (neighbors.Max().Y > 0.99 && neighbors.Min().Y < 0.01)
	                        {
		                        currentValue = RandomAny(currentValue, neighbors);
	                        }
	                        else if (currentValue.Z < 0.1f) // && org.X < 1f)
	                        {
		                        currentValue = AverageMaxHigh(currentValue, neighbors);
	                        }
	                        else if (currentValue.X > 0.90f) // && org.X < 1f)
	                        {
		                        currentValue = currentFn1(currentValue, neighbors);
	                        }
	                        else if (Math.Abs(currentValue.X - currentValue.Y) > 0.9f)
	                        {
		                        currentValue = currentFn2(currentValue, neighbors);
	                        }
	                        else if (currentValue.Z < 0.1f || currentValue.Z > 0.94f)
	                        {
		                        var temp = currentFn1;
		                        currentFn1 = currentFn2;
		                        currentFn2 = temp;
		                        //currentValue.FloatDataRef[2] = 0.3f;
	                        }
	                        else if (neighbors.Average().Y < 0.001f)
	                        {
		                        currentValue = Mix1(currentValue, neighbors);
	                        }
	                        else
	                        {
		                        currentValue = Mix1(currentValue, neighbors);
	                        }
                        }

                        _automata.GetFullSeries().SetSeriesAtIndex(i, currentValue);
				    }
			    }

			    isBusy = false;
		    }
	    }
		
        public delegate Series ParameterizedFunction(Series currentValue, Series neighbors);


		// experimental functions
        private static readonly ParameterizedFunction Darken = InterpolateWithConstantFn(Colors.Black, 0.3f);
        private static readonly ParameterizedFunction DarkenSmall = InterpolateWithConstantFn(Colors.Black, 0.08f);
        private static readonly ParameterizedFunction Lighten = InterpolateWithConstantFn(Colors.White, 0.1f);
        private static readonly ParameterizedFunction Average1 = ModifyNeighborsFn(InterpolateFn(0.1f), SeriesUtils.Average);
        private static readonly ParameterizedFunction AverageMaxHigh = ModifyNeighborsFn(InterpolateFn(0.95f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MaxNeighborPart = ModifyNeighborsFn(InterpolateFn(0.2f), SeriesUtils.Max);
        private static readonly ParameterizedFunction MinNeighborPart = ModifyNeighborsFn(InterpolateFn(0.99f), SeriesUtils.Min);
        private static readonly ParameterizedFunction Mix1 = ModifyResultsFn(
            ModifyNeighborsFn( MixFn(new ParametricSeries(3, 0.1f, 0.2f, 0.3f)), SeriesUtils.Average),SeriesUtils.ClampTo01);
        private static readonly ParameterizedFunction RandomAny = RandomColorFn(0, 1f);
        private static readonly ParameterizedFunction RandomMid = RandomColorFn(0.3f, 0.7f);
        private static readonly ParameterizedFunction RandomDark = RandomColorFn(0.0f, 0.3f);
		
		
        private static ParameterizedFunction MixFn(ParametricSeries mix)
        {
	        return (currentValue, target) => Mix(currentValue, target, mix);
        }
        private static ParameterizedFunction InterpolateFn(float interpolationAmount)
        {
	        return (currentValue, target) => SeriesUtils.InterpolateInto(currentValue, target, interpolationAmount);
        }
        private static ParameterizedFunction InterpolateWithConstantFn(Series constant, float amount)
        {
	        return (currentValue, neighbors) => SeriesUtils.InterpolateInto(currentValue, constant, amount);// ignore neighbors
        }
        private static ParameterizedFunction ModifyNeighborsFn(ParameterizedFunction targetFn, params Func<Series, Series>[] neighborModifiers)
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
        private static ParameterizedFunction ModifyResultsFn(ParameterizedFunction targetFn, params Func<Series, Series>[] resultsModifiers)
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
        private static ParameterizedFunction RandomColorFn(float min, float max)
        {
	        return (currentValue, neighbors) => ColorAdapter.RandomColor(min, max, min, max, min, max);
        }

        private static Series Mix(Series source, Series target, ParametricSeries mix)
        {
	        return new FloatSeries(source.VectorSize,
		        source.X + (target.X - 0.5f) * mix.X,
		        source.Y + (target.Y - 0.5f) * mix.Y,
		        source.Z + (target.Z - 0.5f) * mix.Z);

        }
    }
}
