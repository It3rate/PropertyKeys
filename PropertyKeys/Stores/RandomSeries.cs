using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class RandomSeries : Series
    {
        private CombineFunction _combineFunction;
        private Random _random;
        private readonly int _seed;
        private float _min;
        private float _max;

        private Series _series;

        /// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int virtualCount, float min, float max, int seed = 0, CombineFunction combineFunction = CombineFunction.Add) : base(vectorSize, type, virtualCount)
        {
            _min = min;
            _max = max;
            seed = seed == 0 ? DataUtils.Random.Next() : seed;
            _seed = seed;
            _random = new Random(_seed);
            _combineFunction = combineFunction;
            _series = GenerateData();
        }

        private Series GenerateData()
        {
            Series result;
            int len = VirtualCount * VectorSize;
            if (Type == SeriesType.Int)
            {
                int[] data = new int[len];
                int min = (int)_min;
                int max = (int)_max;
                for (int i = 0; i < len; i++)
                {
                    data[i] = _random.Next(min, max);
                }
                result = new IntSeries(VectorSize, data);
            }
            else
            {
                float[] data = new float[len];
                for (int i = 0; i < len; i++)
                {
                    data[i] = (float)(_random.NextDouble() * (_max - _min) + _min);
                }
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
            int len = VirtualCount * VectorSize;
            if ((int)(_series.DataSize / VectorSize) != len)
            {
                result = GenerateData();
            }
            return result;
        }

        public override void Reset()
        {
            _random = new Random(_seed);
            GenerateData();
        }
        public override void Update()
        {
            Series b = GenerateData();
            _series.Combine(b, _combineFunction);
        }

        public void setMinMax(float min, float max)
        {
            _min = min;
            _max = max;
        }
        protected override void CalculateFrame()
        {
            // nothing to do as internal series calculates it's own frame.
        }

        public override void Combine(Series b, CombineFunction combineFunction)
        {
            _series.Combine(b, combineFunction);
        }

        public override float FloatAt(int index)
        {
            return _series[index];
        }

        public override int IntAt(int index)
        {
            return _series.IntAt(index);
        }

        public override bool BoolAt(int index)
        {
            return _series.BoolAt(index);
        }

        public override float[] Floats => _series.Floats;
        public override int[] Ints => _series.Ints;
        public override bool[] Bools => _series.Bools;
        public override void Interpolate(Series b, float t)
        {
            _series.Interpolate(b, t);
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
    }
}
