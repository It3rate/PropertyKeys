using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers
{
    public class LinkSampler : Sampler
    {
	    public int LinkedCompositeId { get; }
	    public PropertyId PropertyId { get; }

        public LinkSampler(int compositeId, PropertyId propertyId, Slot[] swizzleMap = null, int capacity = 1) : base(swizzleMap, 1)
        {
	        LinkedCompositeId = compositeId;
	        PropertyId = propertyId;
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    ParametricSeries link = Runner.CurrentComposites[LinkedCompositeId]?.GetNormalizedPropertyAtT(PropertyId, seriesT);
		    return Swizzle(link, seriesT);
        }
    }
}
