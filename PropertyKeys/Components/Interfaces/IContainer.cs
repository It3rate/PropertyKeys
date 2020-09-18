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
    public interface IContainer : IComposite, IDrawable
    {
	    int[] ChildCounts { get; }
	    int NestedItemCount { get; }
	    int NestedItemCountAtT(float t);

        IRenderable QueryPropertiesAtT(Dictionary<PropertyId, Series> data, float t, bool addLocalProperties);
	    Series GetNestedSeriesAtT(PropertyId propertyId, float t, Series parentSeries);
	    Series GetNestedSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries);

        /// <summary>
        /// CurrentComposites can be composed by merging with parent CurrentComposites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        IContainer Parent { get; set; }

        void AddChild(IContainer child);
	    void RemoveChild(IContainer child);
	    IContainer CreateChild();
    }
}
