using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;

namespace DataArcs.Components
{
    public abstract class CompositeBase
    {
        protected float CurrentT { get; set; }
        public GraphicBase Graphic { get; set; } // will become store props or something

        public abstract IStore GetStore(PropertyID propertyID);
        public abstract void GetDefinedProperties(HashSet<PropertyID> ids);
        public abstract void Update(float time);
        public abstract CompositeBase CreateChild();

        public virtual void Draw(Graphics g)
        {
            var itemStore = GetStore(PropertyID.Items);
            if (itemStore != null)
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
            Graphic.Draw(g, b, null, CurrentT);
            g.Restore(state);
        }


    }
}
