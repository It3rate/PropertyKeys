using System.Collections.Generic;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
	public class PropertyStore 
	{
		private readonly List<IStore> _stores;
        public float CurrentT { get; set; } = 0;

		public PropertyStore(params IStore[] stores)
		{
			_stores = new List<IStore>(stores);
		}

		public IStore this[int index]
		{
			get => _stores[index];
			set => _stores[index] = value;
		}

		public void Add(Store item)
		{
			_stores.Add(item);
		}

		public void Insert(int index, IStore item)
		{
			if (index >= 0 && index < _stores.Count)
			{
				_stores.Insert(index, item);
			}
		}

		public bool Remove(IStore item)
		{
			return _stores.Remove(item);
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < _stores.Count)
			{
				_stores.RemoveAt(index);
			}
		}

		public virtual void ResetData()
		{
			foreach (var store in _stores)
			{
				store.ResetData();
			}
		}

		public virtual void Update(float time)
		{
			foreach (var store in _stores)
			{
				store.Update(time);
			}
		}

		public Series GetSeriesAtIndex(int index, float t, int virtualCount = -1)
		{
			Series result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = _stores[startIndex].GetSeriesAtIndex(index, virtualCount);
			}
			else
			{
				result = BlendValueAtIndex(_stores[startIndex], _stores[endIndex], index, vT, virtualCount);
			}

			return result;
		}

		public Series GetSeriesAtT(float indexT, float t, int virtualCount = -1)
		{
			Series result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = _stores[startIndex].GetSeriesAtT(indexT, virtualCount);
			}
			else
			{
				result = BlendValueAtT(_stores[startIndex], _stores[endIndex], indexT, vT, virtualCount);
			}

			return result;
		}

		public int GetElementCountAt(float t)
		{
			int result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = _stores[startIndex].VirtualCount;
			}
			else
			{
				var sec = _stores[startIndex].VirtualCount;
				var eec = _stores[startIndex + 1].VirtualCount;
				result = sec + (int) (vT * (eec - sec));
			}

			return result;
		}


		public static Series BlendValueAtIndex(IStore start, IStore end, int index, float t, int virtualCount = -1)
		{
			var result = start.GetSeriesAtIndex(index, virtualCount);
			if (end != null)
			{
				var endAr = end.GetSeriesAtIndex(index, virtualCount);
				result.InterpolateInto(endAr, t);
			}

			return result;
		}

		public static Series BlendValueAtT(IStore start, IStore end, float indexT, float t, int virtualCount = -1)
		{
			var result = start.GetSeriesAtT(indexT, virtualCount);
			if (end != null)
			{
				var endAr = end.GetSeriesAtT(indexT, virtualCount);
				result.InterpolateInto(endAr, t);
			}

			return result;
		}
	}
}