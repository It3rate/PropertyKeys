using System;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class IntSeries : Series
	{
		public override SeriesType Type => SeriesType.Int;

        private int[] _intValues;
        public override int Count => (int)(_intValues.Length / VectorSize);
        public override int DataSize => _intValues.Length;

        public IntSeries(int vectorSize, params int[] values) : base(vectorSize)
		{
			// insure at least vectorSize elements in values array.
			if (values.Length < vectorSize)
			{
				_intValues = new int[vectorSize];
				for (int i = 0; i < vectorSize; i++)
				{
					_intValues[i] = i < values.Length ? values[i] : values[values.Length - 1];
				}
			}
			else
			{
				_intValues = values;
			}
		}

		//public int[] this[int index] => GetRawDataAt(index).IntDataRef;

        public override Series GetRawDataAt(int index)
		{
			var startIndex = Math.Min(Count - 1, Math.Max(0, index));
			var result = new int[VectorSize];
			if (startIndex * VectorSize + VectorSize - 1 <= DataSize)
			{
				Array.Copy(_intValues, startIndex * VectorSize, result, 0, VectorSize);
			}
			else
			{
				Array.Copy(_intValues, DataSize - VectorSize, result, 0, VectorSize);
			}

			return new IntSeries(VectorSize, result);
		}

		public override void SetRawDataAt(int index, Series series)
        {
            var startIndex = Math.Min(Count - 1, Math.Max(0, index));
            Array.Copy(series.IntDataRef, 0, _intValues, startIndex * VectorSize, VectorSize);
        }

        public override void Append(Series series)
		{
			Array.Resize(ref _intValues, _intValues.Length + VectorSize);
			SetRawDataAt(Count - 1, series);
		}

        public override void ReverseEachElement()
		{
			for (int i = 0; i < Count; i++)
			{
				var org = GetRawDataAt(i).IntDataRef;
				Array.Reverse(org);
				SetRawDataAt(i, new IntSeries(VectorSize, org));
			}
		}

        public override void InterpolateInto(Series b, float t)
		{
			for (var i = 0; i < DataSize; i++)
			{
				if (i < b.DataSize)
                {
                    _intValues[i] += (int)Math.Round((b.IntDataAt(i) - _intValues[i]) * t);
				}
				else
				{
					break;
				}
			}
        }
        public override void InterpolateInto(Series b, ParametricSeries seriesT)
        {
	        for (var i = 0; i < DataSize; i++)
	        {
		        if (i < b.DataSize)
		        {
			        var t = i < seriesT.DataSize ? seriesT[i] : seriesT[seriesT.DataSize - 1];
			        _intValues[i] += (int)Math.Round((b.IntDataAt(i) - _intValues[i]) * t);
                }
		        else
		        {
			        break;
		        }
	        }
        }

        protected override void CalculateFrame()
		{
			var min = ArrayExtension.GetIntMinArray(VectorSize);
			var max = ArrayExtension.GetIntMaxArray(VectorSize);

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

			Frame = new RectFSeries(ArrayExtension.CombineIntArrays(min, max).ToFloat());
			ArrayExtension.SubtractIntArrayFrom(max, min);
			Size = new IntSeries(VectorSize, max);
		}

		public override void CombineInto(Series b, CombineFunction combineFunction, float t = 0)
		{
			int minSize = Math.Min(DataSize, b.DataSize);
            switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] += b.IntDataAt(i);
					}

					break;
				case CombineFunction.Subtract:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] -= b.IntDataAt(i);
					}

					break;
				case CombineFunction.Multiply:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] *= b.IntDataAt(i);
					}

					break;
				case CombineFunction.Divide:
					for (var i = 0; i < minSize; i++)
					{
						var div = IntDataAt(i);
						_intValues[i] = div != 0 ? _intValues[i] / div : _intValues[i];
					}

					break;
				case CombineFunction.Average:
					for (var i = 0; i < minSize; i++)
					{
						_intValues[i] = (int) ((_intValues[i] + b.IntDataAt(i)) / 2.0f);
					}

					break;
				case CombineFunction.Replace:
				case CombineFunction.Final:
                    for (var i = 0; i < minSize; i++)
					{
						_intValues[i] = b.IntDataAt(i);
					}

					break;
			}
		}

		public override float[] FloatDataRef => _intValues.ToFloat(); //_intValues.ToFloat();
		public override int[] IntDataRef => (int[]) _intValues;

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

		public override Series Copy()
		{
			IntSeries result = new IntSeries(VectorSize, (int[])_intValues.Clone());
			return result;
		}

        private static readonly IntSeries _empty = new IntSeries(1,0);
        public static IntSeries Empty => _empty;
    }
}