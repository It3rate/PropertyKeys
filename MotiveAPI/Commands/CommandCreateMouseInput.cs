using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components.ExternalInput;
using Motive.Samplers;

namespace Motive.Commands
{
    public class CommandCreateMouseInput : CommandBase
    {
        public readonly int ContainerId;
	    public readonly MouseInput Composite;

	    public CommandCreateMouseInput()
	    {
		    Composite = new MouseInput();
		    ContainerId = Composite.Id;
	    }

	    public override void Execute()
	    {
		    Runner.CurrentRunner.ActivateComposite(ContainerId);
	    }

	    public override void UnExecute()
	    {
		    Runner.CurrentRunner.DeactivateComposite(ContainerId);
	    }

	    public override void Update(double time)
	    {
	    }
    }
}
