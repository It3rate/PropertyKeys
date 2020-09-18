using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Stores;

namespace DataArcs.Commands
{
    public class CommandCreateContainer : CommandBase
    {
	    private readonly int _containerId;

	    //private readonly int _parentId;
	    //private readonly int _rendererId;
	    //private readonly Dictionary<PropertyId, int> _properties;
	    
        public CommandCreateContainer(IStore items, IContainer parent, IRenderable renderer, Dictionary<PropertyId, int> properties)
        {
	        var itemStoreId = items?.Id ?? 0;
	        var parentId = parent?.Id ?? 0;
	        var rendererId = renderer?.Id ?? 0;
	        //_properties = properties;

			// May need to create and destroy container each time to allow rewind edits that result in different object counts?
			// Probably need to recreate the whole command though.
			var container = new Container(itemStoreId, parentId, rendererId);
			foreach (var propId in properties.Keys)
			{
				container.AddProperty(propId, properties[propId]);
			}

			_containerId = container.Id;
        }

	    public override void Execute()
	    {
			Player.CurrentPlayer.ActivateComposite(_containerId);
	    }

	    public override void UnExecute()
	    {
		    Player.CurrentPlayer.DeactivateComposite(_containerId);
	    }

	    public override void Update(double time)
	    {
	    }
    }
}
