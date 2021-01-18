using System;
using System.Collections;
using System.Collections.Generic;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Samplers;

namespace Motive.SeriesData
{
    // todo: Random needs to be rewritten to reflect new approach to ISeries.
    // todo: split off Series wrapper base class
	public class RandomSeries : ISeries
	{
		private ISeries _series;
        private readonly CombineFunction _combineFunction;
		private Random _random;
		private RectFSeries _minMax;

		public string Name { get; set; }
		public int Id { get; private set; }
        public SeriesType Type { get; }
        public int Count => _series.Count;
		public int VectorSize
	    {
		    get => _series.VectorSize;
		    set { _series.VectorSize = value; }
	    }
	    public int DataSize => _series.DataSize;
        public DiscreteClampMode IndexClampMode { get; set; } = DiscreteClampMode.Clamp;
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
	    public float W { get; }

	    private ISeries _cachedSize;
	    private RectFSeries _cachedFrame;
        public RectFSeries Frame
	    {
		    get
		    {
			    if (_cachedFrame == null)
			    {
				    CalculateFrame();
			    }

			    return _cachedFrame;
		    }
		    protected set => _cachedFrame = value;
	    }

	    public ISeries Size
	    {
		    get
		    {
			    if (_cachedSize == null)
			    {
				    CalculateFrame();
			    }

			    return _cachedSize;
		    }
		    protected set => _cachedSize = value;
	    }


        private int _seed;
        public int Seed
        {
            get => _seed;
            set { _seed = value; GenerateDataSeries(VectorSize, Count); }
        }

		/// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int count, RectFSeries minMax = null, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace)
		{
			Type = type;
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
            _minMax = minMax ?? new RectFSeries(0,0,1f,1f);
			_combineFunction = combineFunction;
			_series = combineFunction == CombineFunction.ContinuousAdd ?
				SeriesUtils.CreateSeriesOfType(Type, vectorSize, count, 0f) :
				GenerateDataSeries(vectorSize, count);
		}
		
		public float[] this[int index] => _series.GetSeriesAt(index).FloatDataRef;
		
        public void ReverseEachElement()
		{
			_series.ReverseEachElement();
		}

		public Series GenerateDataSeries(int vectorSize, int count)
		{
			Series result;
            _random = new Random(_seed);
            var len = count * vectorSize;
			if (Type == SeriesType.Int)
			{
				var data = new int[len];
				for (var i = 0; i < count; i++)
				{
                    for (int j = 0; j < vectorSize; j++)
                    {
                        data[i * vectorSize + j] = _random.Next(_minMax.GetSeriesAt(0).IntValueAt(j), _minMax.GetSeriesAt(1).IntValueAt(j));
                    }
				}

				result = new IntSeries(vectorSize, data);
			}
			else
			{
				var data = new float[len];
                for (var i = 0; i < count; i++)
                {
                    for (int j = 0; j < vectorSize; j++)
                    {
                        float min = _minMax.GetSeriesAt(0).FloatValueAt(j);
                        float max = _minMax.GetSeriesAt(1).FloatValueAt(j);
                        data[i * vectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
				result = new FloatSeries(vectorSize, data);
			}

			return result;
		}


		public void Map(FloatEquation floatEquation)
		{
			_series.Map(floatEquation);
		}

		public void MapValuesToItemPositions(IntSeries items)
		{
			throw new NotImplementedException();
		}

		public void MapOrderToItemPositions(IntSeries items)
		{
			throw new NotImplementedException();
		}

		public void Append(Series series)
		{
			_series.Append(series);
		}

		public Series GetVirtualValueAt(float t)
		{
			return _series.GetVirtualValueAt(t);
		}

		public void ResetData()
		{
			_random = new Random(_seed);
			GenerateDataSeries(VectorSize, Count);
		}

        public bool AssignIdIfUnset(int id)
		{
			bool result = false;
			if (Id == 0 && id > 0)
			{
				Id = id;
				result = true;
			}
			return result;
        }

		public void OnActivate()
		{
		}

		public void OnDeactivate()
		{
		}

		public void Update(double currentTime, double deltaTime)
		{
            if(_combineFunction == CombineFunction.ContinuousAdd)
            {
	            _seed = SeriesUtils.Random.Next();
                var b = GenerateDataSeries(VectorSize, Count);
                float tSec = (float)(currentTime / 1000.0);
                var scaled = new FloatSeries(VectorSize, ArrayExtension.GetSizedFloatArray(VectorSize, tSec));
				b.CombineInto(scaled, CombineFunction.Multiply);
                _series.CombineInto(b, CombineFunction.Add);
            }
        }

		public void setMinMax(RectFSeries minMax)
		{
			_minMax = minMax;
		}

		protected void CalculateFrame()
		{
            Frame = (RectFSeries)_minMax.Copy();
            float[] max = _minMax.GetVirtualValueAt(1f).FloatDataRef;
            ArrayExtension.SubtractFloatArrayFrom(max, _minMax.GetVirtualValueAt(0).FloatDataRef);
            Size = new FloatSeries(VectorSize, max);
        }

		public void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0)
		{
			_series.CombineInto(b, combineFunction, t);
		}

		public float[] FloatDataRef => _series.FloatDataRef;
		public int[] IntDataRef => _series.IntDataRef;

		public ISeries GetSeriesAt(float t)
		{
			return _series.GetSeriesAt(t);
		}

		public Series GetSeriesAt(int index)
		{
			return _series.GetSeriesAt(index);
		}

		public void SetSeriesAt(int index, ISeries series)
		{
			_series.SetSeriesAt(index, series);
		}

        public float FloatValueAt(int index)
		{
			return _series.FloatValueAt(index);
		}

		public int IntValueAt(int index)
		{
			return _series.IntValueAt(index);
		}
		
		public void InterpolateInto(ISeries b, float t)
		{
			_series.InterpolateInto(b, t);
		}

		public void InterpolateInto(ISeries b, ParametricSeries seriesT)
		{
			_series.InterpolateInto(b, seriesT);
        }


        // TODO: This may not be right for random, should be random sampler?
		public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
		public IStore Store(Sampler sampler = null)
		{
			sampler = sampler ?? new LineSampler(this.Count);
			return new Store(this, sampler);
		}

		public List<ISeries> ToList()
		{
			return _series.ToList();
		}

		public void SetByList(List<ISeries> items)
		{
			_series.SetByList(items);
		}

		public ISeries Copy()
		{
			RandomSeries result = new RandomSeries(VectorSize, Type, Count, (RectFSeries)_minMax.Copy(), _seed);
			result._series = _series.Copy();
			return result;
		}

		public IEnumerator GetEnumerator()
		{
			return new ISeriesEnumerator(this);
		}
	}
}