using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Components.Libraries;

namespace Motive.SeriesData
{
	//public interface ISeriesBaseX
	//{
	//	int Count { get; }
	//	SeriesType Type { get; }
	//	int VectorSize { get; set; }
	//	DiscreteClampMode IndexClampMode { get; set; }

	//	float FloatValueAt(int index);
	//	IStore Store(Sampler sampler = null);
	//	ISeriesBaseX Copy();

 //       void ResetData();
 //       void ReverseEachElement();
 //       void Append(Series series);
 //       void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
 //       void InterpolateInto(Series b, float t);
 //       void InterpolateInto(Series b, ParametricSeries seriesT);
 //   }
    public abstract class Series : ISeries
    {
	    protected readonly IList _valuesRef;

	    protected float[] _floatValues;
	    protected int[] _intValues;

        protected Series(int vectorSize, float[] values)
        {
	        // insure at least vectorSize elements in values array.
	        // This can be eliminated if moving to a clamping system for series (shouldn't be needed anyway, but currently copy etc relies on it).
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

            VectorSize = vectorSize;
	        _valuesRef = _floatValues;
        }
        protected Series(int vectorSize, int[] values)
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

            VectorSize = vectorSize;
	        _valuesRef = _intValues;
        }

        public ISeries this[int i]
        {
	        get => GetSeriesAt(i);
	        set => SetSeriesAt(i, value);
        }

        private ISeries _cachedSize;
        private RectFSeries _cachedFrame;

        public string Name { get; set; }
        public int Id { get; private set; }

        public int VectorSize { get; set; }
        public DiscreteClampMode IndexClampMode { get; set; } = DiscreteClampMode.Clamp;
        public int DataSize => _valuesRef.Count;
        public virtual int Count => (int)(_valuesRef.Count / VectorSize);
        public abstract SeriesType Type { get; }


        /// <summary>
        /// The raw size of the stored data array, ignores SampleCount and VectorSize.
        /// </summary>

        protected int StartIndex = 0;
        protected int EndIndex = 0;
        public float X => FloatValueAt(StartIndex);
        public float Y => FloatValueAt(StartIndex + 1);
        public float Z => FloatValueAt(StartIndex + 2);
        public float W => FloatValueAt(StartIndex + 3);

        /// <summary>
        /// Cached frame in four vectorSize data points, x, y, x + width, y + height.
        /// </summary>
        public RectFSeries Frame
        {
            get
            {
                if (_cachedFrame == null)
                {
                    CalculateFrame();
                }

                return _cachedFrame;
            }
            protected set => _cachedFrame = value;
        }

        /// <summary>
        /// Cached size in two vectorSize data points, width and height of current data.
        /// </summary>
        public ISeries Size
        {
            get
            {
                if (_cachedSize == null)
                {
                    CalculateFrame();
                }

                return _cachedSize;
            }
            protected set => _cachedSize = value;
        }

        public void Append(Series series)
        {
	        for (int i = 0; i < series.DataSize; i++)
	        {
		        _valuesRef.Add(series._valuesRef[i]);
            }
        }

        private float[] GetFloatArray()
        {
            float[] result;
            if (_floatValues != null)
            {
	            result = _floatValues;
            }
            else
            {
	            result = _intValues.ToFloat();
            }

            return result;
        }
        protected void CalculateFrame()
        {
            // TODO: move this down, or generally find a better way to do this.
	        var min = GetSeriesAt(0).GetFloatArray();
	        var max = GetSeriesAt(0).GetFloatArray();

	        for (var i = 0; i < DataSize; i += VectorSize)
	        {
		        for (var j = 0; j < VectorSize; j++)
		        {
			        if ( ((IComparable)_valuesRef[i + j]).CompareTo(min[j]) < 0)
			        {
				        min[j] = (float)_valuesRef[i + j];
			        }

			        if (((IComparable)_valuesRef[i + j]).CompareTo(max[j]) > 0)
                    {
				        max[j] = (float)_valuesRef[i + j];
			        }
		        }
	        }
	        Frame = new RectFSeries(ArrayExtension.CombineFloatArrays(min, max));
            ArrayExtension.SubtractFloatArrayFrom(max, min);
            Size = new FloatSeries(VectorSize, max); ;
        }

        public virtual void ReverseEachElement()
        {
	        if (VectorSize > 1)
	        {
		        var len = VectorSize - 1;
		        for (int i = 0; i < Count; i++)
		        {
			        var start = i * VectorSize;
			        for (int j = 0; j < (int)VectorSize / 2; j++)
			        {
				        var temp = _valuesRef[start + j];
				        _valuesRef[start + j] = _valuesRef[start + len - j];
				        _valuesRef[start + len - j] = temp;
			        }
		        }
	        }
        }

