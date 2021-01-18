using System;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public class IntSeries : Series
	{
		public override SeriesType Type => SeriesType.Int;

        //public override int Count => (int)(_intValues.Length / VectorSize);
        //public override int DataSize => _intValues.Length;

        public IntSeries(int vectorSize, params int[] values) : base(vectorSize, values)
		{
		}

		//public int[] this[int index] => GetSeriesAt(index).IntDataRef;

		public override void Map(FloatEquation floatEquation)
		{
			for (int i = 0; i < _intValues.Length; i++)
			{
				_intValues[i] = (int)floatEquation.Invoke(_intValues[i]);
			}
		}
        //      public override void Append(Series series)
        //{
        //	Array.Resize(ref _intValues, _intValues.Length + VectorSize);
        //	SetSeriesAt(Count - 1, series);
        //}
        //      protected override void CalculateFrame()
        //      {
        //       var min = ArrayExtension.GetIntMinArray(VectorSize);
        //       var max = ArrayExtension.GetIntMaxArray(VectorSize);

        //       for (var i = 0; i < DataSize; i += VectorSize)
        //       {
        //        for (var j = 0; j < VectorSize; j++)
        //        {
        //	        if (_intValues[i + j] < min[j])
        //	        {
        //		        min[j] = _intValues[i + j];
        //	        }

        //	        if (_intValues[i + j] > max[j])
        //	        {
        //		        max[j] = _intValues[i + j];
        //	        }
        //        }
        //       }

        //       Frame = new RectFSeries(ArrayExtension.CombineIntArrays(min, max).ToFloat());
        //       ArrayExtension.SubtractIntArrayFrom(max, min);
        //       Size = new IntSeries(VectorSize, max);
        //      }

        //      public override void ReverseEachElement()
        //{
        //	for (int i = 0; i < Count; i++)
        //	{
        //		var org = GetSeriesAt(i).IntDataRef;
        //		Array.Reverse(org);
        //		SetSeriesAt(i, new IntSeries(VectorSize, org));
        //	}
        //}

        //      public override void InterpolateInto(Series b, float t)
        //{
        //	for (var i = 0; i < DataSize; i++)
        //	{
        //		if (i < b.DataSize)
        //              {
        //                  _intValues[i] += (int)Math.Round((b.IntValueAt(i) - _intValues[i]) * t);
        //		}
        //		else
        //		{
        //			break;
        //		}
        //	}
        //      }
        //      public override void InterpolateInto(Series b, ParametricSeries seriesT)
        //{
        // for (var i = 0; i < DataSize; i++)
        // {
        //  if (i < b.DataSize)
        //  {
        //   var t = i < seriesT.DataSize ? seriesT[i] : seriesT[seriesT.DataSize - 1];
        //   _intValues[i] += (int)Math.Round((b.IntValueAt(i) - _intValues[i]) * t);
        //        }
        //  else
        //  {
        //   break;
        //  }
        // }
        //}

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

		public override Series GetSeriesAt(int index)
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

        public override float FloatValueAt(int index)
        {
            index = IndexClampMode.GetClampedValue(index, _intValues.Length);// Math.Max(0, Math.Min(_intValues.Length - 1, index));
            return (float)_intValues[index];
		}

		public override int IntValueAt(int index)
        {
            index = IndexClampMode.GetClampedValue(index, _intValues.Length);// Math.Max(0, Math.Min(_intValues.Length - 1, index));
            return _intValues[index];
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