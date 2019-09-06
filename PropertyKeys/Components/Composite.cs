using System.Collections.Generic;
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
            Parent = parent;
            Stores = new Dictionary<PropertyID, PropertyStore>();
        }

        public void Step(float timeStep)
        {

        }
        public void SetT(float t)
        {
            foreach (var propertyStore in Stores.Values)
            {
                propertyStore.CurrentT = t; // hmm, these t's need to be dynamic, but lookup will be important - or is that just functional flow?
            }
        }

        // todo: this should probably bet get/set values for t/index by propertyID, but not access stores?
        // Need to compose and query nested values and t's using the hierarchy eg query t for a certain location in a grid, but props can vary at different speeds (or can they?)
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

        public void Update(float t)
        {

        }

        public void Draw()
        {

        }

        public Composite CreateChild()
        {
            return new Composite(this);
        }
    }
}
