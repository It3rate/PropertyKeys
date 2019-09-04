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

    public class Series
    {
        public int VectorSize { get; } = 1;
        public int VirtualCount { get; set; }
        public int DataCount { get; }
        public SeriesType Type { get; }

        public Series Frame
        {
            get
            {
                if (_frame == null)
                {
                    CalculateFrame();
                }

                return _frame;
            }
        }
        public Series Size
        {
            get
            {
                if (_size == null)
                {
                    CalculateFrame();
                }

                return _size;
            }
        }
        public EasingType[] EasingTypes { get; }

        // perhaps these should be separated into subclasses with conversions.
        private int[] _intValues;
        private float[] _floatValues;
        private Series _frame;
        private Series _size;

        public Series(int vectorSize, int[] values, int virtualCount = -1, EasingType[] easingTypes = null)
        {
            Type = SeriesType.Int;
            VectorSize = vectorSize;
            _intValues = values;
            VirtualCount = (virtualCount < 0) ? values.Length : virtualCount;
            EasingTypes = easingTypes;
        }
        public Series(int vectorSize, float[] values, int virtualCount = -1, EasingType[] easingTypes = null)
        {
            Type = SeriesType.Int;
            VectorSize = vectorSize;
            _floatValues = values;
            VirtualCount = (virtualCount < 0) ? values.Length : virtualCount;
            EasingTypes = easingTypes;
        }

        public Series GetValuesAtIndex(int index){ return null;} // could be array indexer?
        public Series GetValuesAtT(float t) { return null; }

        public float[] GetFloatValues { get; }
        public int[] GetIntValues { get; }
        public bool[] GetBoolValues { get; }

        public void HardenToData() {}

        private void CalculateFrame()
        {
            _frame = null;
            _size = null;
        }
        public static Series GetZeroArray(int size) { return null; }
        public static Series GetMinArray(int size) { return null; }
        public static Series GetMaxArray(int size) { return null; }
    }
}
