using System.Collections;
using System.Collections.Generic;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public class WrappedSeries : ISeries
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
		public virtual float X => _series.X;
		public virtual float Y => _series.Y;
		public virtual float Z => _series.Z;
		public virtual float W => _series.W;
		public virtual int DataSize => _series.DataSize;

		public virtual ISeries GetSeriesAt(float t)
		{
			return _series.GetSeriesAt(t);
		}

		public virtual SeriesBase GetSeriesAt(int index)
		{
			return _series.GetSeriesAt(index);
		}

		public virtual void SetSeriesAt(int index, ISeries series)
		{
			_series.SetSeriesAt(index, series);
		}

		public virtual SeriesBase GetVirtualValueAt(float t)
		{
			return _series.GetVirtualValueAt(t);
		}

		public virtual float FloatValueAt(int index)
		{
			return _series.FloatValueAt(index);
		}

		public virtual int IntValueAt(int index)
		{
			return _series.IntValueAt(index);
		}

		public virtual float[] FloatDataRef => _series.FloatDataRef;
		public virtual int[] IntDataRef => _series.IntDataRef;

		public virtual void ReverseEachElement()
		{
			_series.ReverseEachElement();
		}

		public virtual void Append(SeriesBase series)
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

		public virtual Store CreateLinearStore(int capacity)
		{
			return _series.CreateLinearStore(capacity);
		}

		public virtual IStore Store(Sampler sampler = null)
		{
			return _series.Store(sampler);
		}

		public virtual List<ISeries> ToList()
		{
			return _series.ToList();
		}

		public virtual void SetByList(List<ISeries> items)
		{
			_series.SetByList(items);
		}

		public virtual ISeries Copy()
		{
			return _series.Copy();
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
	}
}