﻿using System;
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
		int CompositeId { get; }
		float InputT { get; set; }
		IDrawable Graphic { get; set; }
        IStore Items { get; }
        int TotalItemCount { get; }

        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        IComposite Parent { get; set; }

        void AddProperty(PropertyId id, IStore store);
		void AppendProperty(PropertyId id, IStore store);
		void RemoveProperty(PropertyId id, BlendStore store);
        IStore GetStore(PropertyId propertyId);
		void GetDefinedStores(HashSet<PropertyId> ids);

		void Update(float currentTime, float deltaTime);

		Series GetSeriesAtT(PropertyId propertyId, float t);
		Series GetSeriesAtIndex(PropertyId propertyId, int index);
		ParametricSeries GetSampledT(PropertyId propertyId, float t);

		void Draw(IComposite composite, Graphics g);

        IComposite CreateChild();
    }
}
