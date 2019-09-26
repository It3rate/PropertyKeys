using System;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class IntSeries : Series
	{
		private readonly int[] _intValues;
        public override int Count => (int)(_intValues.Length / VectorSize);
        public override int DataSize => _intValues.Length;
        
		public IntSeries(int vectorSize, params int[] values) : base(vectorSize, SeriesType.Int)
		{
			_intValues = values;
		}

		public override Series GetSeriesAtIndex(int index)
		{
			var startIndex = Math.Min(Count - 1, Math.Max(0, index));
			var result = new int[VectorSize];
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
			var startIndex = Math.Min(Count - 1, Math.Max(0, index));
			Array.Copy(series.IntData, 0, _intValues, startIndex * VectorSize, VectorSize);
		}

		public override void InterpolateInto(Series b, float t)
		{
			for (var i = 0; i < DataSize; i++)
			{
				if (i < b.DataSize)
				{
					_intValues[i] = (int) Math.Round(_intValues[i] + (b.IntDataAt(i) - _intValues[i]) * t);
				}
				else
				{
					break;
				}
			}
		}

		protected override void CalculateFrame()
		{
			var min = SeriesUtils.GetIntMinArray(VectorSize);
			var max = SeriesUtils.GetIntMaxArray(VectorSize);

			for (var i = 0; i < DataSize; i += VectorSize)
			{
				for (var j = 0; j < VectorSize; j++)
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

			CachedFrame = new IntSeries(VectorSize, SeriesUtils.CombineIntArrays(min, max));
			SeriesUtils.SubtractIntArrayFrom(max, min);
			CachedSize = new IntSeries(VectorSize, max);
		}

		public override void CombineInto(Series b, CombineFunction combineFunction)
		{
			switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < DataSize; i++)
					{
						_intValues[i] += b.IntDataAt(i);
					}

					break;
				case CombineFunction.Subtract:
					for (var i = 0; i < DataSize; i++)
					{
						_intValues[i] -= b.IntDataAt(i);
					}

					break;
				case CombineFunction.Multiply:
					for (var i = 0; i < DataSize; i++)
					{
						_intValues[i] *= b.IntDataAt(i);
					}

					break;
				case CombineFunction.Divide:
					for (var i = 0; i < DataSize; i++)
					{
						var div = IntDataAt(i);
						_intValues[i] = div != 0 ? _intValues[i] / div : _intValues[i];
					}

					break;
				case CombineFunction.Average:
					for (var i = 0; i < DataSize; i++)
					{
						_intValues[i] = (int) ((_intValues[i] + b.IntDataAt(i)) / 2.0f);
					}

					break;
				case CombineFunction.Replace:
					for (var i = 0; i < DataSize; i++)
					{
						_intValues[i] = b.IntDataAt(i);
					}

					break;
			}
		}

		public override float[] FloatData => _intValues.ToFloat();
		public override int[] IntData => (int[]) _intValues.Clone();
		public override bool[] BoolData => throw new NotImplementedException();

		public override float FloatDataAt(int index)
        {
            index = Math.Max(0, Math.Min(_intValues.Length - 1, index));
            return (float)_intValues[index];
		}

		public override int IntDataAt(int index)
        {
            index = Math.Max(0, Math.Min(_intValues.Length - 1, index));
            return _intValues[index];
		}

		public override bool BoolDataAt(int index)
		{
			return index >= 0 && index < _intValues.Length && _intValues[index] != 0;
		}

		public override Series GetZeroSeries()
		{
			return new IntSeries(VectorSize, SeriesUtils.GetIntZeroArray(VectorSize));
		}

		public override Series GetZeroSeries(int elementCount)
		{
			return SeriesUtils.GetZeroIntSeries(VectorSize, elementCount);
		}

		public override Series GetMinSeries()
		{
			return new IntSeries(VectorSize, SeriesUtils.GetIntMinArray(VectorSize));
		}

		public override Series GetMaxSeries()
		{
			return new IntSeries(VectorSize, SeriesUtils.GetIntMaxArray(VectorSize));
		}

		public override Series Copy()
		{
			IntSeries result = new IntSeries(VectorSize, IntData);
			result.CachedFrame = CachedFrame;
			result.CachedSize = CachedSize;
			return result;
		}
    }
}