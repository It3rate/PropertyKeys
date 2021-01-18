﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Graphic;
using Motive.SeriesData;

namespace Motive.Components.Interfaces
{
    public class QueryResult
    {
        //ParametricSeries T { get; }
        //ParametricSeries IndexT { get; }
        public float T { get; set; }
        public float IndexT { get; set; }
        public IRenderable Renderer { get; set; }
        private Dictionary<PropertyId, ISeries> Properties { get; } = new Dictionary<PropertyId, ISeries>(20);

        //float DeltaT { get; }
        //float CurrentTime { get; }
        //IComposite CompositeRef { get; }

        private List<PropertyId> Keys => Properties.Keys.ToList();
        public void ClearAll() => Properties.Clear();
        public ISeries GetProperty(PropertyId propertyId) => Properties[propertyId];
        public void AddProperty(PropertyId propertyId, ISeries value) => Properties[propertyId] = value;
        public void Remove(PropertyId propertyId) => Properties.Remove(propertyId);

        public void QueryComposite(IComposite composite)
        {
        }
    }
}