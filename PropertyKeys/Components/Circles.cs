using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Components
{
    public class Circles
    {
        ValueKey Location { get; set; }

        public Circles()
        {
            //Vector2[] start = new Vector2[] { new Vector2(20, 70), new Vector2(40, 150), new Vector2(20, 260) };
            //Vector2[] end = new Vector2[] { new Vector2(220, 10), new Vector2(340, 250), new Vector2(85, 3800) };
            Vector2[] start = new Vector2[] { new Vector2(100, 100), new Vector2(100, 200) };
            Vector2[] end = new Vector2[] { new Vector2(200, 100), new Vector2(200, 200) };
            Location = new ValueKey(start, end);
            Location.ElementCount = 9;
            Location.Dimensions = new int[] { 3 };
        }

        public void Draw(Graphics g, float t)
        {
            //t = 1f;
            for (int i = 0; i < Location.ElementCount; i++)
            {
                t = t > (int)t ? t - (int)t : t;
                Vector2 v2 = Location.GetVector2AtIndex(i, true, t);
                Brush b = new SolidBrush(Color.Red);
                float r = 10;
                g.FillEllipse(b, v2.X - r, v2.Y - r, r * 2, r * 2);
            }
        }
    }
}
