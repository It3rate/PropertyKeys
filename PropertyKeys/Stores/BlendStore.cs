using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms.Layout;
using DataArcs.Samplers;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
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
        public BlendStore(IStore[] stores, Sampler sampler):base()
        {
	        _stores = new List<IStore>(stores);
	        Sampler = sampler;
        }

        public void Reverse() { _stores.Reverse();}

        public override Series GetFullSeries()
		{
			return GetSeriesAtIndex(0, CurrentT);
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

        public override void BakeData()
		{
			foreach (var store in _stores)
			{
				store.BakeData();
			}
		}

        public override Series GetValuesAtIndex(int index)
        {
	        return GetSeriesAtIndex(index, CurrentT * Capacity);
        }

        public override Series GetValuesAtT(float t)
        {
	        return GetSeriesAtT(t, CurrentT);
        }

        public override ParametricSeries GetSampledTs(float t)
        {
	        SamplerUtils.InterpolatedIndexAndRemainder(_stores.Count, t, out var startIndex, out var vT);
	        vT = _easing?.GetValuesAtT(vT).X ?? vT;

	        ParametricSeries result = _stores[startIndex].GetSampledTs(vT);
	        if (vT > SamplerUtils.TOLERANCE && startIndex < _stores.Count - 1)
	        {
		        var endValue = _stores[startIndex + 1].GetSampledTs(t);
		        result.InterpolateInto(endValue, vT);
	        }
	        return result;
        }

        public int GetElementCountAt(float t)
		{
			SamplerUtils.InterpolatedIndexAndRemainder(_stores.Count, t, out var startIndex, out var vT);
			vT = _easing?.GetValuesAtT(vT).X ?? vT;

			int result = _stores[startIndex].Capacity;

            if (vT > SamplerUtils.TOLERANCE && startIndex < _stores.Count - 1)
            {
	            var endCapacity = _stores[startIndex + 1].Capacity;
	            result += (int)(vT * (endCapacity - result));
			}
			return result;
		}

        public Series GetSeriesAtIndex(int index, float t)
        {
	        return GetSeriesAtT(index / (_stores.Count - 1f), t);
        }

        public Series GetSeriesAtT(float indexT, float t)
        {
	        SamplerUtils.InterpolatedIndexAndRemainder(_stores.Count, indexT, out var startIndex, out var remainder);
	        remainder = _easing?.GetValuesAtT(remainder).X ?? remainder;
            float storeInterpolation = remainder;
            
	        if (Sampler != null)
            {
		        var sample = Sampler.GetSampledTs(indexT);
                storeInterpolation = sample[sample.VectorSize - 1];
	        }
            
	        Series result = _stores[startIndex].GetValuesAtT(remainder);
            if (startIndex < _stores.Count - 1)
            {
	            Series endSeries = _stores[startIndex + 1].GetValuesAtT(remainder);
	            result.InterpolateInto(endSeries, storeInterpolation);
	        }
	        return  result; 
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