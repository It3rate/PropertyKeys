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
		public int Id { get; set; }

		public abstract void DrawWithProperties(Dictionary<PropertyId, ISeries> dict, Graphics g);
		public abstract IDrawableSeries GetDrawable(Dictionary<PropertyId, ISeries> dict);

        protected GraphicBase()
        {
	        Runner.CurrentRenderables.AddToLibrary(this);
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