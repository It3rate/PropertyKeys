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
        private Container _container;
		
        private readonly IStore _items;
        private readonly IContainer _parent;
        private readonly IRenderable _renderer;
        private readonly Dictionary<PropertyId, int> _properties;

        public CommandCreateContainer(IStore items, IContainer parent, IRenderable renderer, Dictionary<PropertyId, int> properties)
        {
	        _items = items;
	        _parent = parent;
	        _renderer = renderer;
	        _properties = properties;
        }

	    public override void Execute()
	    {
			_container = new Container(_items, _parent, _renderer);
			foreach (var propId in _properties.Keys)
			{
				_container.AddProperty(propId, _properties[propId]);
			}
			Player.CurrentPlayer.ActivateComposite(_container.Id);
	    }

	    public override void UnExecute()
	    {
		    throw new NotImplementedException();
	    }

	    public override void Update(double time)
	    {
		    throw new NotImplementedException();
	    }
    }
}
