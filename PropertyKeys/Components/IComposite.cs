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
        float InputT { get; set; }
        IStore Items { get; }
        int Capacity { get; }
        int TotalItemCount { get; }
        int[] ChildCounts { get; }
        IComposite Background { get; set; }

        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        IComposite Parent { get; set; }

        void AddProperty(PropertyId id, IStore store);
		void AppendProperty(PropertyId id, IStore store);
		void RemoveProperty(PropertyId id, BlendStore store);
        IStore GetStore(PropertyId propertyId);
		void GetDefinedStores(HashSet<PropertyId> ids);

        void AddChild(IComposite child);
        void RemoveChild(IComposite child);

        void Update(float currentTime, float deltaTime);

        void AddLocalPropertiesAtT(Dictionary<PropertyId, Series> data, float t);
        IRenderable QueryPropertiesAtT(Dictionary<PropertyId, Series> data, float t, bool addLocalProperties);

        Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries);
		Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries);
		ParametricSeries GetSampledT(PropertyId propertyId, float t);
        Series GetChildSeriesAtT(PropertyId propertyId, float t, Series parentSeries);
        Series GetChildSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries);

        void Draw(IComposite composite, Graphics g, Dictionary<PropertyId, Series> dict);

        IComposite CreateChild();
    }
}
