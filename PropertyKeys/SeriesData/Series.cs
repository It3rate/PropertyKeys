using System;
using System.Collections;
using DataArcs.Samplers;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	public enum SeriesType
	{
		Int,
		Float,
		Parametric,
		Bool,
	}

    public abstract class Series : IEnumerable
    {
        public int VectorSize { get; set; }
        public abstract int Count { get; }
        public SeriesType Type { get; }

        /// <summary>
        /// The raw size of the stored data array, ignores Capacity and VectorSize.
        /// </summary>
        public abstract int DataSize { get; }

        /// <summary>
        /// Cached frame in four vectorSize data points, x, y, x + width, y + height.
        /// </summary>
        public Series Frame
        {
            get
            {
                if (CachedFrame == null)
                {
                    CalculateFrame();
                }

                return CachedFrame;
            }
        }

        /// <summary>
        /// Cached size in two vectorSize data points, width and height of current data.
        /// </summary>
        public Series Size
        {
            get
            {
                if (CachedSize == null)
                {
                    CalculateFrame();
                }

                return CachedSize;
            }
        }

        protected Series CachedFrame;
        protected Series CachedSize;

        protected Series(int vectorSize, SeriesType type)
        {
            Type = type;
            VectorSize = vectorSize;
        }

        public virtual void ResetData()
        {
        }

        public virtual void Update(float time)
        {
        }

        protected abstract void CalculateFrame();

        public abstract Series GetSeriesAtIndex(int index);
        public abstract void SetSeriesAtIndex(int index, Series series);

        /// <summary>
        /// Gets data with an assumed count. Series do not know about capacities, so needs to be passed.
        /// </summary>
        public virtual Series GetValueAtVirtualIndex(int index, int capacity)
        {
            var indexT = index / (capacity - 1f);
            return GetValueAtT(indexT);
        }

        // todo: All float t's should probably be float[] t.
        public virtual Series GetValueAtT(float t)
        {
            Series result;
            if (t >= 1)
            {
                result = GetSeriesAtIndex(Count - 1);
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
                    result = GetSeriesAtIndex(startIndex);
                    var end = GetSeriesAtIndex(startIndex + 1);
                    result.InterpolateInto(end, remainderT);
                }
                else
                {
                    result = GetSeriesAtIndex(startIndex);
                }
            }
            else
            {
                result = GetSeriesAtIndex(0);
            }

            return result;
        }

        public abstract float FloatDataAt(int index);
        public abstract int IntDataAt(int index);
        public abstract bool BoolDataAt(int index);

        public abstract float[] FloatData { get; }
        public abstract int[] IntData { get; }
        public abstract bool[] BoolData { get; }

        public abstract void CombineInto(Series b, CombineFunction combineFunction);
        public abstract void InterpolateInto(Series b, float t);

        public abstract Series GetZeroSeries();
        public abstract Series GetZeroSeries(int elements);
        public abstract Series GetMinSeries();
        public abstract Series GetMaxSeries();

        public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
        public Store Store => new Store(this, new LineSampler(this.Count));

        public abstract Series Copy();

        public float X => DataSize > 0 ? FloatDataAt(0) : 0;
        public float Y => DataSize > 1 ? FloatDataAt(1) : 0;
        public float Z => DataSize > 2 ? FloatDataAt(2) : 0;
        public float W => DataSize > 3 ? FloatDataAt(3) : 0;

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
            public object Current => _instance.GetSeriesAtIndex(_position);

            public void Reset()
            {
                _position = 0;
            }
        }
#endregion

    }
}