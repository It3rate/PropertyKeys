using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Components
{
    public abstract class BaseComposite : IComposite
    {
	    public string Name { get; set; }
	    public int Id { get; set; }
		public virtual int Capacity { get; set; }

        protected readonly Dictionary<PropertyId, int> _properties = new Dictionary<PropertyId, int>();

        protected BaseComposite()
	    {
		    Runner.CurrentComposites.AddToLibrary(this);
        }

        public virtual void AddProperty(PropertyId id, int storeId)
        {
	        _properties[id] = storeId;
        }
        public virtual void AddProperty(PropertyId id, IStore store)
        {
	        _properties[id] = store.Id;
        }
        public void AppendProperty(PropertyId id, IStore store)
	    {
		    if (_properties.ContainsKey(id))
		    {
			    IStore curStore = Runner.CurrentStores[_properties[id]];
			    if (curStore is MergingStore functionalStore)
			    {
				    functionalStore.Add(store);
			    }
			    else
			    {
                    AddProperty(id, new MergingStore(curStore, store) { CombineFunction = curStore.CombineFunction });
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
		    //Player.CurrentStores.RemoveActiveElementById(_properties[id]);
		    _properties.Remove(id);
	    }
	    public IStore GetLocalStore(PropertyId propertyId)
	    {
		    _properties.TryGetValue(propertyId, out var result);
		    return Runner.CurrentStores[result];
	    }
	    public virtual IStore GetStore(PropertyId propertyId)
	    {
		    return GetLocalStore(propertyId);
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
			    Runner.CurrentStores[storeId].Update(currentTime, deltaTime);
		    }
	    }
	    public virtual void EndUpdate(double currentTime, double deltaTime) { }

	    public virtual ISeries GetSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries)
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
		
	    public virtual ParametricSeries GetNormalizedPropertyAtT(PropertyId propertyId, ParametricSeries seriesT)
	    {
		    var store = GetStore(propertyId);
		    return store != null ? store.GetSampledTs(seriesT) :seriesT;
        }

        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }
    }
}
