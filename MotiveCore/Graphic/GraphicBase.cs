using System.Collections.Generic;
using System.Drawing;
using Motive.Components;
using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Graphic
{
	public abstract class GraphicBase : IRenderable
	{
		public string Name { get; set; }
		public int Id { get; private set; }

		public abstract void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);
		public abstract IDrawableSeries GetDrawable(Dictionary<PropertyId, Series> dict);

        protected GraphicBase()
        {
	        Runner.CurrentRenderables.AddToLibrary(this);
        }

        public bool AssignIdIfUnset(int id)
        {
	        bool result = false;
	        if (Id == 0 && id > 0)
	        {
		        Id = id;
		        result = true;
	        }
	        return result;
        }
        public void OnActivate()
        {
        }

        public void OnDeactivate()
        {
        }

        public void Update(double currentTime, double deltaTime)
        {
        }
    }
}