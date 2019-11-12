using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.SeriesData;

namespace DataArcs.Components.Interfaces
{
    public class QueryResult
    {
        //ParametricSeries T { get; }
        //ParametricSeries IndexT { get; }
        public float T { get; }
        public float IndexT { get; }

        //float DeltaT { get; }
        //float CurrentTime { get; }
        //IComposite CompositeRef { get; }
        public IRenderable Renderer { get; }

        private Dictionary<PropertyId, Series> Properties { get; }

        private List<PropertyId> Keys => Properties.Keys.ToList();
        public void ClearAll() => Properties.Clear();

        public void AddProperty(PropertyId propertyId, Series value) => Properties[propertyId] = value;

        public void Remove(PropertyId propertyId) => Properties.Remove(propertyId);

        public void QueryComposite(IComposite composite)
        {
        }
    }
}
