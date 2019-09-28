using System;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class FloatSeries : Series
	{
		protected float[] _floatValues;
        public override int Count => (int)(_floatValues.Length / VectorSize);
        public override int DataSize => _floatValues.Length;
        
		public FloatSeries(int vectorSize, params float[] values) : base(vectorSize, SeriesType.Float)
		{
			// insure at least vectorSize elements in values array.
			if (values.Length < vectorSize)
			{
				values = new float[vectorSize];
				for (int i = 0; i < vectorSize; i++)
				{
					values[i] = i < values.Length ? values[i] : values[values.Length - 1];
				}
			}
			_floatValues = values;
		}

		public override Series GetSeriesAtIndex(int index)
		{
			var startIndex = Math.Min(Count - 1, Math.Max(0, index));
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

		protected override void CalculateFrame()
		{
			var min = SeriesUtils.GetFloatMaxArray(VectorSize);
			var max = SeriesUtils.GetFloatMinArray(VectorSize);

			for (var i = 0; i < DataSize; i += VectorSize)
			{
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
			}

			Frame = new FloatSeries(VectorSize, SeriesUtils.CombineFloatArrays(min, max));
			SeriesUtils.SubtractFloatArrayFrom(max, min);
			Size = new FloatSeries(VectorSize, max);
		}

		public void Normalize()
        {
            float[] frameMin = Frame.GetValueAtT(0).FloatData;
            float[] frameMax = Frame.GetValueAtT(1).FloatData;
            float maxDif = int.MinValue;
            for (int i = 0; i < frameMax.Length; i++)
            {
                float dif = frameMax[i] - frameMin[i];
                maxDif = dif > maxDif ? dif : maxDif;
            }
            
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < VectorSize; j++)
                {
                    _floatValues[i * VectorSize + j] = (_floatValues[i * VectorSize + j] - frameMin[j]) / maxDif;
                }
            }
		}

        // FitInto distorts the shape to fit into the passed series frame. Ignores any overhang in VectorSize.
        public void FitInto(Series bounds)
		{
            Normalize();

            Series frame = bounds.Frame;
            float[] boundsMin = frame.GetValueAtT(0).FloatData;
			float[] boundsDif = frame.GetValueAtT(1).FloatData;
			for (int i = 0; i < boundsDif.Length; i++)
			{
				boundsDif[i] -= boundsMin[i];
			}

            int maxLen = Math.Min(boundsMin.Length, boundsDif.Length);
            for (int i = 0; i < Count; i++)
			{
				for (int j = 0; j < VectorSize; j++)
				{
					int index = j < maxLen ? j : maxLen - 1;
					_floatValues[i * VectorSize + j] = _floatValues[i * VectorSize + j] * boundsDif[index] + boundsMin[index];
				}
			}
		}

		public override void InterpolateInto(Series b, float t)
		{
			for (var i = 0; i < DataSize; i++)
			{
				if (i < b.DataSize)
				{
					_floatValues[i] += (b.FloatDataAt(i) - _floatValues[i]) * t;
				}
				else
				{
					break;
				}
			}
		}

		public override void CombineInto(Series b, CombineFunction combineFunction, float t = 0)
		{
			switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] += b.FloatDataAt(i);
					}

					break;
                case CombineFunction.Subtract:
                    for (var i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] -= b.FloatDataAt(i);
                    }
                    break;
                case CombineFunction.SubtractFrom:
                    for (var i = 0; i < DataSize; i++)
                    {
                        _floatValues[i] = b.FloatDataAt(i) - _floatValues[i];
                    }
                    break;
                case CombineFunction.Multiply:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] *= b.FloatDataAt(i);
					}

					break;
                case CombineFunction.Divide:
                    for (var i = 0; i < DataSize; i++)
                    {
                        var div = b.FloatDataAt(i);
                        _floatValues[i] = div != 0 ? _floatValues[i] / div : _floatValues[i];
                    }
                    break;
                case CombineFunction.DivideFrom:
                    for (var i = 0; i < DataSize; i++)
                    {
                        var div = b.FloatDataAt(i);
                        _floatValues[i] = _floatValues[i] != 0 ? div /_floatValues[i] : div;
                    }
                    break;
                case CombineFunction.Average:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] = (_floatValues[i] + b.FloatDataAt(i)) / 2.0f;
					}
					break;
				case CombineFunction.Replace:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] = b.FloatDataAt(i);
					}
					break;
				case CombineFunction.Interpolate:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] += (b.FloatDataAt(i) - _floatValues[i]) * t;
					}
					break;
            }
		}

		public void CombineWith(float b, CombineFunction combineFunction)
		{
			switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] += b;
					}

					break;
				case CombineFunction.Subtract:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] -= b;
					}

					break;
				case CombineFunction.Multiply:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] *= b;
					}

					break;
				case CombineFunction.Divide:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] = b != 0 ? _floatValues[i] / b : _floatValues[i];
					}

					break;
				case CombineFunction.Average:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] = (_floatValues[i] + b) / 2.0f + _floatValues[i];
					}

					break;
				case CombineFunction.Replace:
					for (var i = 0; i < DataSize; i++)
					{
						_floatValues[i] = b;
					}

					break;
			}
		}

        public override float[] FloatData => (float[]) _floatValues.Clone();
		public override int[] IntData => _floatValues.ToInt();
		public override bool[] BoolData => throw new NotImplementedException();

		//public new float this[int index] => _floatValues[index]; // uncomment for direct special case access to float value
		public override float FloatDataAt(int index)
		{
            index = Math.Max(0, Math.Min(_floatValues.Length - 1, index));
			return _floatValues[index];
		}

		public override int IntDataAt(int index)
        {
            index = Math.Max(0, Math.Min(_floatValues.Length - 1, index));
            return (int)_floatValues[index];
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

		public override Series Copy()
		{
			FloatSeries result = new FloatSeries(VectorSize, FloatData);
			return result;
		}
	}
}