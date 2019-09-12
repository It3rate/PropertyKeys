using System.Collections.Generic;
using DataArcs.Series;

namespace DataArcs.Stores
{
    public class PropertyStore
    {
        private readonly List<Store> _stores;
        public float CurrentT { get; set; } = 0;

        public PropertyStore(params Store[] stores)
        {
            _stores = new List<Store>(stores);
        }

        public Store this[int index]
        {
            get => _stores[index];
            set => _stores[index] = value;
        }

        public void Add(Store item)
        {
            _stores.Add(item);
        }

        public void Insert(int index, Store item)
        {
            if (index >= 0 && index < _stores.Count) _stores.Insert(index, item);
        }

        public bool Remove(Store item)
        {
            return _stores.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _stores.Count) _stores.RemoveAt(index);
        }

        public virtual void Reset()
        {
            foreach (var store in _stores) store.Reset();
        }

        public virtual void Update(float time)
        {
            foreach (var store in _stores) store.Update(time);
        }

        public Series.Series GetValuesAtIndex(int index, float t, int virtualCount = -1)
        {
            Series.Series result;

            SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

            if (startIndex == endIndex)
                result = _stores[startIndex].GetValueAtIndex(index, virtualCount);
            else
                result = BlendValueAtIndex(_stores[startIndex], _stores[endIndex], index, vT, virtualCount);
            return result;
        }

        public Series.Series GetValuesAtT(float indexT, float t, int virtualCount = -1)
        {
            Series.Series result;

            SeriesUtils.GetScaledT(t, _stores.Count, out var vT, out var startIndex, out var endIndex);

            if (startIndex == endIndex)
                result = _stores[startIndex].GetValueAtT(indexT, virtualCount);
            else
                result = BlendValueAtT(_stores[startIndex], _stores[endIndex], indexT, vT, virtualCount);
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


        public static Series.Series BlendValueAtIndex(Store start, Store end, int index, float t, int virtualCount = -1)
        {
            var result = start.GetValueAtIndex(index, virtualCount);
            if (end != null)
            {
                var endAr = end.GetValueAtIndex(index, virtualCount);
                result.InterpolateInto(endAr, t);
            }

            return result;
        }

        public static Series.Series BlendValueAtT(Store start, Store end, float indexT, float t, int virtualCount = -1)
        {
            var result = start.GetValueAtT(indexT, virtualCount);
            if (end != null)
            {
                var endAr = end.GetValueAtT(indexT, virtualCount);
                result.InterpolateInto(endAr, t);
            }

            return result;
        }
    }
}