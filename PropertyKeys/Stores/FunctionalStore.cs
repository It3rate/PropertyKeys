namespace DataArcs.Stores
{
    public class FunctionalStore : Store
    {
        public readonly Store[] Stores;
        public override Series.Series Series => Stores[0].Series;

        //public FunctionalStore(Store[] stores)
        //{
        //    Stores = stores;
        //}
        public FunctionalStore(params Store[] stores)
        {
            Stores = stores;
        }

        public override Series.Series GetValueAtIndex(int index, int virtualCount = -1)
        {
            var series = Stores[0].GetValueAtIndex(index, virtualCount);
            for (var i = 1; i < Stores.Length; i++)
            {
                var b = Stores[i].GetValueAtIndex(index, virtualCount);
                series.CombineInto(b, Stores[i].CombineFunction);
            }

            return series;
        }

        public override Series.Series GetValueAtT(float t, int virtualCount = -1)
        {
            var series = Stores[0].GetValueAtT(t, virtualCount);
            for (var i = 1; i < Stores.Length; i++)
            {
                var b = Stores[i].GetValueAtT(t, virtualCount);
                series.CombineInto(b, Stores[i].CombineFunction);
            }

            return series;
        }

        public override void HardenToData()
        {
            foreach (var store in Stores) store.HardenToData();
        }

        public override void Reset()
        {
            foreach (var store in Stores) store.Reset();
        }

        public override void Update(float time)
        {
            foreach (var store in Stores) store.Update(time);
        }
    }
}