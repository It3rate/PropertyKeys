using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
    public abstract class BaseComposite : IComposite
    {
	    private static int _idCounter = 1;
	    public string Name { get; set; }
	    public int Id { get; }
		public virtual int Capacity { get; set; }

        protected readonly Dictionary<PropertyId, IStore> _stores = new Dictionary<PropertyId, IStore>();

	    public BaseComposite()
	    {
		    Id = _idCounter++;
		    Player.GetPlayerById(0).AddCompositeToLibrary(this);
        }

	    public virtual void AddProperty(PropertyId id, IStore store)
	    {
		    _stores[id] = store;
	    }
	    public void AppendProperty(PropertyId id, IStore store)
	    {
		    if (_stores.ContainsKey(id))
		    {
			    IStore curStore = _stores[id];
			    if (curStore is FunctionalStore)
			    {
				    ((FunctionalStore)curStore).Add(store);
			    }
			    else
			    {
				    _stores[id] = new FunctionalStore(curStore, store);
				    _stores[id].CombineFunction = curStore.CombineFunction;
				    //_stores[id].SampleCount = curStore.SampleCount;
                }
		    }
		    else
		    {
			    AddProperty(id, store);
		    }
	    }
	    public void RemoveProperty(PropertyId id)
	    {
		    _stores.Remove(id);
	    }
	    public virtual IStore GetStore(PropertyId propertyId)
	    {
		    _stores.TryGetValue(propertyId, out var result);
		    return result;
	    }
	    public virtual void GetDefinedStores(HashSet<PropertyId> ids)
	    {
		    foreach (var item in _stores.Keys)
		    {
			    ids.Add(item);
		    }
	    }

	    public void Update(double currentTime, double deltaTime)
	    {
		    StartUpdate(currentTime, deltaTime);
		    EndUpdate(currentTime, deltaTime);
	    }
	    public virtual void StartUpdate(double currentTime, double deltaTime)
	    {
		    foreach (var store in _stores.Values)
		    {
			    store.Update(deltaTime);
		    }
	    }
	    public virtual void EndUpdate(double currentTime, double deltaTime) { }

	    public virtual Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
	    {
		    var store = GetStore(propertyId);
		    var result = store?.GetValuesAtT(t);
		    if (parentSeries != null)
		    {
			    if (result != null)
			    {
				    result.CombineInto(parentSeries, store.CombineFunction, t);
			    }
			    else
			    {
				    result = parentSeries;
			    }
		    }
		    return result;
	    }
	    public virtual Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
	    {
		    var store = GetStore(propertyId);
		    var result = store?.GetValuesAtIndex(index);
		    if (parentSeries != null)
		    {
			    if (result != null)
			    {
				    result.CombineInto(parentSeries, store.CombineFunction);
			    }
			    else
			    {
				    result = parentSeries;
			    }
		    }
		    return result;
	    }
	    public virtual ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
	    {
		    var store = GetStore(propertyId);
		    return store != null ? store.GetSampledTs(seriesT) :seriesT;
        }

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
    }
}
