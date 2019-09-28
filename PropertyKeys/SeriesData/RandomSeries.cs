using System;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class RandomSeries : Series
    {
        public override int Count => _count;
        private int _count;
        private CombineFunction _combineFunction;
		private Random _random;
		private int _seed;
		private Series _minMax;
		private Series _series;

        public int Seed
        {
            get => _seed;
            set { _seed = value; GenerateData(); }
        }

		/// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int count, Series minMax, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace) : base(vectorSize, type)
		{
            _count = count;
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
            _minMax = minMax;
			_combineFunction = combineFunction;
			_series = combineFunction == CombineFunction.ContinuousAdd ? SeriesUtils.GetZeroFloatSeries(vectorSize, _count) : GenerateData();
		}

		private Series GenerateData()
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
                        data[i * VectorSize + j] = _random.Next(_minMax.GetSeriesAtIndex(0).IntDataAt(j), _minMax.GetSeriesAtIndex(1).IntDataAt(j));
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
                        float min = _minMax.GetSeriesAtIndex(0).FloatDataAt(j);
                        float max = _minMax.GetSeriesAtIndex(1).FloatDataAt(j);
                        data[i * VectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
				result = new FloatSeries(VectorSize, data);
			}

			return result;
		}

		public override int DataSize => _series.DataSize;

		public override Series GetSeriesAtIndex(int index)
		{
			return _series.GetSeriesAtIndex(index);
		}

		public override void SetSeriesAtIndex(int index, Series series)
		{
			_series.SetSeriesAtIndex(index, series);
		}

		public override Series GetValueAtT(float t)
		{
			return _series.GetValueAtT(t);
		}

		public override void ResetData()
		{
			_random = new Random(_seed);
			GenerateData();
		}

		public override void Update(float time)
		{
            if(_combineFunction == CombineFunction.ContinuousAdd)
            {
	            _seed = SeriesUtils.Random.Next();
                var b = GenerateData();
                float tSec = time / 1000f;
                var scaled = new FloatSeries(VectorSize, SeriesUtils.GetSizedFloatArray(VectorSize, tSec));
				b.CombineInto(scaled, CombineFunction.Multiply);
                _series.CombineInto(b, CombineFunction.Add);
            }
        }

		public void setMinMax(Series minMax)
		{
			_minMax = minMax;
		}

		protected override void CalculateFrame()
		{
            Frame = _minMax.Copy();
            float[] max = _minMax.GetValueAtT(1f).FloatData;
            SeriesUtils.SubtractFloatArrayFrom(max, _minMax.GetValueAtT(0).FloatData);
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

		public override bool BoolDataAt(int index)
		{
			return _series.BoolDataAt(index);
		}

		public override float[] FloatData => _series.FloatData;
		public override int[] IntData => _series.IntData;
		public override bool[] BoolData => _series.BoolData;

		public override void InterpolateInto(Series b, float t)
		{
			_series.InterpolateInto(b, t);
		}

		public override Series GetZeroSeries()
		{
			return _series.GetZeroSeries();
		}

		public override Series GetZeroSeries(int elements)
		{
			return _series.GetZeroSeries(elements);
		}

		public override Series GetMinSeries()
		{
			return _series.GetMinSeries();
		}

		public override Series GetMaxSeries()
		{
			return _series.GetMaxSeries();
		}
		public override Series Copy()
		{
			RandomSeries result = new RandomSeries(VectorSize, Type, Count, _minMax.Copy(), _seed);
			result._series = _series.Copy();
			return result;
		}
    }
}