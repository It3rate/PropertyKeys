using System;

namespace DataArcs.Stores
{
    public class FloatSeries : Series
    {
        protected readonly float[] _floatValues;
        public override int DataSize => _floatValues.Length;

        public FloatSeries(int vectorSize, float[] values, int virtualCount = -1) :
            base(vectorSize, SeriesType.Float, (virtualCount <= 0) ? values.Length / vectorSize : virtualCount)
        {
            _floatValues = values;
        }
        public FloatSeries(int vectorSize, params float[] values) :
            base(vectorSize, SeriesType.Float, values.Length / vectorSize)
        {
            _floatValues = values;
        }

        public override Series GetDataAtIndex(int index)
        {
            int len = DataSize / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            float[] result = new float[VectorSize];
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
        public override void SetDataAtIndex(int index, Series series)
        {
            int len = DataSize / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            Array.Copy(series.Floats, 0, _floatValues, startIndex * VectorSize, VectorSize);
        }

        public override Series HardenToData(Store store = null)
        {
            Series result = this;
            int len = VirtualCount * VectorSize;
            if (_floatValues.Length != len)
            {
                float[] vals = new float[len];
                for (int i = 0; i < VirtualCount; i++)
                {
                    float[] val = store == null ? GetDataAtIndex(i).Floats : store.GetValueAtIndex(i).Floats;
                    Array.Copy(val, 0 * VectorSize, vals, i * VectorSize, VectorSize);
                }

                result = new FloatSeries(VectorSize, vals, VirtualCount);
            }
            return result;
        }

        protected override void CalculateFrame()
        {
            float[] min = DataUtils.GetFloatMaxArray(VectorSize);
            float[] max = DataUtils.GetFloatMinArray(VectorSize);

            for (int i = 0; i < DataSize; i += VectorSize)
            {
                for (int j = 0; j < VectorSize; j++)
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
            }
            CachedFrame = new FloatSeries(VectorSize, DataUtils.CombineFloatArrays(min, max));
            DataUtils.SubtractFloatArrayFrom(max, min);
            CachedSize = new FloatSeries(VectorSize, max);
        }

        public override void Interpolate(Series b, float t)
        {
            for (int i = 0; i < DataSize; i++)
            {
                if (i < b.DataSize)
                {
                    _floatValues[i] += (b[i] - _floatValues[i]) * t;
                }
                else
                {
                    break;
                }
            }
        }

        public override void Combine(Series b, CombineFunction combineFunction)
        {
            switch (combineFunction)
            {
                case CombineFunction.Add:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] += b[i];
                    }
                    break;
                case CombineFunction.Subtract:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] -= b[i];
                    }
                    break;
                case CombineFunction.Multiply:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] *= b[i];
                    }
                    break;
                case CombineFunction.Divide:
                    for (int i = 0; i < DataSize; i++)
                    {
                        float div = b[i];
                        _floatValues[i] = div != 0 ? _floatValues[i] / div : _floatValues[i];
                    }
                    break;
                case CombineFunction.Average:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] = (_floatValues[i] + b[i]) / 2.0f;
                    }
                    break;
                case CombineFunction.Replace:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] = b[i];
                    }
                    break;
            }
        }

        public override float[] Floats => (float[])_floatValues.Clone();
        public override int[] Ints => _floatValues.ToInt();
        public override bool[] Bools => throw new NotImplementedException();

        public override float FloatAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) ? _floatValues[index] : 0;
        }
        public override int IntAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) ? (int)_floatValues[index] : 0;
        }
        public override bool BoolAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) && Math.Abs(_floatValues[index]) < 0.0000001;
        }

        public override Series GetZeroSeries() { return new FloatSeries(VectorSize, DataUtils.GetFloatZeroArray(VectorSize)); }
        public override Series GetZeroSeries(int elementCount) { return GetZeroSeries(VectorSize, elementCount); }
        public override Series GetMinSeries() { return new FloatSeries(VectorSize, DataUtils.GetFloatMinArray(VectorSize)); }
        public override Series GetMaxSeries() { return new FloatSeries(VectorSize, DataUtils.GetFloatMaxArray(VectorSize)); }
        
        public static FloatSeries GetZeroSeries(int vectorSize, int elementCount) { return new FloatSeries(vectorSize, DataUtils.GetFloatZeroArray(vectorSize * elementCount)); }
    }
}
