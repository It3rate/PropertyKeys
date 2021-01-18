using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public class FloatSeries : SeriesBase
	{
		public override SeriesType Type => SeriesType.Float;

		public FloatSeries(int vectorSize, params float[] values) : base(vectorSize, values)
		{
        }

		public override void InterpolateValue(ISeries a, ISeries b, int i, float t)
		{
			_floatValues[i] = Interpolate(_floatValues[i], b.FloatValueAt(i), t);
        }
		protected static float Interpolate(float a, float b, float t)
		{
			return (b - a) * t + a;
		}

        public override void Map(FloatEquation floatEquation)
		{
			for (int i = 0; i < _floatValues.Length; i++)
			{
				_floatValues[i] = floatEquation.Invoke(_floatValues[i]);
			}
		}

        public override void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0)
        {
	        int minSize = Math.Min(DataSize, b.DataSize);
			switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] += b.FloatValueAt(i);
					}

					break;
                case CombineFunction.Subtract:
                    for (var i = 0; i < minSize; i++)
                    {
                        _floatValues[i] -= b.FloatValueAt(i);
                    }
                    break;
                case CombineFunction.SubtractFrom:
                    for (var i = 0; i < minSize; i++)
                    {
                        _floatValues[i] = b.FloatValueAt(i) - _floatValues[i];
                    }
                    break;
                case CombineFunction.Multiply:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] *= b.FloatValueAt(i);
					}

					break;
                case CombineFunction.Divide:
                    for (var i = 0; i < minSize; i++)
                    {
                        var div = b.FloatValueAt(i);
                        _floatValues[i] = div != 0 ? _floatValues[i] / div : _floatValues[i];
                    }
                    break;
                case CombineFunction.DivideFrom:
                    for (var i = 0; i < minSize; i++)
                    {
                        var div = b.FloatValueAt(i);
                        _floatValues[i] = _floatValues[i] != 0 ? div /_floatValues[i] : div;
                    }
                    break;
                case CombineFunction.Average:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] = (_floatValues[i] + b.FloatValueAt(i)) / 2.0f;
					}
					break;
				case CombineFunction.Replace:
				case CombineFunction.Final:
                    for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] = b.FloatValueAt(i);
					}
					break;
				case CombineFunction.Interpolate:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] += (b.FloatValueAt(i) - _floatValues[i]) * t;
					}
					break;
            }
		}

        public override float[] FloatDataRef => _floatValues;
		public override int[] IntDataRef => _floatValues.ToInt(); //_floatValues.ToInt();

		public override SeriesBase GetSeriesAt(int index)
		{
			var startIndex = IndexClampMode.GetClampedValue(index, Count);//Math.Min(Count - 1, Math.Max(0, index));
			var result = new float[VectorSize];
			if (startIndex * VectorSize + VectorSize - 1 <= DataSize)
			{
				Array.Copy(_floatValues, startIndex * VectorSize, result, 0, VectorSize);
			}
			else
			{
				Array.Copy(_floatValues, DataSize - VectorSize, result, 0, VectorSize);
			}

			return new FloatSeries(VectorSize, result) { IndexClampMode = this.IndexClampMode };
		}
		public override void SetSeriesAt(int index, ISeries series)
		{
			var startIndex = IndexClampMode.GetClampedValue(index, Count);//Math.Min(len - 1, Math.Max(0, index));
			Array.Copy(series.FloatDataRef, 0, _floatValues, startIndex * VectorSize, series.VectorSize);
		}

		public override IList AppendBase(SeriesBase series)
		{
            var len = _floatValues.Length;
            float[] newArray = new float[len + series.DataSize];
            _floatValues.CopyTo(newArray, 0);
			for (int i = 0; i < series.DataSize; i++)
			{
				newArray[len + i] = series.FloatValueAt(i);
			}
			_floatValues = newArray;
			return _floatValues;
		}
//public new float this[int index] => _floatValues[index]; // uncomment for direct special case access to float value
public override float FloatValueAt(int index)
		{
            index = IndexClampMode.GetClampedValue(index, _floatValues.Length);//Math.Max(0, Math.Min(_floatValues.Length - 1, index));
            return _floatValues[index];
		}

		public override int IntValueAt(int index)
        {
            index = IndexClampMode.GetClampedValue(index, _floatValues.Length);//Math.Max(0, Math.Min(_floatValues.Length - 1, index));
            return (int)_floatValues[index];
		}
		
		public override ISeries Copy()
		{
			FloatSeries result = new FloatSeries(VectorSize, (float[])FloatDataRef.Clone()) { IndexClampMode = this.IndexClampMode };
			return result;
		}

		private static readonly FloatSeries _empty = new FloatSeries(1, 0);
		public static FloatSeries Empty => _empty;
		public static FloatSeries NormSeries { get; } = new FloatSeries(1, 0f, 1f);

		public void Normalize()
		{
			float[] frameMin = Frame.GetVirtualValueAt(0).FloatDataRef;
			float[] frameMax = Frame.GetVirtualValueAt(1).FloatDataRef;
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
		public void FitInto(ISeries bounds)
		{
			Normalize();

			ISeries frame = bounds.Frame;
			float[] boundsMin = frame.GetVirtualValueAt(0).FloatDataRef;
			float[] boundsDif = (float[])frame.GetVirtualValueAt(1).FloatDataRef.Clone();
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

		public ISeries SumSlots(params Slot[] slots)
		{
			return SeriesUtils.SumSlots(this, slots);
		}
		public ISeries MultiplySlots(params Slot[] slots)
		{
			return SeriesUtils.MultiplySlots(this, slots);
		}
		public ISeries AverageSlots(params Slot[] slots)
		{
			return SeriesUtils.AverageSlots(this, slots);
		}
		public ISeries MaxSlots(params Slot[] slots)
		{
			return SeriesUtils.MaxSlots(this, slots);
		}
		public ISeries MinSlots(params Slot[] slots)
		{
			return SeriesUtils.MinSlots(this, slots);
		}

    }
}