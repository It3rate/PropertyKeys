using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Stores;

namespace DataArcs.Mutators
{
    public abstract class Mutator
    {
        protected Store Store { get; set; }

        public Mutator(Store store)
        {
            Store = store;
            Store.HardenToData();
        }

        public virtual Series GetValueAtIndex(Series series, int index) => Store.GetValueAtIndex(index);
        public virtual Series GetValueAtT(Series series, float t) => Store.GetValueAtT(t);
        public virtual float GetTAtT(float t) => Store.GetTatT(t);

        public abstract void Update(float t);
        public abstract void Reset();
    }
}
