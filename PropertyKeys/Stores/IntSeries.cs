﻿using System;

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
        
        public override Series GetSeriesAtIndex(int index)
        {
            int len = DataSize / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            int[] result = new int[VectorSize];
            if (startIndex * VectorSize + (VectorSize - 1) < DataSize)
            {
                Array.Copy(_intValues, startIndex * VectorSize, result, 0, VectorSize);
            }
            else
            {
                Array.Copy(_intValues, DataSize - VectorSize, result, 0, VectorSize);
            }
            return new IntSeries(VectorSize, result);
        }
        public override void SetSeriesAtIndex(int index, Series series)
        {
            int len = DataSize / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            Array.Copy(series.FloatData, 0, _intValues, startIndex * VectorSize, VectorSize);
        }

        public override Series HardenToData(Store store = null)
        {
            Series result = this;
            int len = VirtualCount * VectorSize;
            if (_intValues.Length != len)
            {
                int[] vals = new int[len];
                for (int i = 0; i < VirtualCount; i++)
                {
                    int[] val = store == null ? GetSeriesAtIndex(i).IntData : store.GetValueAtIndex(i).IntData;
                    Array.Copy(val, 0 * VectorSize, vals, i * VectorSize, VectorSize);
                }

                result = new IntSeries(VectorSize, vals, VirtualCount);
            }
            return result;
        }

        public override void InterpolateInto(Series b, float t)
        {
            for (int i = 0; i < DataSize; i++)
            {
                if (i < b.DataSize)
                {
                    _intValues[i] = (int)(_intValues[i] + (b.IntDataAt(i) - _intValues[i]) * t);
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
        public override void CombineInto(Series b, CombineFunction combineFunction)
        {
            switch (combineFunction)
            {
                case CombineFunction.Add:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] += b.IntDataAt(i);
                    }
                    break;
                case CombineFunction.Subtract:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] -= b.IntDataAt(i);
                    }
                    break;
                case CombineFunction.Multiply:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] *= b.IntDataAt(i);
                    }
                    break;
                case CombineFunction.Divide:
                    for (int i = 0; i < DataSize; i++)
                    {
                        int div = IntDataAt(i);
                        _intValues[i] = div != 0 ? _intValues[i] / div : _intValues[i];
                    }
                    break;
                case CombineFunction.Average:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] = (int)((_intValues[i] + b.IntDataAt(i)) / 2.0f);
                    }
                    break;
                case CombineFunction.Replace:
                    for (int i = 0; i < DataSize; i++)
                    {
                        _intValues[i] = b.IntDataAt(i);
                    }
                    break;
            }
        }

        public override float[] FloatData => _intValues.ToFloat();
        public override int[] IntData => (int[])_intValues.Clone();
        public override bool[] BoolData => throw new NotImplementedException();

        public override float FloatDataAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? (float)_intValues[index] : 0;
        }
        public override int IntDataAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? _intValues[index] : 0;
        }
        public override bool BoolDataAt(int index)
        {
            return index >= 0 && index < _intValues.Length && _intValues[index] != 0;
        }

        public override Series GetZeroSeries() { return new IntSeries(VectorSize, DataUtils.GetIntZeroArray(VectorSize)); }
        public override Series GetZeroSeries(int elementCount) { return SeriesUtils.GetZeroIntSeries(VectorSize, elementCount); }
        public override Series GetMinSeries() { return new IntSeries(VectorSize, DataUtils.GetIntMinArray(VectorSize)); }
        public override Series GetMaxSeries() { return new IntSeries(VectorSize, DataUtils.GetIntMaxArray(VectorSize)); }
    }
}
