﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.Samplers;
using Motive.SeriesData.Utils;

namespace Motive.Commands
{
    public class CommandCreateLinkSampler : CommandBase
    {
	    public int SamplerId => Sampler?.Id ?? 0;
	    public readonly LinkSampler Sampler;

	    //private readonly int _linkedCompositeId;
	    //private readonly PropertyId _propertyId;
	    //private readonly Slot[] _swizzleMap;
	    //private readonly int _capacity;


        public CommandCreateLinkSampler(int linkedCompositeId, PropertyId propertyId, Slot[] swizzleMap = null, int capacity = 1)
        {
	        //_linkedCompositeId = linkedCompositeId;
			//_propertyId = propertyId;
		    //_swizzleMap = swizzleMap;
		    //_capacity = capacity;
			Sampler = new LinkSampler(linkedCompositeId, propertyId, swizzleMap, capacity);
        }

        public override void Execute()
        {
	        Runner.CurrentRunner.ActivateComposite(Sampler.Id);
        }

        public override void UnExecute()
        {
	        Runner.CurrentRunner.DeactivateComposite(Sampler.Id);
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
