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
        public override Series Series => Stores[0].Series;

        //public FunctionalStore(Store[] stores)
        //{
        //    Stores = stores;
        //}
        public FunctionalStore(params Store[] stores)
        {
            Stores = stores;
        }
        public override Series GetValueAtIndex(int index, int virtualCount = -1)
        {
            Series series = Stores[0].GetValueAtIndex(index, virtualCount);
            for (int i = 1; i < Stores.Length; i++)
            {
                Series b = Stores[i].GetValueAtIndex(index, virtualCount);
                series.Combine(b, Stores[i].CombineFunction);
            }
            return series;
        }

        public override Series GetValueAtT(float t, int virtualCount = -1)
        {
            Series series = Stores[0].GetValueAtT(t, virtualCount);
            for (int i = 1; i < Stores.Length; i++)
            {
                Series b = Stores[i].GetValueAtT(t, virtualCount);
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
        public override void Reset()
        {
            foreach (var store in Stores)
            {
                store.Reset();
            }
        }
        public override void Update(float time)
        {
            foreach (var store in Stores)
            {
                store.Update(time);
            }
        }
    }
}
