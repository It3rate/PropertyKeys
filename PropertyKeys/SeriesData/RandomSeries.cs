﻿using System;
using DataArcs.Samplers;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class RandomSeries : Series
	{
		private SeriesType _type;
	    public override SeriesType Type => _type;

        public override int Count => _count;
        private int _count;
        private CombineFunction _combineFunction;
		private Random _random;
		private int _seed;
		private RectFSeries _minMax;
		private Series _series;

        public int Seed
        {
            get => _seed;
            set { _seed = value; GenerateDataSeries(); }
        }

		/// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int count, RectFSeries minMax = null, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace) : base(vectorSize)
		{
			_type = type;
            _count = count;
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
            _minMax = minMax ?? new RectFSeries(0,0,1f,1f);
			_combineFunction = combineFunction;
			_series = combineFunction == CombineFunction.ContinuousAdd ?
				SeriesUtils.CreateSeriesOfType(_type, VectorSize, _count, 0f) :
				GenerateDataSeries();
		}
		
		public float[] this[int index] => _series.GetRawDataAt(index).FloatDataRef;
		
        public override void ReverseEachElement()
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
                        data[i * VectorSize + j] = _random.Next(_minMax.GetRawDataAt(0).IntDataAt(j), _minMax.GetRawDataAt(1).IntDataAt(j));
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
                        float min = _minMax.GetRawDataAt(0).FloatDataAt(j);
                        float max = _minMax.GetRawDataAt(1).FloatDataAt(j);
                        data[i * VectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
				result = new FloatSeries(VectorSize, data);
			}

			return result;
		}

		public override int DataSize => _series.DataSize;

		public override Series GetRawDataAt(int index)
		{
			return _series.GetRawDataAt(index);
		}

		public override void SetRawDataAt(int index, Series series)
		{
			_series.SetRawDataAt(index, series);
		}

		public override void Append(Series series)
		{
			_series.Append(series);
		}

		public override Series GetVirtualValueAt(float t)
		{
			return _series.GetVirtualValueAt(t);
		}

		public override void ResetData()
		{
			_random = new Random(_seed);
			GenerateDataSeries();
		}

		public override void Update(float time)
		{
            if(_combineFunction == CombineFunction.ContinuousAdd)
            {
	            _seed = SeriesUtils.Random.Next();
                var b = GenerateDataSeries();
                float tSec = time / 1000f;
                var scaled = new FloatSeries(VectorSize, ArrayExtension.GetSizedFloatArray(VectorSize, tSec));
				b.CombineInto(scaled, CombineFunction.Multiply);
                _series.CombineInto(b, CombineFunction.Add);
            }
        }

		public void setMinMax(RectFSeries minMax)
		{
			_minMax = minMax;
		}

		protected override void CalculateFrame()
		{
            Frame = (RectFSeries)_minMax.Copy();
            float[] max = _minMax.GetVirtualValueAt(1f).FloatDataRef;
            ArrayExtension.SubtractFloatArrayFrom(max, _minMax.GetVirtualValueAt(0).FloatDataRef);
            Size = new FloatSeries(VectorSize, max);
        }

		public override void CombineInto(Series b, CombineFunction combineFunction, float t = 0)
		{
			_series.CombineInto(b, combineFunction, t);
		}

		public override float FloatDataAt(int index)
		{
			return _series.FloatDataAt(index);
		}

		public override int IntDataAt(int index)
		{
			return _series.IntDataAt(index);
		}
		

		public override float[] FloatDataRef => _series.FloatDataRef;
		public override int[] IntDataRef => _series.IntDataRef;

		public override void InterpolateInto(Series b, float t)
		{
			_series.InterpolateInto(b, t);
		}

		public override void InterpolateInto(Series b, ParametricSeries seriesT)
		{
			_series.InterpolateInto(b, seriesT);
        }

		public override Series Copy()
		{
			RandomSeries result = new RandomSeries(VectorSize, Type, Count, (RectFSeries)_minMax.Copy(), _seed);
			result._series = _series.Copy();
			return result;
		}
    }
}