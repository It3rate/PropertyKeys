using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Stores
{
    public class SampledSeries
    {
        protected static Random Rnd = new Random();
        protected static readonly EasingType[] DefaultEasing = new EasingType[] { EasingType.Linear };
        protected static readonly int[] DefaultStrides = new int[] { 0 }; // zero means repeating, so this is a regular one row array

        public Series Series { get; }
        public EasingType[] EasingTypes { get; } // move to properties? No, useful for creating virtual data.
        protected BaseSampler Sampler { get; }

        public GraphicsPath GetPath(){return null;} // todo: move to static method on Bezier store
        public int[] Strides { get; set; } // todo: move to grid/hex samplers


        public SampledSeries(Series series, BaseSampler sampler, EasingType[] easingTypes = null)
        {
            Series = series;
            Sampler = sampler;
            EasingTypes = easingTypes ?? DefaultEasing;
        }

        public Series GetValueAtIndex(int index)
        {
            return Sampler != null ? Sampler.GetValueAtIndex(Series, index) : Series.GetValueAtIndex(index);
        }

        public Series GetValueAtT(float t)
        {
            return Sampler != null ? Sampler.GetValueAtT(Series, t) : Series.GetValueAtT(t);
        }

        // these should be Vector GetValueAtIndex/T - Vector being convertible to float or int arrays.
        //public abstract float[] GetFloatArrayAtIndex(int index);
        //public abstract float[] GetFloatArrayAtT(float t);
        //public abstract int[] GetIntArrayAtIndex(int index);
        //public abstract int[] GetIntArrayAtT(float t);

        //public abstract float[] GetInterpolatededValueAtT(float t);
        public void HardenToData() { }

    }
}
