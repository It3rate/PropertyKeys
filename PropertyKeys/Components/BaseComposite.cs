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

        protected readonly Dictionary<PropertyId, int> _properties = new Dictionary<PropertyId, int>();

        protected BaseComposite()
	    {
		    Id = _idCounter++;
		    Player.GetPlayerById(0).AddCompositeToLibrary(this);
        }

	    public virtual void AddProperty(PropertyId id, IStore store)
	    {
		    _properties[id] = store.Id;
	    }
	    public void AppendProperty(PropertyId id, IStore store)
	    {
		    if (_properties.ContainsKey(id))
		    {
			    IStore curStore = Player.Stores[_properties[id]];
			    if (curStore is FunctionalStore functionalStore)
			    {
				    functionalStore.Add(store);
			    }
			    else
			    {
                    AddProperty(id, new FunctionalStore(curStore, store) { CombineFunction = curStore.CombineFunction });
                }
		    }
		    else
		    {
			    AddProperty(id, store);
		    }
	    }
	    public void RemoveProperty(PropertyId id)
	    {
			// can clean up removes is using ref counting, but maybe makes more sense to leave them and have a UI clean option?
		    //Player.Stores.RemoveActiveElementById(_properties[id]);
		    _properties.Remove(id);
	    }
	    public virtual IStore GetStore(PropertyId propertyId)
	    {
		    _properties.TryGetValue(propertyId, out var result);
		    return Player.Stores[result];
	    }
	    public virtual void GetDefinedStores(HashSet<PropertyId> ids)
	    {
		    foreach (var item in _properties.Keys)
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
		    foreach (var storeId in _properties.Values)
		    {
			    Player.Stores[storeId].Update(currentTime, deltaTime);
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
