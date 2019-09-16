using System;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class RandomSeries : Series
	{
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
        public RandomSeries(int vectorSize, SeriesType type, int virtualCount, Series minMax, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace) : base(vectorSize, type, virtualCount)
		{
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
			_random = new Random(_seed);
            _minMax = minMax;
			_combineFunction = combineFunction;
			_series = GenerateData();
		}

		private Series GenerateData()
		{
			Series result;
			var len = VirtualCount * VectorSize;
			if (Type == SeriesType.Int)
			{
				var data = new int[len];
				for (var i = 0; i < VirtualCount; i++)
				{
                    for (int j = 0; j < VectorSize; j++)
                    {
                        data[i * VectorSize + j] = _random.Next(_minMax.GetDataAtIndex(0).IntDataAt(j), _minMax.GetDataAtIndex(1).IntDataAt(j));
                    }
				}

				result = new IntSeries(VectorSize, data);
			}
			else
			{
				var data = new float[len];
                for (var i = 0; i < VirtualCount; i++)
                {
                    for (int j = 0; j < VectorSize; j++)
                    {
                        float min = _minMax.GetDataAtIndex(0).FloatDataAt(j);
                        float max = _minMax.GetDataAtIndex(1).FloatDataAt(j);
                        data[i * VectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
    //            for (var i = 0; i < len; i++)
				//{
				//	data[i] = (float) (_random.NextDouble() * (_max - _min) + _min);
				//}

				result = new FloatSeries(VectorSize, data);
			}

			return result;
		}

		public override int DataSize => _series.DataSize;

		public override Series GetDataAtIndex(int index)
		{
			return _series.GetDataAtIndex(index);
		}

		public override void SetDataAtIndex(int index, Series series)
		{
			_series.SetDataAtIndex(index, series);
		}

		public override Series GetValueAtT(float t)
		{
			return _series.GetValueAtT(t);
		}

		public override Series HardenToData(Store store = null)
		{
			Series result = this;
			var len = VirtualCount * VectorSize;
			if ((int) (_series.DataSize / VectorSize) != len)
			{
				result = GenerateData();
			}

			return result;
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
                var b = GenerateData();
                _series.CombineInto(b, CombineFunction.Add);
            }
        }

		public void setMinMax(Series minMax)
		{
			_minMax = minMax;
		}

		protected override void CalculateFrame()
		{
			// nothing to do as internal series calculates it's own frame.
		}

		public override void CombineInto(Series b, CombineFunction combineFunction)
		{
			_series.CombineInto(b, combineFunction);
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
			RandomSeries result = new RandomSeries(VectorSize, Type, VirtualCount, _minMax.Copy(), _seed);
			result.CachedFrame = CachedFrame;
			result.CachedSize = CachedSize;
			result._series = _series.Copy();
			return result;
		}
    }
}