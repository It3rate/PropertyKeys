using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
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

        Starness,
        Roundness,
        Radius,

        Custom = 0x1000,
    }
    public class Composite
    {
        public Dictionary<PropertyID, PropertyStore> Stores { get; }
        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        public Composite Parent { get; set; }

        public Composite(Composite parent = null)
        {
            Parent = parent;
            Stores = new Dictionary<PropertyID, PropertyStore>();
        }

        public PropertyStore GetPropertyStore(PropertyID propertyID)
        {
            PropertyStore result = Stores[propertyID];
            if(result == null && Parent != null)
            {
                result = Parent.GetPropertyStore(propertyID);
            }
            return result;
        }

        public Composite CreateChild()
        {
            return new Composite(this);
        }
    }
}
