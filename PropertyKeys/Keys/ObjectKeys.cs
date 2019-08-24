using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Keys
{
    public enum PropertyID : int
    {
        None = 0,
        Index,
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
    public class ObjectKeys
    {
        public Dictionary<PropertyID, ValueKey> Keys { get; }
        /// <summary>
        /// ObjectKeys can be composed by merging with parent keys. First match wins, though this could change to merge/add/interpolate with parents.
        /// </summary>
        public ObjectKeys Parent { get; set; }

        public ObjectKeys()
        {
            Keys = new Dictionary<PropertyID, ValueKey>();
        }

        public ValueKey GetValueKey(PropertyID propertyID)
        {
            ValueKey result = Keys[propertyID];
            if(result == null && Parent != null)
            {
                result = Parent.GetValueKey(propertyID);
            }
            return result;
        }
    }
}
