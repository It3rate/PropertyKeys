using System;
using DataArcs.Stores;

namespace DataArcs.Series
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
        public RandomSeries(int vectorSize, SeriesType type, int virtualCount, float min, float max, int seed = 0,
            CombineFunction combineFunction = CombineFunction.Add) : base(vectorSize, type, virtualCount)
        {
            _min = min;
            _max = max;
            seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
            _seed = seed;
            _random = new Random(_seed);
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
                var min = (int) _min;
                var max = (int) _max;
                for (var i = 0; i < len; i++) data[i] = _random.Next(min, max);
                result = new IntSeries(VectorSize, data);
            }
            else
            {
                var data = new float[len];
                for (var i = 0; i < len; i++) data[i] = (float) (_random.NextDouble() * (_max - _min) + _min);
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

        public override void Reset()
        {
            _random = new Random(_seed);
            GenerateData();
        }

        public override void Update(float time)
        {
            var b = GenerateData();
            _series.CombineInto(b, _combineFunction);
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

        public override void CombineInto(Series b, CombineFunction combineFunction)
        {
            _series.CombineInto(b, combineFunction);
        }

        public override float FloatDataAt(int index)
        {
            return _series[index];
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
    }
}