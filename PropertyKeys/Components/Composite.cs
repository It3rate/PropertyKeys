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
		private Dictionary<PropertyID, IStore> Stores { get; }
		public GraphicBase Graphic { get; set; }

        public IntSeries Items { get; set; }

        private Series _location;
        private Series _color;

        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        public Composite Parent { get; set; }

		public Composite(Composite parent = null)
		{
			Parent = parent;
			Stores = new Dictionary<PropertyID, IStore>();
		}

		// todo: this should probably bet get/set values for t/index by propertyID, but not access stores?
		// Need to compose and query nested values and t's using the hierarchy eg query t for a certain location in a grid, but props can vary at different speeds (or can they?)
		public IStore GetStore(PropertyID propertyID)
		{
			Stores.TryGetValue(propertyID, out var result);
			if (result == null && Parent != null)
			{
				result = Parent.GetStore(propertyID);
			}

			return result;
		}

		public void AddProperty(PropertyID id, BlendStore propertyStore)
		{
			Stores.Add(id, propertyStore);
		}

		public void RemoveProperty(PropertyID id, BlendStore propertyStore)
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

            if (time <= 0.05f && shouldShuffle)
            {
                SeriesUtils.Shuffle(((BlendStore)GetStore(PropertyID.Location))._stores[1].GetSeries(0));
            }
            if (time > 0.99 && shouldShuffle)
            {
                Series s = ((BlendStore)GetStore(PropertyID.Location))._stores[0].GetSeries(0);
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }

            t = time;
		}

		public void Draw(Graphics g)
		{
            var itemStore = GetStore(PropertyID.Items);
            if(itemStore != null)
            {
                foreach (Series series in itemStore)
                {
                    DrawAtIndex(series.IntDataAt(0), itemStore.VirtualCount, g);
                }
            }
            else
            {
                var count = 50;// GetStore(PropertyID.Location).GetElementCountAt(t);
                for (var i = 0; i < count; i++)
                {
                    DrawAtIndex(i, count, g);
                }
            }


            //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
        }
        public void DrawAtIndex(int index, int count, Graphics g)
        {
            var loc = GetStore(PropertyID.Location);
            var col = GetStore(PropertyID.FillColor);
            var it = index / (count - 1f);
            Series v = loc.GetSeriesAtT(it, count); 

            var c = col.GetSeriesAtT(it).RGB();
            Brush b = new SolidBrush(c);
            var state = g.Save();
            var scale = 1f; //  + t * 0.2f;
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(v.X() / scale, v.Y() / scale);
            Graphic.Draw(g, b, null, t);
            g.Restore(state);
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

        Items,
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