        public abstract void InterpolateValue(ISeries a, ISeries b, int index, float t);

        public void InterpolateInto(ISeries b, float t)
        {
	        for (var i = 0; i < DataSize; i++)
	        {
		        if (i < b.DataSize)
		        {
                    InterpolateValue(this, b, i, t);
			        //_valuesRef[i] = Interpolate(_valuesRef[i], b._valuesRef[i], t);
		        }
		        else
		        {
			        break;
		        }
	        }
        }

        public void InterpolateInto(ISeries b, ParametricSeries seriesT)
        {
	        for (var i = 0; i < DataSize; i++)
	        {
		        if (i < b.DataSize)
		        {
			        var t = i < seriesT.DataSize ? seriesT[i] : seriesT[seriesT.DataSize - 1];
			        InterpolateValue(this, b, i, t);
                    //_valuesRef[i] = Interpolate(_valuesRef[i], b._valuesRef[i], t);
		        }
		        else
		        {
			        break;
		        }
	        }
        }

        public abstract void Map(FloatEquation floatEquation);

        //protected abstract T Add<T>(T a, T b, float t);

        public static ISeries operator +(Series a, Series b) => ApplyFunction(a, b, CombineFunction.Add);
        public static ISeries operator -(Series a, Series b) => ApplyFunction(a, b, CombineFunction.Subtract);
        public static ISeries operator *(Series a, Series b) => ApplyFunction(a, b, CombineFunction.Multiply);
        public static ISeries operator /(Series a, Series b) => ApplyFunction(a, b, CombineFunction.Divide);
        private static ISeries ApplyFunction(ISeries a, ISeries b, CombineFunction combineFunction, float t = 0)
        {
	        var result = (Series)a.Copy();
	        result.CombineInto(b, combineFunction, t);
	        return result;
        }

        public abstract void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0);

        public abstract Series GetSeriesAt(int index);
        public abstract void SetSeriesAt(int index, ISeries series);
        public abstract float FloatValueAt(int index);
        public abstract int IntValueAt(int index);
        public abstract float[] FloatDataRef { get; }
        public abstract int[] IntDataRef { get; }
        public abstract ISeries Copy();


        public virtual ISeries GetSeriesAt(float t) => GetSeriesAt(SamplerUtils.IndexFromT(Count, t));
        public virtual Series GetVirtualValueAt(float t)
        {
	        Series result;
            if (t >= 1)
            {
                result = GetSeriesAt(Count - 1);
            }
            else if (Count > 1)
            {
                var endIndex = Count - 1;
                // interpolate between indexes to get virtual values from array.
                var pos = Math.Min(1, Math.Max(0, t)) * endIndex;
                var startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(endIndex, Math.Max(0, startIndex));
                if (pos < endIndex)
                {
                    var remainderT = pos - startIndex;
                    result = GetSeriesAt(startIndex);
                    var end = GetSeriesAt(startIndex + 1);
                    result.InterpolateInto(end, remainderT);
                }
                else
                {
                    result = GetSeriesAt(startIndex);
                }
            }
            else
            {
                result = GetSeriesAt(0);
            }

            return result;
        }





        public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
        public IStore Store(Sampler sampler = null)
        {
            sampler = sampler ?? new LineSampler(this.Count);
            return new Store(this, sampler);
        }


        public List<ISeries> ToList()
        {
            var result = new List<ISeries>(Count);
            for (int i = 0; i < Count; i++)
            {
                result.Add(GetSeriesAt(i));
            }
            return result;
        }
        public void SetByList(List<ISeries> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SetSeriesAt(i, items[i]);
            }
        }
        public void MapValuesToItemPositions(IntSeries items)
        {
            var selfCopy = Copy();
            for (int i = 0; i < items.Count; i++)
            {
                int index = items.IntValueAt(i);
                ISeries second = selfCopy.GetSeriesAt(i);
                SetSeriesAt(index, second);
            }
        }
        public void MapOrderToItemPositions(IntSeries items)
        {
            var selfCopy = Copy();
            for (int i = 0; i < items.Count; i++)
            {
                int index = items.IntValueAt(i);
                ISeries second = selfCopy.GetSeriesAt(index);
                SetSeriesAt(i, second);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new ISeriesEnumerator(this);
        }

        public bool AssignIdIfUnset(int id)
        {
            bool result = false;
            if (Id == 0 && id > 0)
            {
                Id = id;
                result = true;
            }
            return result;
        }
        public void OnActivate()
        {
        }

        public void OnDeactivate()
        {
        }

        public void ResetData()
        {
        }
        public virtual void Update(double currentTime, double deltaTime)
        {
        }
    }
}