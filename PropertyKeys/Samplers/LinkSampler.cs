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
	    public override Series GetValueAtIndex(Series series, int index)
	    {
		    var indexT = index / (Capacity - 1f);
            return GetValuesAtT(series, indexT);
	    }

	    public override Series GetValuesAtT(Series series, float t)
	    {
		    ParametricSeries slotMapped = GetSampledTs(new ParametricSeries(1, t));
			float[] floats = new float[slotMapped.Count];
		    for (int i = 0; i < slotMapped.Count; i++)
		    {
			    floats[i] = series.GetValueAtT(slotMapped[i]).FloatDataAt(i);
		    }

		    return SeriesUtils.CreateSeriesOfType(series, floats);
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    ParametricSeries link = _player[LinkedCompositeId]?.GetSampledTs(PropertyId, seriesT);
		    return Swizzle(link);
        }
    }
}
