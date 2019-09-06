using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class RandomSeries : Series
    {
        private readonly Random _random;
        private readonly int _seed;
        private readonly float _min;
        private readonly float _max;

        private Series _series;

        /// <summary>
        /// RandomSeries always has an actual store in order to be consistent on repeated queries.
        /// </summary>
        public RandomSeries(int vectorSize, SeriesType type, int virtualCount, float min, float max, int seed = 0) : base(vectorSize, type, virtualCount)
        {
            _min = min;
            _max = max;
            seed = seed == 0 ? DataUtils.Random.Next() : seed;
            _seed = seed;
            _random = new Random(_seed);
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

        public override Series GetValueAtIndex(int index)
        {
            return _series.GetValueAtIndex(index);
        }

        public override Series GetValueAtT(float t)
        {
            return _series.GetValueAtT(t);
        }
        
        public override Series HardenToData(Store store)
        {
            Series result = this;
            int len = VirtualCount * VectorSize;
            if ((int)(_series.DataSize / VectorSize) != len)
            {
                result = GenerateData();
            }
            return result;
        }


        protected override void CalculateFrame()
        {
            // nothing to do as internal series calculates it's own frame.
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
