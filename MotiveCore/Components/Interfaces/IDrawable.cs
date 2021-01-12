using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.Stores;
using MotiveCore.Graphic;
using MotiveCore.SeriesData;

namespace MotiveCore.Components
{
	public interface IDrawable
    {
        IRenderable Renderer { get; set; }

        void Draw(Graphics g, Dictionary<PropertyId, Series> dict);
    }
}
