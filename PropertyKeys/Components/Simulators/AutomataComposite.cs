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
	    private ParamterizedFunction currentFn1 = Average1;
	    private ParamterizedFunction currentFn2 = RandomMid;
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
		
        public delegate Series ParamterizedFunction(Series currentValue, Series neighbors);


        private static readonly ParamterizedFunction Darken = InterpolateWithConstantFn(Colors.Black, 0.3f);
        private static readonly ParamterizedFunction DarkenSmall = InterpolateWithConstantFn(Colors.Black, 0.08f);
        private static readonly ParamterizedFunction Lighten = InterpolateWithConstantFn(Colors.White, 0.1f);
        private static readonly ParamterizedFunction Reden = InterpolateWithConstantFn(Colors.Red, 0.1f);
        private static readonly ParamterizedFunction Average1 = ModifyNeighborsFn(InterpolateFn(0.1f), SeriesUtils.Average);
        private static readonly ParamterizedFunction AverageMaxHigh = ModifyNeighborsFn(InterpolateFn(0.95f), SeriesUtils.Max);
        private static readonly ParamterizedFunction RandomAny = RandomColor(0, 1f);
        private static readonly ParamterizedFunction RandomMid = RandomColor(0.3f, 0.7f);
        private static readonly ParamterizedFunction RandomDark = RandomColor(0.0f, 0.3f);
		

        private static Series Mix1(Series currentValue, Series neighbors)
        {
	        return AverageMix(currentValue, neighbors, 0.1f, 0.2f, 0.3f);
        }

        private static Series MaxNeighborPart(Series currentValue, Series neighbors)
        {
			currentValue.InterpolateInto(neighbors.Max(), .2f);
			return currentValue;
        }

        private static Series MinNeighborPart(Series currentValue, Series neighbors)
        {
	        currentValue.InterpolateInto(neighbors.Min(), .99f);
	        return currentValue;
        }

        private static Series MaxNeighbor(Series currentValue, Series neighbors)
        {
	        var scale = 0.97f;
	        var result = neighbors.Max();
	        result.SetSeriesAtIndex(0, new FloatSeries(result.VectorSize, result.X * scale, result.Y * scale, result.Z * scale));
	        return result;
        }
        private static Series MinNeighbor(Series currentValue, Series neighbors)
        {
	        var scale = 0.97f;
	        var result = neighbors.Min();
	        result.SetSeriesAtIndex(0, new FloatSeries(result.VectorSize, result.X * scale, result.Y * scale, result.Z * scale));
	        return result;
        }






        private static ParamterizedFunction InterpolateFn(float interpolationAmount)
        {
	        return (currentValue, target) => SeriesUtils.InterpolateInto(currentValue, target, interpolationAmount);
        }
        private static ParamterizedFunction InterpolateWithConstantFn(Series constant, float amount)
        {
	        return (currentValue, neighbors) => SeriesUtils.InterpolateInto(currentValue, constant, amount);// ignore neighbors
        }
        private static ParamterizedFunction ModifyNeighborsFn(ParamterizedFunction targetFn, params Func<Series, Series>[] neighborModifiers)
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
        private static ParamterizedFunction ModifyResultsFn(ParamterizedFunction targetFn, params Func<Series, Series>[] resultsModifiers)
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
        private static ParamterizedFunction RandomColor(float min, float max)
        {
	        return (currentValue, neighbors) => ColorAdapter.RandomColor(min, max, min, max, min, max);
        }

		
	    private static Series AverageMix(Series currentValue, Series neighbors, float mixR, float mixG, float mixB)
	    {
		    var average = neighbors.Average();
		    return new FloatSeries(currentValue.VectorSize,
			    Math.Max(0, Math.Min(1, currentValue.X + (average.X - 0.5f) * mixR)),
			    Math.Max(0, Math.Min(1, currentValue.Y + (average.Y - 0.5f) * mixG)),
			    Math.Max(0, Math.Min(1, currentValue.Z + (average.Z - 0.5f) * mixB)));
	    }

    }
}
