using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public interface IComposite
	{
		int CompositeId { get; }
		float CurrentT { get; set; }
		PolyShape Graphic { get; set; }

		IStore GetStore(PropertyId propertyId);
		void GetDefinedStores(HashSet<PropertyId> ids);
		void Update(float currentTime, float deltaTime);

		Series GetSeriesAtT(PropertyId propertyId, float t);
		Series GetSeriesAtIndex(PropertyId propertyId, int index);
		ParametricSeries GetSampledT(PropertyId propertyId, float t);

		IComposite CreateChild();
    }
}
