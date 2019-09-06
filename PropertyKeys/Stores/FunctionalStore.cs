using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class FunctionalStore : Store
    {
        public readonly Store[] Stores;
        public override Series Series => null;

        public FunctionalStore(Store[] stores)
        {
            Stores = stores;
        }
        public override Series GetValueAtIndex(int index)
        {
            Series series = Stores[0].GetValueAtIndex(index);
            for (int i = 1; i < Stores.Length; i++)
            {
                Series b = Stores[i].GetValueAtIndex(index);
                series.Combine(b, Stores[i].CombineFunction);
            }
            return series;
        }

        public override Series GetValueAtT(float t)
        {
            Series series = Stores[0].GetValueAtT(t);
            for (int i = 1; i < Stores.Length; i++)
            {
                Series b = Stores[i].GetValueAtT(t);
                series.Combine(b, Stores[i].CombineFunction);
            }
            return series;
        }

        public override void HardenToData()
        {
            foreach (var store in Stores)
            {
                store.HardenToData();
            }
        }
    }
}
