using System;
using System.Collections;
using System.Collections.Generic;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Samplers;

namespace Motive.SeriesData
{
	public class RandomSeries : ISeries
	{
		private ISeries _series;
        private readonly CombineFunction _combineFunction;
		private Random _random;
		private RectFSeries _minMax;

	    public SeriesType Type { get; }
	    public int Count { get; }
	    public int VectorSize { get; set; }
	    public float W { get; }
	    public int DataSize => _series.DataSize;
        public DiscreteClampMode IndexClampMode { get; set; } = DiscreteClampMode.Clamp;
        public RectFSeries Frame { get; private set; }
        public Series Size { get; private set; }
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        private int _seed;
        public int Seed
        {
            get => _seed;
            set { _seed = value; GenerateDataSeries(); }
        }

		/// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int count, RectFSeries minMax = null, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace)
		{
			Type = type;
            Count = count;
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
            _minMax = minMax ?? new RectFSeries(0,0,1f,1f);
			_combineFunction = combineFunction;
			_series = combineFunction == CombineFunction.ContinuousAdd ?
				SeriesUtils.CreateSeriesOfType(Type, vectorSize, Count, 0f) :
				GenerateDataSeries();
		}
		
		public float[] this[int index] => _series.GetSeriesAt(index).FloatDataRef;
		
        public void ReverseEachElement()
		{
			_series.ReverseEachElement();
		}

		public Series GenerateDataSeries()
		{
			Series result;
            _random = new Random(_seed);
            var len = Count * VectorSize;
			if (Type == SeriesType.Int)
			{
				var data = new int[len];
				for (var i = 0; i < Count; i++)
				{
                    for (int j = 0; j < VectorSize; j++)
                    {
                        data[i * VectorSize + j] = _random.Next(_minMax.GetSeriesAt(0).IntValueAt(j), _minMax.GetSeriesAt(1).IntValueAt(j));
                    }
				}

				result = new IntSeries(VectorSize, data);
			}
			else
			{
				var data = new float[len];
                for (var i = 0; i < Count; i++)
                {
                    for (int j = 0; j < VectorSize; j++)
                    {
                        float min = _minMax.GetSeriesAt(0).FloatValueAt(j);
                        float max = _minMax.GetSeriesAt(1).FloatValueAt(j);
                        data[i * VectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
				result = new FloatSeries(VectorSize, data);
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
			GenerateDataSeries();
		}

		public string Name { get; set; }
		public int Id { get; }
		public bool AssignIdIfUnset(int id)
		{
			throw new NotImplementedException();
		}

		public void OnActivate()
		{
			throw new NotImplementedException();
		}

		public void OnDeactivate()
		{
			throw new NotImplementedException();
		}

		public void Update(double currentTime, double deltaTime)
		{
            if(_combineFunction == CombineFunction.ContinuousAdd)
            {
	            _seed = SeriesUtils.Random.Next();
                var b = GenerateDataSeries();
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

		public void CombineInto(Series b, CombineFunction combineFunction, float t = 0)
		{
			_series.CombineInto(b, combineFunction, t);
		}

		public float[] FloatDataRef => _series.FloatDataRef;
		public int[] IntDataRef => _series.IntDataRef;

		public Series GetSeriesAt(float t)
		{
			return _series.GetSeriesAt(t);
		}

		public Series GetSeriesAt(int index)
		{
			return _series.GetSeriesAt(index);
		}

		public void SetSeriesAt(int index, Series series)
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
		
		public void InterpolateInto(Series b, float t)
		{
			_series.InterpolateInto(b, t);
		}

		public void InterpolateInto(Series b, ParametricSeries seriesT)
		{
			_series.InterpolateInto(b, seriesT);
        }


        // TODO: Need to convert everything to ISeries
		public Store CreateLinearStore(int capacity) => new Store((Series)_series, new LineSampler(capacity));
		public IStore Store(Sampler sampler = null)
		{
			sampler = sampler ?? new LineSampler(this.Count);
			return new Store((Series)_series, sampler);
		}

		public List<Series> ToList()
		{
			return _series.ToList();
		}

		public void SetByList(List<Series> items)
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