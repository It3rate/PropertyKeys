using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Graphic;
using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Components
{
	public interface IDrawable
    {
        IRenderable Renderer { get; set; }

        void Draw(Graphics g, Dictionary<PropertyId, Series> dict);
    }
}
