﻿using System;
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

        private Series Series { get; set; }

        // todo: turn easing into t samplers
        public EasingType[] EasingTypes { get; protected set; } // move to properties? No, useful for creating virtual data.
        protected Sampler Sampler { get; set; }

        public int VirtualCount 
        {
            get => Series.VirtualCount;
            set => Series.VirtualCount = value;
        }

        public Store(EasingType[] easingTypes = null)
        {
            EasingTypes = easingTypes ?? DefaultEasing;
        }
        public Store(Series series, Sampler sampler = null, EasingType[] easingTypes = null)
        {
            Series = series;
            Sampler = sampler ?? new LineSampler();
            EasingTypes = easingTypes ?? DefaultEasing;
        }
        public Store(int[] data, Sampler sampler = null, EasingType[] easingTypes = null) : this(new IntSeries(1, data), sampler, easingTypes) {}
        public Store(float[] data, Sampler sampler = null, EasingType[] easingTypes = null) : this(new FloatSeries(1, data), sampler, easingTypes) {}

        public Series GetValueAtIndex(int index)
        {
            return Sampler != null ? Sampler.GetValueAtIndex(Series, index) : Series.GetValueAtIndex(index);
        }

        public Series GetValueAtT(float t)
        {
            return Sampler != null ? Sampler.GetValueAtT(Series, t) : Series.GetValueAtT(t);
        }

        public void HardenToData()
        {
            Series = Series.HardenToData(this);
            Sampler = null;
            EasingTypes = null;
        }
    }
}
