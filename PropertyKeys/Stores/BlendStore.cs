using System;
using System.Collections;
using System.Collections.Generic;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
	// todo: maybe. Remove blendStore as all transitions will happen at the composite level or functional blends.
	public class BlendStore : StoreBase
    {
		private readonly List<IStore> _stores;
        public float CurrentT { get; set; }
        private readonly IStore _easing;
		
        public override int Capacity
        {
            get
            {
	            int result = 0;
                foreach (var store in _stores) { result = Math.Max(result, store.Capacity); } // todo: use combine function for virtual count, rename to capacity.
                return result;
            }
        }

        public BlendStore(params IStore[] stores)
        {
	        _stores = new List<IStore>(stores);
        }
        public BlendStore(IStore[] stores, IStore easing)
        {
	        _stores = new List<IStore>(stores);
	        _easing = easing;
        }

        public override Series GetFullSeries(int index)
		{
			return GetSeriesAtIndex(index, CurrentT);
		}

		public override Series GetValuesAtIndex(int index)
		{
			return GetSeriesAtIndex(index, CurrentT);
		}

		public override Series GetValuesAtT(float t)
		{
			return GetSeriesAtT(t, CurrentT);
		}

        public override ParametricSeries GetSampledTs(float t)
        {
            ParametricSeries result;

            SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);
            vT = _easing?.GetValuesAtT(vT).X ?? vT;

            if (startIndex == endIndex)
            {
                result = _stores[startIndex].GetSampledTs(vT);
            }
            else
            {
                result = _stores[startIndex].GetSampledTs(t);
                var endPS = _stores[endIndex].GetSampledTs(t);
                result.InterpolateInto(endPS, vT);
            }

            return result;
        }

        public override void Update(float deltaTime)
		{
			CurrentT = deltaTime;
			foreach (var store in _stores)
			{
				store.Update(deltaTime);
			}
		}

		public override void ResetData()
		{
			foreach (var store in _stores)
			{
				store.ResetData();
			}
		}

        public override void HardenToData()
		{
			foreach (var store in _stores)
			{
				store.HardenToData();
			}
		}

		public Series GetSeriesAtIndex(int index, float t)
		{
			Series result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);
			vT = _easing?.GetValuesAtT(vT).X ?? vT;

            if (startIndex == endIndex)
			{
				result = _stores[startIndex].GetValuesAtIndex(index);
			}
			else
			{
				result = BlendValueAtIndex(_stores[startIndex], _stores[endIndex], index, vT);
			}

			return result;
		}

		public Series GetSeriesAtT(float indexT, float t)
		{
			Series result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);
			vT = _easing?.GetValuesAtT(vT).X ?? vT;

            if (startIndex == endIndex)
			{
				result = _stores[startIndex].GetValuesAtT(indexT);
			}
			else
			{
				result = BlendValueAtT(_stores[startIndex], _stores[endIndex], indexT, vT);
			}

			return result;
		}

		public int GetElementCountAt(float t)
		{
			int result;

			SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

			if (startIndex == endIndex)
			{
				result = _stores[startIndex].Capacity;
			}
			else
			{
				var sec = _stores[startIndex].Capacity;
				var eec = _stores[startIndex + 1].Capacity;
				result = sec + (int) (vT * (eec - sec));
			}

			return result;
		}

        public static Series BlendValueAtIndex(IStore start, IStore end, int index, float t)
        {
            var result = start.GetValuesAtIndex(index);
            if (end != null)
            {
                var endAr = end.GetValuesAtIndex(index);
                result.InterpolateInto(endAr, t);
            }

            return result;
        }

        public static Series BlendValueAtT(IStore start, IStore end, float indexT, float t)
        {
            var result = start.GetValuesAtT(indexT);
            if (end != null)
            {
                var endAr = end.GetValuesAtT(indexT);

                result.InterpolateInto(endAr, t);
            }

            return result;
        }


        //public IStore this[int index] // todo: change to return series
        //{
        // get => _stores[index];
        // set => _stores[index] = value;
        //}

        public IStore GetStoreAt(int index) => _stores[Math.Max(0, Math.Min(_stores.Count - 1, index))];
        public void Add(IStore item) => _stores.Add(item);
        public void Insert(int index, IStore item) => _stores.Insert(Math.Max(0, Math.Min(_stores.Count - 1, index)), item);
        public bool Remove(IStore item) => _stores.Remove(item);
        public void RemoveAt(int index)
        {
	        if (index >= 0 && index < _stores.Count)
	        {
		        _stores.RemoveAt(index);
	        }
        }
        public bool RemoveById(int id)
        {
	        bool result = false;
	        int index = _stores.FindIndex(s => s.StoreId == id);
	        if (index > -1)
	        {
		        _stores.RemoveAt(index);
		        result = true;
	        }
	        return result;
        }

    }
}