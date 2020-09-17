using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
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
		    ParametricSeries link = Player.CurrentComposites[LinkedCompositeId]?.GetSampledTs(PropertyId, seriesT);
		    return Swizzle(link, seriesT);
        }
    }
}
