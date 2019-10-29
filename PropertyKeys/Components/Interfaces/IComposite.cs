using System;
using System.Collections.Generic;
using System.Drawing;
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
        string Name { get; set; }

        int CompositeId { get; }
        int Capacity { get; }

        void Update(float currentTime, float deltaTime);

        void AddProperty(PropertyId id, IStore store);
        void AppendProperty(PropertyId id, IStore store);
        void RemoveProperty(PropertyId id, BlendStore store);
        IStore GetStore(PropertyId propertyId);
        void GetDefinedStores(HashSet<PropertyId> ids);

        ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT);
        Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries);
		Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries);

        void OnActivate();
        void OnDeactivate();
    }
}
