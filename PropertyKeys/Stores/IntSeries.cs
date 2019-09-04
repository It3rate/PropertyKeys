﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class IntSeries : Series
    {
        private readonly int[] _intValues;
        public override int DataCount => _intValues.Length;

        public IntSeries(int vectorSize, int[] values, int virtualCount = -1, EasingType[] easingTypes = null) :
            base(vectorSize, SeriesType.Int, (virtualCount <= 0) ? values.Length / vectorSize : virtualCount, easingTypes)
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
            int len = DataCount / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            float[] result = new float[VectorSize];
            if (startIndex * VectorSize + VectorSize <= DataCount)
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

        public override void Interpolate(Series b, float t)
        {
            for (int i = 0; i < DataCount; i++)
            {
                if (i < b.DataCount)
                {
                    _intValues[i] = (int)(_intValues[i] + (b.IntValueAt(i) - _intValues[i]) * t);
                }
                else
                {
                    break;
                }
            }
        }

        public override void HardenToData()
        {
            throw new NotImplementedException();
        }

        protected override void CalculateFrame()
        {
            throw new NotImplementedException();
        }

        public override float[] FloatValuesCopy => _intValues.ToFloat();
        public override int[] IntValuesCopy => (int[])_intValues.Clone();
        public override bool[] BoolValuesCopy => throw new NotImplementedException();

        public override float FloatValueAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? (float)_intValues[index] : 0;
        }
        public override int IntValueAt(int index)
        {
            return (index >= 0 && index < _intValues.Length) ? _intValues[index] : 0;
        }
        public override bool BoolValueAt(int index)
        {
            return index >= 0 && index < _intValues.Length ? _intValues[index] != 0 : false;
        }

    }
}
