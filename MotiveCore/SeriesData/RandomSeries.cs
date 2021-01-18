using System;
using System.Collections;
using System.Collections.Generic;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Samplers;

namespace Motive.SeriesData
{
    // todo: Random needs to be rewritten to reflect new approach to ISeries.
	public class RandomSeries : WrappedSeries
	{
        private readonly CombineFunction _combineFunction;
		private Random _random;

        private readonly SeriesType _type;
		public override SeriesType Type => _type;

		private RectFSeries _minMax;
		private RectFSeries MinMax
		{
			get => _minMax;
			set
			{
				_minMax = value;
				CalculateFrame();
			}
		}

        private int _seed;
        public int Seed
        {
            get => _seed;
            set { _seed = value; GenerateDataSeries(VectorSize, Count); }
        }
        private ISeries _cachedSize;
        public override ISeries Size => _cachedSize;
        private RectFSeries _cachedFrame;
        public override RectFSeries Frame => _cachedFrame;

        /// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int count, RectFSeries minMax = null, int seed = 0,
			CombineFunction combineFunction = CombineFunction.Replace)
        {
	        _type = type;
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
			_combineFunction = combineFunction;
			_minMax = minMax ?? new RectFSeries(0, 0, 1f, 1f);
            _series = combineFunction == CombineFunction.ContinuousAdd ?
				SeriesUtils.CreateSeriesOfType(Type, vectorSize, count, 0f) :
				GenerateDataSeries(vectorSize, count);
            CalculateFrame();
		}
		
		public SeriesBase GenerateDataSeries(int vectorSize, int count)
		{
			SeriesBase result;
            _random = new Random(_seed);
            var len = count * vectorSize;
			if (Type == SeriesType.Int)
			{
				var data = new int[len];
				for (var i = 0; i < count; i++)
				{
                    for (int j = 0; j < vectorSize; j++)
                    {
                        data[i * vectorSize + j] = _random.Next(MinMax.GetSeriesAt(0).IntValueAt(j), MinMax.GetSeriesAt(1).IntValueAt(j));
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
                        float min = MinMax.GetSeriesAt(0).FloatValueAt(j);
                        float max = MinMax.GetSeriesAt(1).FloatValueAt(j);
                        data[i * vectorSize + j] = (float)(_random.NextDouble() * (max - min) + min);
                    }
                }
				result = new FloatSeries(vectorSize, data);
			}

			return result;
		}

		public override void ResetData()
		{
			_random = new Random(_seed);
			GenerateDataSeries(VectorSize, Count);
		}

		public override void Update(double currentTime, double deltaTime)
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

		protected void CalculateFrame()
		{
            _cachedFrame = (RectFSeries)MinMax.Copy();
            float[] max = MinMax.GetVirtualValueAt(1f).FloatDataRef;
            ArrayExtension.SubtractFloatArrayFrom(max, MinMax.GetVirtualValueAt(0).FloatDataRef);
            _cachedSize = new FloatSeries(VectorSize, max);
        }

        // TODO: This may not be right for random, should be random sampler?
		public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
		public IStore Store(Sampler sampler = null)
		{
			sampler = sampler ?? new LineSampler(this.Count);
			return new Store(this, sampler);
		}

		public ISeries Copy()
		{
			RandomSeries result = new RandomSeries(VectorSize, Type, Count, (RectFSeries)MinMax.Copy(), _seed);
			result._series = _series.Copy();
			return result;
		}
	}
}