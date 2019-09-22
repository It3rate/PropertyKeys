using System;
using System.Collections;
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
        /// The raw size of the stored data array, ignores VirtualCount and VectorSize.
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

        public abstract Series GetDataAtIndex(int index);
        public abstract void SetDataAtIndex(int index, Series series);

        public virtual Series GetValueAtVirtualIndex(int index, int virtualCount)
        {
            var indexT = index / (virtualCount - 1f);
            return GetValueAtT(indexT);
        }
        //public virtual Series GetValueAtVirtualIndex(int index, int vectorSize)
        //{
        //    var indexT = index / (VirtualCount - 1f);
        //    Series result = GetValueAtT(indexT);
        //    if (result.VectorSize != vectorSize)
        //    {
        //        result.VectorSize = 1;
        //        result.VirtualCount = vectorSize;
        //        result.HardenToData();
        //        result.VectorSize = vectorSize;
        //    }
        //    return result;
        //}

        // todo: All float t's should probably be float[] t.
        public virtual Series GetValueAtT(float t)
        {
            Series result;
            if (t >= 1)
            {
                result = GetDataAtIndex(Count - 1);
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
                    result = GetDataAtIndex(startIndex);
                    var end = GetDataAtIndex(startIndex + 1);
                    result.InterpolateInto(end, remainderT);
                }
                else
                {
                    result = GetDataAtIndex(startIndex);
                }
            }
            else
            {
                result = GetDataAtIndex(0);
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

        public Store Store => new Store(this);

        public abstract Series Copy();

#region Enumeration
        //public Series this[int index] => GetValueAtVirtualIndex(index);
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
            public object Current => _instance.GetDataAtIndex(_position);

            public void Reset()
            {
                _position = 0;
            }
        }
#endregion

    }
}