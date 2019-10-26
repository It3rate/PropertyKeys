using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Players;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
    public class LinkSampler : Sampler
    {
	    public int LinkedCompositeId { get; }
	    public PropertyId PropertyId { get; }
	    private Player _player;

        public LinkSampler(int compositeId, PropertyId propertyId, Slot[] swizzleMap = null, int capacity = 1) : base(swizzleMap, 1)
        {
	        LinkedCompositeId = compositeId;
	        PropertyId = propertyId;
	        _player = Player.GetPlayerById(0);
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    ParametricSeries link = _player[LinkedCompositeId]?.GetSampledTs(PropertyId, seriesT);
		    return Swizzle(link);
        }
    }
}
