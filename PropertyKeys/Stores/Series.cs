
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

        // todo: All float t's should probably be float[] t.
        public abstract Series GetDataAtIndex(int index);
        public virtual Series GetValueAtVirtualIndex(int index)
        {
            float indexT = index / (VirtualCount - 1f);
            return GetValueAtT(indexT);
        }
        public virtual Series GetValueAtT(float t)
        {
            Series result;
            int len = DataSize / VectorSize;

            if (t >= 1)
            {
                result = GetDataAtIndex(len - 1);
            }
            else if (len > 1)
            {
                // interpolate between indexes to get virtual values from array.
                float pos = Math.Min(1, Math.Max(0, t)) * (len - 1f);
                int startIndex = (int)Math.Floor(pos);
                startIndex = Math.Min(len - 1, Math.Max(0, startIndex));
                if (pos < len - 1)
                {
                    float remainderT = pos - startIndex;
                    result = GetDataAtIndex(startIndex);
                    Series end = GetDataAtIndex(startIndex + 1);
                    result.Interpolate(end, remainderT);
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
        public abstract Series HardenToData(Store store = null); // return new copy as eventually everything should be immutable

        public virtual void Reset() {}
        public virtual void Update() { }
        protected abstract void CalculateFrame();

        public float this[int index] => FloatAt(index); // convenience indexer for float values.
        public abstract float FloatAt(int index);
        public abstract int IntAt(int index);
        public abstract bool BoolAt(int index);

        public abstract float[] Floats { get; }
        public abstract int[] Ints { get; }
        public abstract bool[] Bools { get; }

        public abstract void Combine(Series b, CombineFunction combineFunction);
        public abstract void Interpolate(Series b, float t);

        public abstract Series GetZeroSeries();
        public abstract Series GetZeroSeries(int elements);
        public abstract Series GetMinSeries();
        public abstract Series GetMaxSeries();

        public static Series Create(Series series, int[] values)
        {
            Series result;
            if (series.Type == SeriesType.Int)
            {
                result = new IntSeries(series.VectorSize, values);
            }
            else
            {
                result = new FloatSeries(series.VectorSize, values.ToFloat());
            }
            return result;
        }
        public static Series Create(Series series, float[] values)
        {
            Series result;
            if (series.Type == SeriesType.Int)
            {
                result = new IntSeries(series.VectorSize, values.ToInt());
            }
            else
            {
                result = new FloatSeries(series.VectorSize, values);
            }
            return result;
        }
    }
}
