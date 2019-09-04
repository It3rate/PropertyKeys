using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class FloatSeries : Series
    {
        private readonly float[] _floatValues;
        public override int DataCount => _floatValues.Length;

        public FloatSeries(int vectorSize, float[] values, int virtualCount = -1, EasingType[] easingTypes = null) :
            base(vectorSize, SeriesType.Int, (virtualCount <= 0) ? values.Length / vectorSize : virtualCount, easingTypes)
        {
            _floatValues = values;
        }
        public FloatSeries(int vectorSize, params float[] values) :
            base(vectorSize, SeriesType.Int, values.Length / vectorSize)
        {
            _floatValues = values;
        }
        public override Series GetValueAtIndex(int index)
        {
            int len = DataCount / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            float[] result = new float[VectorSize];
            if (startIndex * VectorSize + VectorSize <= DataCount)
            {
                Array.Copy(_floatValues, index * VectorSize, result, 0, VectorSize);
            }
            return new FloatSeries(VectorSize, result);
        }

        public override Series GetValueAtT(float t)
        {
            return GetInterpolatededValueAtT(t);
        }

        public override void HardenToData()
        {
            throw new NotImplementedException();
        }

        protected override void CalculateFrame()
        {
            throw new NotImplementedException();
        }

        private Series GetInterpolatededValueAtT(float t)
        {
            Series result;
            int len = DataCount / VectorSize;
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
                    _floatValues[i] += (b.FloatValueAt(i) - _floatValues[i]) * t;
                }
                else
                {
                    break;
                }
            }
        }
        public override float[] FloatValuesCopy => (float[])_floatValues.Clone();
        public override int[] IntValuesCopy => _floatValues.ToInt();
        public override bool[] BoolValuesCopy => throw new NotImplementedException();

        public override float FloatValueAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) ? _floatValues[index] : 0;
        }
        public override int IntValueAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) ? (int)_floatValues[index] : 0;
        }
        public override bool BoolValueAt(int index)
        {
            return (index >= 0 && index < _floatValues.Length) ? (_floatValues[index] != 0) : false;
        }
    }
}
