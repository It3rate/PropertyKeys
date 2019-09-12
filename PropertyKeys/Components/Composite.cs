using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DataArcs.Graphic;
using DataArcs.Series;
using DataArcs.Stores;

namespace DataArcs.Components
{
    public enum PropertyID : int
    {
        None = 0,
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

        public void Step(float timeStep)
        {
        }

        public void SetT(float t)
        {
            foreach (var propertyStore in Stores.Values)
                propertyStore.CurrentT =
                    t; // hmm, these t's need to be dynamic, but lookup will be important - or is that just functional flow?
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

        private float t;
        public bool shouldShuffle; // basis for switching to events

        public virtual void Update(float time)
        {
            foreach (var store in Stores.Values) store.Update(time);

            var floorT = (int) time;
            t = time - floorT;
            if (floorT % 2 == 0)
            {
                t = 1.0f - t;
            }

            if (t <= 0.005f && shouldShuffle)
            {
                SeriesUtils.Shuffle(GetPropertyStore(PropertyID.Location)[1].Series);
            }
        }

        public void Draw(Graphics g)
        {
            var loc = GetPropertyStore(PropertyID.Location);
            var col = GetPropertyStore(PropertyID.FillColor);
            var wander = GetPropertyStore(PropertyID.RandomMotion);

            var easedT = t; // Easing.GetTAtT(t, loc.EasingType);
            var count = loc.GetElementCountAt(easedT);
            float[] v = {0, 0};
            for (var i = 0; i < count; i++)
            {
                //if (i > 88 && i < 111)//count - 1)
                //{
                //    float itx = i / (float)(count - 1f);
                //    var vx = v = loc.GetValuesAtT(itx, easedT).FloatData;
                //    Debug.WriteLine(i + "::" + vx[0] + " : " + vx[1]);
                //}
                var it = i / (float) (count - 1f);
                v = loc.GetValuesAtT(it, easedT, count).FloatData;

                var c = GraphicUtils.GetRGBColorFrom(col.GetValuesAtT(it, easedT));
                Brush b = new SolidBrush(c);
                var state = g.Save();
                var scale = 1f; //  + t * 0.2f;
                g.ScaleTransform(scale, scale);
                g.TranslateTransform(v[0] / scale, v[1] / scale);
                Graphic.Draw(g, b, null, easedT);
                g.Restore(state);
            }

            //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
        }

        public Composite CreateChild()
        {
            return new Composite(this);
        }
    }
}