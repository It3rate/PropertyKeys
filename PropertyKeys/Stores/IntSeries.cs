using System;

namespace DataArcs.Stores
{
    public class IntSeries : Series
    {
        private readonly int[] _intValues;
        public override int DataSize => _intValues.Length;

        public IntSeries(int vectorSize, int[] values, int virtualCount = -1) :
            base(vectorSize, SeriesType.Int, (virtualCount <= 0) ? values.Length / vectorSize : virtualCount)
        {
            _intValues = values;
        }
        public IntSeries(int vectorSize, params int[] values) :
            base(vectorSize, SeriesType.Int, values.Length / vectorSize)
        {
            _intValues = values;
        }

        public override Series GetValueAtIndex(int index)
        {
            int len = DataSize / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            float[] result = new float[VectorSize];
            if (startIndex * VectorSize + VectorSize <= DataSize)
            {
                Array.Copy(_intValues, index * VectorSize, result, 0, VectorSize);
            }
            return new FloatSeries(VectorSize, result);
        }

        public override Series GetValueAtT(float t)
        {
            Series result;
            int len = _intValues.Length / VectorSize;
            if (len > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (len - 1);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(len - 1, Math.Max(0, startIndex));
                if (pos < len - 1)
                {
                    float remainderT = pos - startIndex;
                    result = GetValueAtIndex(startIndex);
                    Series end = GetValueAtIndex(startIndex + 1);
                    result.Interpolate(end, remainderT);
                }
                else
                {
                    result = GetValueAtIndex(startIndex);
                }
            }
            else
            {
                result = GetValueAtIndex(0);
            }
            return result;
        }

        public override Series HardenToData(Store store)
        {
            Series result = this;
            int len = VirtualCount * VectorSize;
            if (_intValues.Length != len)
            {
                int[] vals = new int[len];
                for (int i = 0; i < VirtualCount; i++)
                {
                    int[] val = store.GetValueAtIndex(i).Ints;
                    Array.Copy(val, 0 * VectorSize, vals, i * VectorSize, VectorSize);
                }

                result = new IntSeries(VectorSize, vals, VirtualCount);
            }
            return result;
        }

        public override void Interpolate(Series b, float t)
        {
            for (int i = 0; i < DataSize; i++)
            {
                if (i < b.DataSize)
                {
                    _intValues[i] = (int)(_intValues[i] + (b.IntAt(i) - _intValues[i]) * t);
                }
                else
                {
                    break;
                }
            }
        }

        protected override void CalculateFrame()
        {
            int[] min = DataUtils.GetIntMinArray(VectorSize);
            int[] max = DataUtils.GetIntMaxArray(VectorSize);

            for (int i = 0; i < DataSize; i += VectorSize)
            {
                for (int j = 0; j < VectorSize; j++)
                {
                    if (_intValues[i + j] < min[j])
                    {
                        min[j] = _intValues[i + j];
                    }

                    if (_intValues[i + j] > max[j])
                    {
                        max[j] = _intValues[i + j];
                    }
                }
            }
            CachedFrame = new IntSeries(VectorSize, DataUtils.CombineIntArrays(min, max));
            DataUtils.SubtractIntArrayFrom(max, min);
            CachedSize = new IntSeries(VectorSize, max);
        }
        public override void Combine(Series b, CombineFunction combineFunction)
        {
            switch (combineFunction)
            {
                case CombineFunction.Add:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] += b.IntAt(i);
                    }
                    break;
                case CombineFunction.Subtract:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] -= b.IntAt(i);
                    }
                    break;
                case CombineFunction.Multiply:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] *= b.IntAt(i);
                    }
                    break;
                case CombineFunction.Divide:
                    for (int i = 0; i < DataSize; i++)
                    {
                        int div = IntAt(i);
                        _intValues[i] = div != 0 ? _intValues[i] / div : _intValues[i];
                    }
                    break;
                case CombineFunction.Average:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] = (int)((_intValues[i] + b.IntAt(i)) / 2.0f);
                    }
                    break;
                case CombineFunction.Replace:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] = b.IntAt(i);
                    }
                    break;
            }
        }

        public override float[] Floats => _intValues.ToFloat();
        public override int[] Ints => (int[])_intValues.Clone();
        public override bool[] Bools => throw new NotImplementedException();

        public override float FloatAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? (float)_intValues[index] : 0;
        }
        public override int IntAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? _intValues[index] : 0;
        }
        public override bool BoolAt(int index)
        {
            return index >= 0 && index < _intValues.Length && _intValues[index] != 0;
        }

        public override Series GetZeroSeries() { return new IntSeries(VectorSize, DataUtils.GetIntZeroArray(VectorSize)); }
        public override Series GetZeroSeries(int elementCount) { return GetZeroSeries(VectorSize, elementCount); }
        public override Series GetMinSeries() { return new IntSeries(VectorSize, DataUtils.GetIntMinArray(VectorSize)); }
        public override Series GetMaxSeries() { return new IntSeries(VectorSize, DataUtils.GetIntMaxArray(VectorSize)); }
        
        public static IntSeries GetZeroSeries(int vectorSize, int elementCount) { return new IntSeries(vectorSize, DataUtils.GetIntZeroArray(vectorSize * elementCount)); }

    }
}
