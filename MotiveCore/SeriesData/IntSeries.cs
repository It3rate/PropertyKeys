using System;
using System.Collections;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public class IntSeries : SeriesBase
	{
		public override SeriesType Type => SeriesType.Int;
		public IntSeries(int vectorSize, params int[] values) : base(vectorSize, values)
		{
		}
		public override void Map(FloatEquation floatEquation)
		{
			for (int i = 0; i < _intValues.Length; i++)
			{
				_intValues[i] = (int)floatEquation.Invoke(_intValues[i]);
			}
		}
        public override void InterpolateValue(ISeries a, ISeries b, int i, float t)
        {
	        _intValues[i] = Interpolate(_intValues[i], b.IntValueAt(i), t);
        }
        protected static int Interpolate(int a, int b, float t)
        {
	        return (int)Math.Round((b - a) * t) + a;
        }

        public override void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0)
		{
			int minSize = Math.Min(DataSize, b.DataSize);
            switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] += b.IntValueAt(i);
					}

					break;
				case CombineFunction.Subtract:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] -= b.IntValueAt(i);
					}

					break;
				case CombineFunction.SubtractFrom:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] = b.IntValueAt(i) - _intValues[i];
					}
					break;
                case CombineFunction.Multiply:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] *= b.IntValueAt(i);
					}

					break;
				case CombineFunction.Divide:
					for (var i = 0; i < minSize; i++)
					{
						var div = IntValueAt(i);
						_intValues[i] = div != 0 ? _intValues[i] / div : _intValues[i];
					}

					break;
				case CombineFunction.DivideFrom:
					for (var i = 0; i < minSize; i++)
					{
						var div = b.IntValueAt(i);
						_intValues[i] = _intValues[i] != 0 ? div / _intValues[i] : div;
					}
					break;
                case CombineFunction.Average:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] = (int) ((_intValues[i] + b.IntValueAt(i)) / 2.0f);
					}

					break;
				case CombineFunction.Replace:
				case CombineFunction.Final:
                    for (var i = 0; i < minSize; i++)
					{
						_intValues[i] = b.IntValueAt(i);
					}

					break;
				case CombineFunction.Interpolate:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] += (int)((b.IntValueAt(i) - _intValues[i]) * t);
					}
					break;
            }
		}

		public override float[] FloatDataRef => _intValues.ToFloat(); //_intValues.ToFloat();
		public override int[] IntDataRef => (int[]) _intValues;

		public override ISeries GetSeriesAt(int index)
		{
			var startIndex = IndexClampMode.GetClampedValue(index, Count);// Math.Min(Count - 1, Math.Max(0, index));
			var result = new int[VectorSize];
			if (startIndex * VectorSize + VectorSize - 1 <= DataSize)
			{
				Array.Copy(_intValues, startIndex * VectorSize, result, 0, VectorSize);
			}
			else
			{
				// clamp to last value if overflow
				Array.Copy(_intValues, DataSize - VectorSize, result, 0, VectorSize);
			}

			return new IntSeries(VectorSize, result) { IndexClampMode = this.IndexClampMode };
		}

		public override void SetSeriesAt(int index, ISeries series)
		{
			var startIndex = IndexClampMode.GetClampedValue(index, Count);//Math.Min(Count - 1, Math.Max(0, index));
			Array.Copy(series.IntDataRef, 0, _intValues, startIndex * VectorSize, VectorSize);
		}

		public override void Append(ISeries series)
		{
			var orgLen = _floatValues.Length;
			EnsureCount(orgLen + series.DataSize);
			for (int i = 0; i < series.DataSize; i++)
			{
				_intValues[orgLen + i] = series.IntValueAt(i);
			}
		}

		//public override void EnsureCount(int count, bool makeExact = false)
		//{
		//	int targetCount = Count < count ? count : Count;
		//	targetCount = makeExact ? count : targetCount;
		//	if (targetCount != Count)
		//	{
		//		int[] newArray = new int[targetCount * VectorSize];
		//		var len = Math.Min(_intValues.Length, newArray.Length);
		//		Array.Copy(_intValues, newArray, len);
		//		_intValues = newArray;
		//		_valuesRef = _intValues;
		//	}
		//}
		public override IList CloneArrayOfSize(int size, bool assignAsInternalArray)
		{
			int[] result = new int[size];
			var len = Math.Min(_intValues.Length, result.Length);
			Array.Copy(_intValues, result, len);
			if (assignAsInternalArray)
			{
				_intValues = result;
			}
			return result;
		}

        public override float FloatValueAt(int index)
        {
            index = IndexClampMode.GetClampedValue(index, _intValues.Length);
            return (float)_intValues[index];
        }
        public override void SetFloatValueAt(int index, float value)
        {
	        index = IndexClampMode.GetClampedValue(index, _intValues.Length);
	        _intValues[index] = (int)value;
        }

        public override int IntValueAt(int index)
        {
            index = IndexClampMode.GetClampedValue(index, _intValues.Length);
            return _intValues[index];
        }
        public override void SetIntValueAt(int index, int value)
        {
	        index = IndexClampMode.GetClampedValue(index, _intValues.Length);
	        _intValues[index] = value;
        }

        public override ISeries Copy()
		{
			return new IntSeries(VectorSize, (int[])_intValues.Clone()) { IndexClampMode = this.IndexClampMode };
		}

        private static readonly IntSeries _empty = new IntSeries(1,0);
        public static IntSeries Empty => _empty;
        public static IntSeries NormSeries { get; } = new IntSeries(1, 0, 100);
    }
}