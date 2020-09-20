using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components.ExternalInput;
using DataArcs.Players;
using DataArcs.Samplers;

namespace DataArcs.Commands
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
		    Player.CurrentPlayer.ActivateComposite(ContainerId);
	    }

	    public override void UnExecute()
	    {
		    Player.CurrentPlayer.DeactivateComposite(ContainerId);
	    }

	    public override void Update(double time)
	    {
	    }
    }
}
