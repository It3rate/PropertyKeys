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

        public abstract Series GetValueAtIndex(Series series, int index);
        public abstract Series GetValueAtT(Series series, float t);
        public abstract float GetTAtT(float t);

        public abstract void Update(float t);
    }
}
