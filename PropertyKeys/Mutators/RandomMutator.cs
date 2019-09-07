using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Stores;

namespace DataArcs.Mutators
{
    public class RandomMutator : Mutator
    {
        private RandomSeries RandomSeries { get; }
        public RandomMutator(int vectorSize, SeriesType type, int virtualCount, float min, float max, int seed = 0) :
            base(new Store(new RandomSeries(vectorSize, type, virtualCount, min, max, seed)))
        {
            RandomSeries = (RandomSeries)Store.Series;
        }

        public override void Update(float t)
        {
            RandomSeries.Update();
        }

        public override void Reset()
        {
            RandomSeries.Reset();
        }
    }
}
