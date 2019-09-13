using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Composite
	{
		private Dictionary<PropertyID, PropertyStore> Stores { get; }
		public GraphicBase Graphic { get; set; }

		/// <summary>
		/// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
		/// </summary>
		public Composite Parent { get; set; }

		public Composite(Composite parent = null)
		{
			Parent = parent;
			Stores = new Dictionary<PropertyID, PropertyStore>();
		}

		// todo: this should probably bet get/set values for t/index by propertyID, but not access stores?
		// Need to compose and query nested values and t's using the hierarchy eg query t for a certain location in a grid, but props can vary at different speeds (or can they?)
		public PropertyStore GetPropertyStore(PropertyID propertyID)
		{
			Stores.TryGetValue(propertyID, out var result);
			if (result == null && Parent != null)
			{
				result = Parent.GetPropertyStore(propertyID);
			}

			return result;
		}

		public void AddProperty(PropertyID id, PropertyStore propertyStore)
		{
			Stores.Add(id, propertyStore);
		}

		public void RemoveProperty(PropertyID id, PropertyStore propertyStore)
		{
			Stores.Remove(id);
		}
		
		public bool shouldShuffle; // basis for switching to events

		private float t;
		public virtual void Update(float time)
		{
			foreach (var store in Stores.Values)
			{
				store.Update(time);
			}

			if (time <= 0.005f && shouldShuffle)
			{
				SeriesUtils.Shuffle(GetPropertyStore(PropertyID.Location)[1].GetSeries(0));
			}

			t = time;
		}

		public void Draw(Graphics g)
		{
			var loc = GetPropertyStore(PropertyID.Location);
			var col = GetPropertyStore(PropertyID.FillColor);
			
			var count = loc.GetElementCountAt(t);
			for (var i = 0; i < count; i++)
			{
				var it = i / (float) (count - 1f);
				Series v = loc.GetSeriesAtT(it, t, count);

				var c = col.GetSeriesAtT(it, t).RGB();
				Brush b = new SolidBrush(c);
				var state = g.Save();
				var scale = 1f; //  + t * 0.2f;
				g.ScaleTransform(scale, scale);
				g.TranslateTransform(v.X() / scale, v.Y() / scale);
				Graphic.Draw(g, b, null, t);
				g.Restore(state);
			}

			//g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
		}

		public Composite CreateChild()
		{
			return new Composite(this);
		}
	}

	public enum PropertyID : int
	{
		None = 0,
		TModifier,

		Shape,
		Transform,
		Location,
		Size,
		Scale,
		Rotation,
		FillColor,
		PenColor,
		T,
		StartTime,
		Duration,
		Easing,
		SampleType,

		Graphic,
		Starness,
		Roundness,
		Radius,
		RandomMotion,

		Custom = 0x1000,
	}
}