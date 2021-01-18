using System.Collections.Generic;
using Motive.Components.Libraries;
using Motive.SeriesData;
using Motive.Stores;

/*
 Notes for refactor:
 
All queries are: floatSeries Query(floatSeries t)

Series - input always comes in as a series (time, signal, t), output is always a store?
	floats[]
	vectorSize (variables)
	count (samples) - actual sample count, can't be virtual
	query element or t
	ClampMode (Wrap, Mirror, clamp)
	
Sampler
	SampleCount (capacity) - create by sampler growth type or virtual count (or maybe step size)
	Strides
	SwizzleMap
	GrowthType (Product, sum)
	ClampTypes (Wrap, Mirror, clamp)
	Alignment (left, right) - too specific, should be property
	PropertySlots - takes properties from store if available, like renderer.
	query(t, series) - do we need series here? Maybe just t transform and store multiplies by series
			 - this would align with: floatSeries Query(floatSeries t)
	
Store
	Capacity - default from sampler, or 1	
	series - default 0-1
	sampler - default linear
	CombineFunction - how to combine with parent/blend'
	IsBaked - specified, not virtual data
	GetSampledT, GetSeries, Swizzle
	Properties - (currently in container, move to/combine with store? )
	query(t) - sends series to sampler
	
Container (probably a subtype of store)
	Children / Parent
	Properties -> Stores (maybe should be in store and used as way to parametrize store)
	external query: floatSeries Query(floatSeries t)
	internal query(property, t) 

Library
	Holds definitions and lookups

Store is a container base type, it can have properties (maybe not children?)
Parent hierarchy is based on containment, not definition - so property lookup can be different per instance
Experiment: Drag and object from one container to another to see it change based on parent settings (animation speed, color etc).
	
 */
namespace Motive.Components
{
	public interface IComposite : IDefinition
    {
        //string Name { get; set; }
        //int Id { get; }
        //void OnActivate();
        //void OnDeactivate();
        //void Update(double currentTime, double deltaTime);

        int Capacity { get; }


        void AddProperty(PropertyId id, IStore store);
        void AppendProperty(PropertyId id, IStore store);
        void RemoveProperty(PropertyId id);
        IStore GetStore(PropertyId propertyId);
        void GetDefinedStores(HashSet<PropertyId> ids);

        ParametricSeries GetNormalizedPropertyAtT(PropertyId propertyId, ParametricSeries seriesT);
        ISeries GetSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries);

    }
}
