using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public class IntStore : Store, IEnumerable<int>
    {
        public int[] Values { get; }

        public override float[] GetFloatValues => Values.ToFloat();
        public override int[] GetIntValues => (int[])Values.Clone();

        //public BaseSampler Sampler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int[] this[int index] => GetSizedValuesAt(index);

        public IntStore(int vectorSize, int[] values, int elementCount = -1, int[] dimensions = null, EasingType[] easingTypes = null) 
            : base(vectorSize, dimensions, easingTypes)
        {
            Values = values;
            ElementCount = (elementCount < 1) ? values.Length / VectorSize : elementCount;
            CalculateBounds(Values.ToFloat());
        }
        public IntStore(int vectorSize, params int[] values) : base(vectorSize, null, null)
        {
            Values = values;
            ElementCount = values.Length / VectorSize;
            CalculateBounds(Values.ToFloat());
        }

        protected override bool BoundsDataReady() => Values != null;

        public override GraphicsPath GetPath()
        {
            throw new NotImplementedException();
        }

        public override float[] GetFloatArrayAtIndex(int index)
        {
            return GetIntArrayAtIndex(index).ToFloat();
        }
        public override float[] GetFloatArrayAtT(float t)
        {
            return GetIntArrayAtT(t).ToFloat();
        }
        public override int[] GetIntArrayAtIndex(int index)
        {
            CurrentT = index / (float)ElementCount;
            int len = Values.Length / VectorSize;
            int startIndex = Math.Min(len - 1, Math.Max(0, index));
            return GetSizedValuesAt(startIndex);
        }

        public override int[] GetIntArrayAtT(float t)
        {
            CurrentT = t;
            int index = (int) (t * ElementCount);
            return GetIntArrayAtIndex(index);
        }

        protected virtual int[] GetSizedValuesAt(int index)
        {
            int[] result = GetZeroArray();
            if (index * VectorSize + VectorSize <= Values.Length)
            {
                Array.Copy(Values, index * VectorSize, result, 0, VectorSize);
            }
            return result;
        }


        public int[] GetZeroArray() { return DataUtils.GetIntZeroArray(VectorSize); }
        public int[] GetMinArray() { return DataUtils.GetIntMinArray(VectorSize); }
        public int[] GetMaxArray() { return DataUtils.GetIntMaxArray(VectorSize); }

        public static int[] GetZeroArray(int size) { return DataUtils.GetIntZeroArray(size); }
        public static int[] GetMinArray(int size) { return DataUtils.GetIntMinArray(size); }
        public static int[] GetMaxArray(int size) { return DataUtils.GetIntMaxArray(size); }


        public static IntStore CreateGrid(int vectorSize, int rows, int cols, SampleType sampleType)
        {
            int[] start = DataUtils.GetSizedIntArray(vectorSize, 0);
            int[] end = DataUtils.GetSizedIntArray(vectorSize, 1);
            end[0] = rows;
            end[1] = cols;
            int[] values = DataUtils.CombineIntArrays(start, end);
            return new IntStore(2, values, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 });
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<int>)Values).GetEnumerator();
        }
    }
}
