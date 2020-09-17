using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Stores;

namespace DataArcs.Commands
{
    public class CommandCreateContainer : CommandBase
    {
        private Container _container;
		
        private readonly IStore _items;
        private readonly IContainer _parent;
        private readonly IRenderable _renderer;

        public CommandCreateContainer(IStore items, IContainer parent, IRenderable renderer)
        {
	        _items = items;
	        _parent = parent;
	        _renderer = renderer;
        }

	    public override void Execute()
	    {
			_container = new Container(_items, _parent, _renderer);
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
