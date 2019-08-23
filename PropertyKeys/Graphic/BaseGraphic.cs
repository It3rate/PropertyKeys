using System;
using System.Drawing;

namespace PropertyKeys.Graphic
{
    public abstract class BaseGraphic
    {
        public const float M_PIx2 = (float)(Math.PI * 2);

        public abstract void Draw(Graphics g, Brush brush, Pen pen, float t);
    }
}
