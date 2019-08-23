using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Graphic
{
    public abstract class BaseGraphic
    {
        public const float M_PIx2 = (float)(Math.PI * 2);

        public abstract void Draw(Graphics g, Brush brush, Pen pen, float t);
    }
}
