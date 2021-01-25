using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Graphic;
using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Components
{
    public interface IContainer : IComposite, IDrawable
    {
	    int[] ChildCounts { get; }       
	    
	    /// <summary>
	    /// Number of contained elements including all child counts.
	    /// </summary>
        int NestedCapacity { get; }
	    int NestedItemCountAtT(float t);

        IRenderable QueryPropertiesAtT(Dictionary<PropertyId, ISeries> data, float t, bool addLocalProperties);
	    ISeries GetNestedSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries);

        /// <summary>
        /// CurrentComposites can be composed by merging with parent CurrentComposites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        IContainer Parent { get; set; }

        void AddChild(IContainer child);
	    void RemoveChild(IContainer child);
	    IContainer CreateChild();
    }
}
