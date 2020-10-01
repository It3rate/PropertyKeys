using System;
using System.Linq;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public class FloatSeries : Series
	{
		public override SeriesType Type => SeriesType.Float;

        protected float[] _floatValues;
        public override int Count => (int)(_floatValues.Length / VectorSize);
        public override int DataSize => _floatValues.Length;
        
		public FloatSeries(int vectorSize, params float[] values) : base(vectorSize)
		{
			// insure at least vectorSize elements in values array.
			if (values.Length < vectorSize)
			{
				_floatValues = new float[vectorSize];
				for (int i = 0; i < vectorSize; i++)
				{
					_floatValues[i] = i < values.Length ? values[i] : values[values.Length - 1];
				}
			}
			else
			{
				_floatValues = values;
			}
			
		}

		//public float[] this[int index] => GetRawDataAt(index).FloatDataRef;

        public override Series GetRawDataAt(int index)
		{
			var startIndex = Math.Min(Count - 1, Math.Max(0, index));
			var result = new float[VectorSize];
			if (startIndex * VectorSize + VectorSize - 1 <= DataSize)
			{
				Array.Copy(_floatValues, startIndex * VectorSize, result, 0, VectorSize);
			}
			else
			{
				Array.Copy(_floatValues, DataSize - VectorSize, result, 0, VectorSize);
			}

			return new FloatSeries(VectorSize, result);
		}

		public override void SetRawDataAt(int index, Series series)
		{
			var len = DataSize / VectorSize;
			var startIndex = Math.Min(len - 1, Math.Max(0, index));
			Array.Copy(series.FloatDataRef, 0, _floatValues, startIndex * VectorSize, series.VectorSize);
		}
		
		public override void Map(FloatEquation floatEquation)
		{
			for (int i = 0; i < _floatValues.Length; i++)
			{
				_floatValues[i] = floatEquation.Invoke(_floatValues[i]);
			}
		}

        public override void Append(Series series)
		{
			Array.Resize(ref _floatValues, _floatValues.Length + VectorSize);
			SetRawDataAt(Count - 1, series);
		}
		protected override void CalculateFrame()
		{
			var min = ArrayExtension.GetFloatMaxArray(VectorSize);
			var max = ArrayExtension.GetFloatMinArray(VectorSize);

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

			Frame = new RectFSeries(ArrayExtension.CombineFloatArrays(min, max));
			ArrayExtension.SubtractFloatArrayFrom(max, min);
			Size = new FloatSeries(VectorSize, max);
		}

		// todo: should reverse based on slots, e.g. a grid sample may have more than xy in vectorSize
		public override void ReverseEachElement()
		{
			for (int i = 0; i < Count; i++)
			{
				var org = GetRawDataAt(i).FloatDataRef;
				Array.Reverse(org);
			}
        }

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
        public void FitInto(Series bounds)
		{
            Normalize();

            Series frame = bounds.Frame;
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
        public override void InterpolateInto(Series b, ParametricSeries seriesT)
        {
	        for (var i = 0; i < DataSize; i++)
	        {
		        if (i < b.DataSize)
		        {
			        var t = i < seriesT.DataSize ? seriesT[i] : seriesT[seriesT.DataSize - 1];
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
	        int minSize = Math.Min(DataSize, b.DataSize);
			switch (combineFunction)
			{
				case CombineFunction.Add:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] += b.FloatDataAt(i);
					}

					break;
                case CombineFunction.Subtract:
                    for (var i = 0; i < minSize; i++)
                    {
                        _floatValues[i] -= b.FloatDataAt(i);
                    }
                    break;
                case CombineFunction.SubtractFrom:
                    for (var i = 0; i < minSize; i++)
                    {
                        _floatValues[i] = b.FloatDataAt(i) - _floatValues[i];
                    }
                    break;
                case CombineFunction.Multiply:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] *= b.FloatDataAt(i);
					}

					break;
                case CombineFunction.Divide:
                    for (var i = 0; i < minSize; i++)
                    {
                        var div = b.FloatDataAt(i);
                        _floatValues[i] = div != 0 ? _floatValues[i] / div : _floatValues[i];
                    }
                    break;
                case CombineFunction.DivideFrom:
                    for (var i = 0; i < minSize; i++)
                    {
                        var div = b.FloatDataAt(i);
                        _floatValues[i] = _floatValues[i] != 0 ? div /_floatValues[i] : div;
                    }
                    break;
                case CombineFunction.Average:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] = (_floatValues[i] + b.FloatDataAt(i)) / 2.0f;
					}
					break;
				case CombineFunction.Replace:
				case CombineFunction.Final:
                    for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] = b.FloatDataAt(i);
					}
					break;
				case CombineFunction.Interpolate:
					for (var i = 0; i < minSize; i++)
					{
						_floatValues[i] += (b.FloatDataAt(i) - _floatValues[i]) * t;
					}
					break;
            }
		}

        public override float[] FloatDataRef => _floatValues;
		public override int[] IntDataRef => _floatValues.ToInt(); //_floatValues.ToInt();

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
		
		public override Series Copy()
		{
			FloatSeries result = new FloatSeries(VectorSize, (float[])FloatDataRef.Clone());
			return result;
		}


		public static FloatSeries NormSeries { get; } = new FloatSeries(1, 0f, 1f);
	}
}