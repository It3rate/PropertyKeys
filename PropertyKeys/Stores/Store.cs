using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
    public class Store
    {
        protected static readonly EasingType[] DefaultEasing = { EasingType.Linear };

        public virtual Series Series { get; set; }
        protected Sampler Sampler { get; set; }
        public CombineFunction CombineFunction { get; set; }
        public EasingType[] EasingTypes { get; protected set; } // move to properties? May be useful for creating virtual data. Change to t sampler.
        public int VirtualCount 
        {
            get => Series.VirtualCount;
            set => Series.VirtualCount = value;
        }

        protected Store(EasingType[] easingTypes = null, CombineFunction combineFunction = CombineFunction.Add) { }
        public Store(Series series, Sampler sampler = null, EasingType[] easingTypes = null, CombineFunction combineFunction = CombineFunction.Add)
        {
            Series = series;
            Sampler = sampler ?? new LineSampler();
            EasingTypes = easingTypes ?? DefaultEasing;
            CombineFunction = combineFunction;
        }
        public Store(int[] data, Sampler sampler = null, EasingType[] easingTypes = null, CombineFunction combineFunction = CombineFunction.Add) : this(new IntSeries(1, data), sampler, easingTypes, combineFunction) {}
        public Store(float[] data, Sampler sampler = null, EasingType[] easingTypes = null, CombineFunction combineFunction = CombineFunction.Add) : this(new FloatSeries(1, data), sampler, easingTypes, combineFunction) {}

        public virtual void Reset()
        {
            Series.Reset();
        }
        public virtual void Update(float time)
        {
            Series.Update(time);
        }
        public virtual void HardenToData()
        {
            Series = Series.HardenToData(this);
            Sampler = null;
            EasingTypes = null;
        }

        public virtual Series GetValueAtIndex(int index, int virtualCount = -1)
        {
            return Sampler != null ? Sampler.GetValueAtIndex(Series, index, virtualCount) : Series.GetSeriesAtIndex(index);
        }
        public virtual Series GetValueAtT(float t, int virtualCount = -1)
        {
            return Sampler?.GetValueAtT(Series, t, virtualCount) ?? Series.GetValueAtT(t);
        }
        public virtual float GetTatT(float t)
        {
            return Sampler?.GetTAtT(t) ?? Series.GetValueAtT(t)[0];
        }

    }
}
