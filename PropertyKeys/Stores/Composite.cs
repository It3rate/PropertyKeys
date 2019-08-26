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
        RandomMotion,

        Custom = 0x1000,
    }
    public class Composite
    {
        private Dictionary<PropertyID, PropertyStore> Stores { get; }
        /// <summary>
        /// Composites can be composed by merging with parent Composites. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        public Composite Parent { get; set; }

        public Composite(Composite parent = null)
        {
            Stores = new Dictionary<PropertyID, PropertyStore>();
            Parent = parent;
            Stores = new Dictionary<PropertyID, PropertyStore>();
        }

        public PropertyStore GetPropertyStore(PropertyID propertyID)
        {
            Stores.TryGetValue(propertyID, out PropertyStore result);
            if(result == null && Parent != null)
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

        public Composite CreateChild()
        {
            return new Composite(this);
        }
    }
}
