using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;

namespace DataArcs.Stores
{
    public abstract class StoreBase : IStore
    {
	    private static int _idCounter = 1;
	    public int StoreId { get; }

        public CombineFunction CombineFunction { get; set; }
        public CombineTarget CombineTarget { get; set; }
        public abstract int VirtualCount { get; set; }

        protected StoreBase()
	    {
		    StoreId = _idCounter++;
	    }


        public abstract Series GetSeries(int index);

        public abstract Series GetSeriesAtIndex(int index, int virtualCount = -1);

        public abstract Series GetSeriesAtT(float t, int virtualCount = -1);

        public abstract void Update(float time);

        public abstract void ResetData();

        public abstract void HardenToData();

        public IEnumerator GetEnumerator()
        {
	        return new IStoreEnumerator(this);
        }
    }
}
