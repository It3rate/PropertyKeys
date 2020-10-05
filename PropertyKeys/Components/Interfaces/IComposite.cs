using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components.Libraries;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

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

        ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT);
        Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries);

    }
}
