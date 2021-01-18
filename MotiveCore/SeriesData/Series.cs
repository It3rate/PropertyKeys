﻿using System;
using System.Collections;
using System.Collections.Generic;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;
using Motive.Components.Libraries;

namespace Motive.SeriesData
{
 //   public interface IA
 //   {
	//    List<IA> ToList();
 //   }
 //   abstract class A<T> :IA
 //   {
	//    public T Val { get; set; }
	//    public A<T> classVal;
	//    protected A()
	//    {
	//    }
 //       public List<IA> ToList() => new List<IA>(){ classVal };
 //   }

 //   class B : A<float>
 //   {
 //       public B()
 //       {
 //           var x = Val;
 //           classVal = this;
 //       }
 //   }

 //   class C
 //   {
 //       static void Test()
 //       {
 //           var x = new B();
 //           var y = x.ToList();
 //           var z = x.Val;
 //       }
	//}

	public enum SeriesType
	{
		Int,
		Float,
		Parametric,
		RectF,
		Bezier,
    }

//    public abstract class Series : ISeries
//    {
//	    public string Name { get; set; }
//	    public int Id { get; private set; }

//        public int VectorSize { get; set; }
//        public DiscreteClampMode IndexClampMode { get; set; } = DiscreteClampMode.Clamp;

//        public abstract int Count { get; }
//        public abstract SeriesType Type { get; }

//        /// <summary>
//        /// The raw size of the stored data array, ignores SampleCount and VectorSize.
//        /// </summary>
//        public abstract int DataSize { get; }

//        protected int StartIndex = 0;
//        protected int EndIndex = 0;
//        public float X => FloatValueAt(0);
//        public float Y => FloatValueAt(1);
//        public float Z => FloatValueAt(2);
//        public float W => FloatValueAt(3);

//        /// <summary>
//        /// Cached frame in four vectorSize data points, x, y, x + width, y + height.
//        /// </summary>
//        public RectFSeries Frame
//        {
//            get
//            {
//                if (_cachedFrame == null)
//                {
//                    CalculateFrame();
//                }

//                return _cachedFrame;
//            }
//            protected set => _cachedFrame = value;
//        }

//        /// <summary>
//        /// Cached size in two vectorSize data points, width and height of current data.
//        /// </summary>
//        public Series Size
//        {
//            get
//            {
//                if (_cachedSize == null)
//                {
//                    CalculateFrame();
//                }

//                return _cachedSize;
//            }
//            protected set => _cachedSize = value;
//        }

//        private RectFSeries _cachedFrame;
//        private Series _cachedSize;

//        protected Series(int vectorSize)
//        {
//            VectorSize = vectorSize;
//        }

//        public virtual void ResetData()
//        {
//        }

//        public abstract void ReverseEachElement();

//        protected abstract void CalculateFrame();

//        public virtual Series GetSeriesAt(float t) => GetSeriesAt(SamplerUtils.IndexFromT(Count, t));
//        public abstract Series GetSeriesAt(int index);
//        public abstract void SetSeriesAt(int index, Series series);

//        public virtual Series GetVirtualValueAt(float t)
//        {
//            Series result;
//            if (t >= 1)
//            {
//                result = GetSeriesAt(Count - 1);
//            }
//            else if (Count > 1)
//            {
//                var endIndex = Count - 1;
//                // interpolate between indexes to get virtual values from array.
//                var pos = Math.Min(1, Math.Max(0, t)) * endIndex;
//                var startIndex = (int)Math.Floor(pos);
//                startIndex = Math.Min(endIndex, Math.Max(0, startIndex));
//                if (pos < endIndex)
//                {
//                    var remainderT = pos - startIndex;
//                    result = GetSeriesAt(startIndex);
//                    var end = GetSeriesAt(startIndex + 1);
//                    result.InterpolateInto(end, remainderT);
//                }
//                else
//                {
//                    result = GetSeriesAt(startIndex);
//                }
//            }
//            else
//            {
//                result = GetSeriesAt(0);
//            }

//            return result;
//        }

//        public abstract float FloatValueAt(int index);
//        public abstract int IntValueAt(int index);
//        public abstract float[] FloatDataRef { get; }
//        public abstract int[] IntDataRef { get; }

//        public abstract void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
//        public abstract void InterpolateInto(Series b, float t);
//        public abstract void InterpolateInto(Series b, ParametricSeries seriesT);

//        public Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
//        public IStore Store(Sampler sampler = null)
//        {
//	        sampler = sampler ?? new LineSampler(this.Count);
//	        return new Store(this, sampler);
//        }

//        public abstract Series Copy();

//        public Series SumSlots(params Slot[] slots)
//        {
//            return SeriesUtils.SumSlots(this, slots);
//        }
//        public Series MultiplySlots(params Slot[] slots)
//        {
//            return SeriesUtils.MultiplySlots(this, slots);
//        }
//        public Series AverageSlots(params Slot[] slots)
//        {
//	        return SeriesUtils.AverageSlots(this, slots);
//        }
//        public Series MaxSlots(params Slot[] slots)
//        {
//	        return SeriesUtils.MaxSlots(this, slots);
//        }
//        public Series MinSlots(params Slot[] slots)
//        {
//	        return SeriesUtils.MinSlots(this, slots);
//        }

//        public abstract void Map(FloatEquation floatEquation);
//        public abstract void Append(Series series);

//        public List<Series> ToList()
//        {
//            var result = new List<Series>(Count);
//            for (int i = 0; i < Count; i++)
//            {
//                result.Add(GetSeriesAt(i));
//            }
//            return result;
//        }
//        public void SetByList(List<Series> items)
//        {
//            for (int i = 0; i < items.Count; i++)
//            {
//                SetSeriesAt(i, items[i]);
//            }
//        }
//        public void MapValuesToItemPositions(IntSeries items)
//        {
//            var selfCopy = Copy();
//            for (int i = 0; i < items.Count; i++)
//            {
//                int index = items.IntValueAt(i);
//                Series second = selfCopy.GetSeriesAt(i);
//                SetSeriesAt(index, second);
//            }
//        }
//        public void MapOrderToItemPositions(IntSeries items)
//        {
//            var selfCopy = Copy();
//	        for (int i = 0; i < items.Count; i++)
//	        {
//		        int index = items.IntValueAt(i);
//				Series second = selfCopy.GetSeriesAt(index);
//				SetSeriesAt(i, second);
//	        }
//        }

//        #region Enumeration
//        public IEnumerator GetEnumerator()
//        {
//            return new SeriesEnumerator(this);
//        }

//        private class SeriesEnumerator : IEnumerator
//        {
//            private Series _instance;
//            private int _position = -1;
//            public SeriesEnumerator(Series instance)
//            {
//                _instance = instance;
//            }
//            public bool MoveNext()
//            {
//                _position++;
//                return (_position < (int)(_instance.DataSize / _instance.VectorSize));
//            }
//            public object Current => _instance.GetSeriesAt(_position);

//            public void Reset()
//            {
//                _position = 0;
//            }
//        }
//        #endregion

//	    public bool AssignIdIfUnset(int id)
//	    {
//		    bool result = false;
//		    if (Id == 0 && id > 0)
//		    {
//			    Id = id;
//			    result = true;
//		    }
//		    return result;
//	    }
//        public void OnActivate()
//	    {
//	    }

//	    public void OnDeactivate()
//	    {
//	    }

//	    public virtual void Update(double currentTime, double deltaTime)
//	    {
//	    }
//    }
}