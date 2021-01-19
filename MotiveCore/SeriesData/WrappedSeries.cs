using System.Collections;
using System.Collections.Generic;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public abstract class WrappedSeries : ISeries
	{
		protected ISeries _series;
		public string Name { get => _series.Name; set => _series.Name = value; }
		public virtual int Id => _series.Id;

		public virtual bool AssignIdIfUnset(int id)
		{
			return _series.AssignIdIfUnset(id);
		}

		public virtual void OnActivate()
		{
			_series.OnActivate();
		}

		public virtual void OnDeactivate()
		{
			_series.OnDeactivate();
		}

		public virtual void Update(double currentTime, double deltaTime)
		{
			_series.Update(currentTime, deltaTime);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return _series.GetEnumerator();
		}

		public virtual int Count => _series.Count;
		public virtual SeriesType Type => _series.Type;
		public virtual int VectorSize
		{
			get => _series.VectorSize;
			set => _series.VectorSize = value;
		}
		public virtual DiscreteClampMode IndexClampMode
		{
			get => _series.IndexClampMode;
			set => _series.IndexClampMode = value;
		}
		public virtual RectFSeries Frame => _series.Frame;
		public virtual ISeries Size => _series.Size;
		public float X
		{
			get => _series.X;
			set => _series.X = value;
		}
		public float Y
		{
			get => _series.Y;
			set => _series.Y = value;
		}
		public float Z
		{
			get => _series.Z;
			set => _series.Z = value;
		}
		public float W
		{
			get => _series.W;
			set => _series.W = value;
		}
		public virtual int DataSize => _series.DataSize;

		public virtual ISeries GetSeriesAt(float t)
		{
			return _series.GetSeriesAt(t);
		}

		public virtual ISeries GetSeriesAt(int index)
		{
			return _series.GetSeriesAt(index);
		}

		public virtual void SetSeriesAt(int index, ISeries series)
		{
			_series.SetSeriesAt(index, series);
		}

		public virtual ISeries GetVirtualValueAt(float t)
		{
			return _series.GetVirtualValueAt(t);
		}

		public virtual float FloatValueAt(int index)
		{
			return _series.FloatValueAt(index);
		}

		public void SetFloatValueAt(int index, float value)
		{
			_series.SetFloatValueAt(index, value);
		}

		public virtual int IntValueAt(int index)
		{
			return _series.IntValueAt(index);
		}

		public void SetIntValueAt(int index, int value)
		{
			_series.SetIntValueAt(index, value);
		}

		public virtual float[] FloatDataRef => _series.FloatDataRef;
		public virtual int[] IntDataRef => _series.IntDataRef;

		public virtual void ReverseEachElement()
		{
			_series.ReverseEachElement();
		}

		public virtual void Append(ISeries series)
		{
			_series.Append(series);
		}

		public virtual void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0)
		{
			_series.CombineInto(b, combineFunction, t);
		}

		public virtual void InterpolateInto(ISeries b, float t)
		{
			_series.InterpolateInto(b, t);
		}

		public virtual void InterpolateInto(ISeries b, ParametricSeries seriesT)
		{
			_series.InterpolateInto(b, seriesT);
		}

        public virtual List<ISeries> ToList()
		{
			return _series.ToList();
		}

		public virtual void SetByList(List<ISeries> items)
		{
			_series.SetByList(items);
		}

		public virtual void ResetData()
		{
			_series.ResetData();
		}

		public virtual void Map(FloatEquation floatEquation)
		{
			_series.Map(floatEquation);
		}

		public virtual void MapValuesToItemPositions(IntSeries items)
		{
			_series.MapValuesToItemPositions(items);
		}

		public virtual void MapOrderToItemPositions(IntSeries items)
		{
			_series.MapOrderToItemPositions(items);
		}

		public virtual Store CreateLinearStore(int capacity) => new Store(this, new LineSampler(capacity));
		public virtual IStore Store(Sampler sampler = null)
		{
			sampler = sampler ?? new LineSampler(this.Count);
			return new Store(this, sampler);
		}

		public abstract ISeries Copy();
	}
}