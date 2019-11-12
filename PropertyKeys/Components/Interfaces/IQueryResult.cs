using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.SeriesData;

namespace DataArcs.Components.Interfaces
{
    public interface IQueryResult
    {
	    float OrderT { get; }
	    ParametricSeries IndexT { get; }
        //float DeltaT { get; }
        //float CurrentTime { get; }
        //IComposite CompositeRef { get; }
        IRenderable Renderer { get; }

        Dictionary<PropertyId, Series> Properties { get; }

        List<PropertyId> Keys { get; }
        void ClearAll();
        void AddProperty(PropertyId propertyId, Series value);
        void Remove(PropertyId propertyId);

        void Parse(IComposite composite);
    }
}
