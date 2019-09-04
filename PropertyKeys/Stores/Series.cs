using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int VirtualCount { get; }
        public SeriesType Type { get; }
        public abstract int DataCount { get; }

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
        public EasingType[] EasingTypes { get; }
        
        protected Series CachedFrame;
        protected Series CachedSize;

        protected Series(int vectorSize, SeriesType type, int virtualCount, EasingType[] easingTypes = null)
        {
            Type = type;
            VectorSize = vectorSize;
            VirtualCount = virtualCount;
            EasingTypes = easingTypes;
        }

        public abstract Series GetValueAtIndex(int index); // could be array indexer?
        public abstract Series GetValueAtT(float t);
        public abstract void HardenToData();
        protected abstract void CalculateFrame();

        public abstract float FloatValueAt(int index);
        public abstract int IntValueAt(int index);
        public abstract bool BoolValueAt(int index);
        public abstract float[] FloatValuesCopy { get; }
        public abstract int[] IntValuesCopy { get; }
        public abstract bool[] BoolValuesCopy { get; }

        public abstract void Interpolate(Series b, float t);

        //public abstract Series GetZeroArray();
        //public abstract Series GetMinArray();
        //public abstract Series GetMaxArray();
    }
}
