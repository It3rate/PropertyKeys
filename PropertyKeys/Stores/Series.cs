
namespace DataArcs.Stores
{
    public enum SeriesType
    {
        Int,
        Float,
        Bool,
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

        public abstract Series GetValueAtIndex(int index); // could be array indexer?
        public abstract Series GetValueAtT(float t);

        protected abstract void CalculateFrame();

        public abstract float FloatValueAt(int index);
        public abstract int IntValueAt(int index);
        public abstract bool BoolValueAt(int index);
        public abstract float[] FloatValuesCopy { get; }
        public abstract int[] IntValuesCopy { get; }
        public abstract bool[] BoolValuesCopy { get; }

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
