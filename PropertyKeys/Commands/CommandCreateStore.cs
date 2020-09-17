using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Commands
{
    public class CommandCreateStore : CommandBase
    {
	    private Store Store;

	    private int _seriesId;
	    private int _samplerId;
	    private CombineFunction _combineFunction;

        public CommandCreateStore(Series series, Sampler sampler = null, CombineFunction combineFunction = CombineFunction.Replace)
        {
	        _seriesId = series.Id;
	        sampler = sampler ?? new LineSampler(series.Count);
	        _samplerId = sampler.Id;
	        _combineFunction = combineFunction;
        }

	    public override void Execute()
	    {
		    Store = new Store(_seriesId, _samplerId, _combineFunction);
	    }

	    public override void UnExecute()
	    {
		    throw new NotImplementedException();
	    }

	    public override void Update(double time)
	    {
		    throw new NotImplementedException();
	    }

	    public override void Draw(Graphics graphics)
	    {
		    throw new NotImplementedException();
	    }
    }
}
