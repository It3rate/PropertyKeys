﻿using System;
using System.Collections;
using DataArcs.Samplers;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public enum SeriesType
	{
		Int,
		Float,
		Parametric,
		RectF,
		Bool,
	}

    public abstract class Series : IEnumerable
    {
        public int VectorSize { get; set; }
        public abstract int Count { get; }
        public abstract SeriesType Type { get; }

        /// <summary>
        /// The raw size of the stored data array, ignores Capacity and VectorSize.
        /// </summary>
        public abstract int DataSize { get; }

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
        public Series Size
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

        private RectFSeries _cachedFrame;
        private Series _cachedSize;

        protected Series(int vectorSize)
        {
            VectorSize = vectorSize;
        }

        public virtual void ResetData()
        {
        }

        public virtual void Update(float time)
        {
        }

        public abstract void Reverse();

        protected abstract void CalculateFrame();

        public abstract Series GetRawDataAt(int index);
        public abstract void SetRawDataAt(int index, Series series);
        /// <summary>
        /// Gets data with an assumed count. Series do not know about capacities, so needs to be passed.
        /// </summary>
        public virtual Series GetVirtualValueAt(int index, int capacity)
        {
	        var indexT = SamplerUtils.TFromIndex(capacity, index);// index / (capacity - 1f);
            return GetVirtualValueAt(indexT);
        }
        public virtual Series GetVirtualValueAt(float t)
        {
            Series result;
            if (t >= 1)
            {
                result = GetRawDataAt(Count - 1);
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
                    result = GetRawDataAt(startIndex);
                    var end = GetRawDataAt(startIndex + 1);
                    result.InterpolateInto(end, remainderT);
                }
                else
                {
                    result = GetRawDataAt(startIndex);
                }
            }
            else
            {
                result = GetRawDataAt(0);
            }

            return result;
        }

        public abstract float FloatDataAt(int index);
        public abstract int IntDataAt(int index);
        public abstract bool BoolDataAt(int index);
        public abstract float[] FloatDataRef { get; }
        public abstract int[] IntDataRef { get; }
        public abstract bool[] BoolDataRef { get; }

        public abstract void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
        public abstract void InterpolateInto(Series b, float t);
        public abstract void InterpolateInto(Series b, ParametricSeries seriesT);

        public abstract Series GetZeroSeries();
        public abstract Series GetZeroSeries(int elements);
        public abstract Series GetMinSeries();
        public abstract Series GetMaxSeries();

        public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
        public Store Store => new Store(this, new LineSampler(this.Count));
        public virtual Store BakedStore
        {
            get
            {
                var result = new Store(this, new LineSampler(this.Count));
                result.BakeData();
                return result;
            }
        }
        public Store ToStore(Sampler sampler) => new Store(this, sampler);

        public abstract Series Copy();

        public float X => FloatDataAt(0);
        public float Y => FloatDataAt(Math.Min(1, DataSize - 1));
        public float Z => FloatDataAt(Math.Min(2, DataSize - 1));
        public float W => FloatDataAt(Math.Min(3, DataSize - 1));

        public Series Sum()
        {
	        return SeriesUtils.Sum(this);
        }
        public Series Average()
        {
	        return SeriesUtils.Average(this);
        }
        public Series Max()
        {
	        return SeriesUtils.Max(this);
        }
        public Series Min()
        {
	        return SeriesUtils.Min(this);
        }

        public void MapToItemOrder(IntSeries items)
        {
            var selfCopy = Copy();
	        int indexA = items.IntDataAt(0);
	        Series first = selfCopy.GetRawDataAt(indexA);
	        for (int i = 1; i < items.Count; i++)
	        {
		        int indexB = items.IntDataAt(i);
				Series second = selfCopy.GetRawDataAt(indexB);
				SetRawDataAt(i, second);
				indexA = indexB;
	        }
	        SetRawDataAt(indexA, first);
        }

        #region Enumeration
        public IEnumerator GetEnumerator()
        {
            return new SeriesEnumerator(this);
        }

        private class SeriesEnumerator : IEnumerator
        {
            private Series _instance;
            private int _position = -1;
            public SeriesEnumerator(Series instance)
            {
                _instance = instance;
            }
            public bool MoveNext()
            {
                _position++;
                return (_position < (int)(_instance.DataSize / _instance.VectorSize));
            }
            public object Current => _instance.GetRawDataAt(_position);

            public void Reset()
            {
                _position = 0;
            }
        }
#endregion

    }
}