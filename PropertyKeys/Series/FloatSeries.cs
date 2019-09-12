using System;
using DataArcs.Stores;

namespace DataArcs.Series
{
    public class FloatSeries : Series
    {
        protected readonly float[] _floatValues;
        public override int DataSize => _floatValues.Length;

        public FloatSeries(int vectorSize, float[] values, int virtualCount = -1) :
            base(vectorSize, SeriesType.Float, virtualCount <= 0 ? values.Length / vectorSize : virtualCount)
        {
            _floatValues = values;
        }

        public FloatSeries(int vectorSize, params float[] values) :
            base(vectorSize, SeriesType.Float, values.Length / vectorSize)
        {
            _floatValues = values;
        }

        public override Series GetSeriesAtIndex(int index)
        {
            var len = DataSize / VectorSize;
            var startIndex = Math.Min(len - 1, Math.Max(0, index));
            var result = new float[VectorSize];
            if (startIndex * VectorSize + VectorSize <= DataSize)
            {
                Array.Copy(_floatValues, startIndex * VectorSize, result, 0, VectorSize);
            }
            else
            {
                Array.Copy(_floatValues, DataSize - VectorSize, result, 0, VectorSize);
            }

            return new FloatSeries(VectorSize, result);
        }

        public override void SetSeriesAtIndex(int index, Series series)
        {
            var len = DataSize / VectorSize;
            var startIndex = Math.Min(len - 1, Math.Max(0, index));
            Array.Copy(series.FloatData, 0, _floatValues, startIndex * VectorSize, VectorSize);
        }

        public override Series HardenToData(Store store = null)
        {
            Series result = this;
            var len = VirtualCount * VectorSize;
            if (_floatValues.Length != len)
            {
                var vals = new float[len];
                for (var i = 0; i < VirtualCount; i++)
                {
                    var val = store == null ? GetSeriesAtIndex(i).FloatData : store.GetValueAtIndex(i).FloatData;
                    Array.Copy(val, 0 * VectorSize, vals, i * VectorSize, VectorSize);
                }

                result = new FloatSeries(VectorSize, vals, VirtualCount);
            }

            return result;
        }

        protected override void CalculateFrame()
        {
            var min = SeriesUtils.GetFloatMaxArray(VectorSize);
            var max = SeriesUtils.GetFloatMinArray(VectorSize);

            for (var i = 0; i < DataSize; i += VectorSize)
            for (var j = 0; j < VectorSize; j++)
            {
                if (_floatValues[i + j] < min[j])
                {
                    min[j] = _floatValues[i + j];
                }

                if (_floatValues[i + j] > max[j])
                {
                    max[j] = _floatValues[i + j];
                }
            }

            CachedFrame = new FloatSeries(VectorSize, SeriesUtils.CombineFloatArrays(min, max));
            SeriesUtils.SubtractFloatArrayFrom(max, min);
            CachedSize = new FloatSeries(VectorSize, max);
        }

        public override void InterpolateInto(Series b, float t)
        {
            for (var i = 0; i < DataSize; i++)
                if (i < b.DataSize)
                {
                    _floatValues[i] += (b[i] - _floatValues[i]) * t;
                }
                else
                {
                    break;
                }
        }

        public override void CombineInto(Series b, CombineFunction combineFunction)
        {
            switch (combineFunction)
            {
                case CombineFunction.Add:
                    for (var i = 0; i < DataSize; i++) _floatValues[i] += b[i];
                    break;
                case CombineFunction.Subtract:
                    for (var i = 0; i < DataSize; i++) _floatValues[i] -= b[i];
                    break;
                case CombineFunction.Multiply:
                    for (var i = 0; i < DataSize; i++) _floatValues[i] *= b[i];
                    break;
                case CombineFunction.Divide:
                    for (var i = 0; i < DataSize; i++)
                    {
                        var div = b[i];
                        _floatValues[i] = div != 0 ? _floatValues[i] / div : _floatValues[i];
                    }

                    break;
                case CombineFunction.Average:
                    for (var i = 0; i < DataSize; i++) _floatValues[i] = (_floatValues[i] + b[i]) / 2.0f;
                    break;
                case CombineFunction.Replace:
                    for (var i = 0; i < DataSize; i++) _floatValues[i] = b[i];
                    break;
            }
        }

        public override float[] FloatData => (float[]) _floatValues.Clone();
        public override int[] IntData => _floatValues.ToInt();
        public override bool[] BoolData => throw new NotImplementedException();

        //public new float this[int index] => _floatValues[index]; // uncomment for direct special case access to float value
        public override float FloatDataAt(int index)
        {
            return index >= 0 && index < _floatValues.Length ? _floatValues[index] : 0;
        }

        public override int IntDataAt(int index)
        {
            return index >= 0 && index < _floatValues.Length ? (int) _floatValues[index] : 0;
        }

        public override bool BoolDataAt(int index)
        {
            return index >= 0 && index < _floatValues.Length && Math.Abs(_floatValues[index]) < 0.0000001;
        }

        public override Series GetZeroSeries()
        {
            return new FloatSeries(VectorSize, SeriesUtils.GetFloatZeroArray(VectorSize));
        }

        public override Series GetZeroSeries(int elementCount)
        {
            return SeriesUtils.GetZeroFloatSeries(VectorSize, elementCount);
        }

        public override Series GetMinSeries()
        {
            return new FloatSeries(VectorSize, SeriesUtils.GetFloatMinArray(VectorSize));
        }

        public override Series GetMaxSeries()
        {
            return new FloatSeries(VectorSize, SeriesUtils.GetFloatMaxArray(VectorSize));
        }
    }
}