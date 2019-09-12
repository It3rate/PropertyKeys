
using System;

namespace DataArcs.Stores
{
    public enum SeriesType
    {
        Int,
        Float,
        Bool,
    }
    public enum CombineFunction
    {
        Replace,
        Append,
        Add,
        Subtract,
        Multiply,
        Divide,
        Average,
        Interpolate,
    }

    public abstract class Series
    {
        public int VectorSize { get; }
        public int VirtualCount { get; set; }
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

        protected Series(int vectorSize, SeriesType type, int virtualCount)
        {
            Type = type;
            VectorSize = vectorSize;
            VirtualCount = virtualCount;
        }

        public virtual void Reset() {}
        public virtual void Update(float time) { }
        protected abstract void CalculateFrame();
        public abstract Series HardenToData(Store store = null); // return new copy as eventually everything should be immutable

        public abstract Series GetSeriesAtIndex(int index);
        public abstract void SetSeriesAtIndex(int index, Series series);
        public virtual Series GetValueAtVirtualIndex(int index)
        {
            float indexT = index / (VirtualCount - 1f);
            return GetValueAtT(indexT);
        }
        // todo: All float t's should probably be float[] t.
        public virtual Series GetValueAtT(float t)
        {
            Series result;
            int len = DataSize / VectorSize;
            //if (virtualCount > -1)
            //{
            //    t *= virtualCount / (float)VirtualCount;
            //}

            if (t >= 1)
            {
                result = GetSeriesAtIndex(len - 1);
            }
            else if (len > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (float)(len - 1f);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(len - 1, Math.Max(0, startIndex));
                if (pos < len - 1)
                {
                    float remainderT = pos - startIndex;
                    result = GetSeriesAtIndex(startIndex);
                    Series end = GetSeriesAtIndex(startIndex + 1);
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

        public float this[int index] => FloatDataAt(index); // convenience indexer for float values.
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

    }
}
