﻿using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class NearestSampler : BaseSampler
    {
        public override float[] GetFloatSample(Store valueStore, int index)
        {
            index = Math.Max(0, Math.Min(valueStore.InternalDataCount - 1, index));
            return valueStore.GetFloatArrayAtIndex(index);
        }
        public override float[] GetFloatSample(Store valueStore, float t)
        {
            int index = (int)Math.Round(t * valueStore.InternalDataCount);
            return GetFloatSample(valueStore, index);
        }
    }
}
