using System.Collections.Generic;
using DataArcs.Components.Libraries;
using DataArcs.SeriesData;
using DataArcs.Stores;

/*
 Notes for refactor:
 
All queries are: floatSeries Query(floatSeries t)

Series 
	floats[]
	vectorSize (variables)
	count (samples)
	query element or t
	
Sampler
	SampleCount (capacity)
	Strides
	SwizzleMap
	GrowthType (Product, sum)
	ClampTypes (Wrap, Mirror, clamp)
	Alignment (left, right) - too specific, should be property
	query(t, series) - do we need series here? Maybe just t transform and store multiplies by series
			 - this would align with: floatSeries Query(floatSeries t)
	
Store
	Capacity - default from sampler, or 1	
	series - default 0-1
	sampler - default linear
	CombineFunction - how to combine with parent/blend'
	IsBaked - specified, not virtual data
	GetSampledT, GetSeries, Swizzle
	query(t) - sends series to sampler
	
Container (probably a subtype of store)
	Children / Parent
	Properties -> Stores (maybe should be in store and used as way to paramertize store)
	external query: floatSeries Query(floatSeries t)
	internal query(property, t) 
	
 */
namespace DataArcs.Components
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
        Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries);

    }
}
