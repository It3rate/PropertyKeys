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
            Random rnd = new Random();
            int test = 1;
            if(test == 0)
            {
                Vector2[] start = new Vector2[] { new Vector2(20, 70), new Vector2(140, 350), new Vector2(440, 260) };
                Vector2[] end = new Vector2[] { new Vector2(220, 10), new Vector2(340, 250), new Vector2(85, 400) };
                //Vector2[] start = new Vector2[] { new Vector2(100, 100), new Vector2(120, 120), new Vector2(200, 200) };
                //Vector2[] end = new Vector2[] { new Vector2(200, 200), new Vector2(370, 370), new Vector2(400, 400) };
                Location = new ValueKey(start, end)
                {
                    ElementCount = 32,
                    Dimensions = new int[] { 4 }
                };
            }
            else if (test == 1)
            {
                int count = 50;
                List<Vector2> start = new List<Vector2>();
                List<Vector2> end = new List<Vector2>();
                for (int i = 0; i < count; i++)
                {
                    Vector2 v = new Vector2(rnd.Next(500), rnd.Next(300));
                    start.Add(v);
                    end.Add( new Vector2(v.X + rnd.Next(250) - 25, v.Y + rnd.Next(50) - 25));
            }
            Location = new ValueKey(start.ToArray(), end.ToArray())
            {
                ElementCount = 200,
                //Dimensions = new int[] { 15 }
            };
        }
        }

        public void Draw(Graphics g, float t)
        {
            //t = 1f;
            for (int i = 0; i < Location.ElementCount; i++)
            {
                t = t > (int)t ? t - (int)t : t;
                Vector2 v2 = Location.GetVector2AtIndex(i, true, t);
                float it = i / (float)Location.ElementCount;
                Brush b = new SolidBrush(Color.FromArgb(128, 128 - (int)(t*128), (int)(it*255)));
                float r = 10;
                g.FillEllipse(b, v2.X - r, v2.Y - r, r * 2, r * 2);
            }
        }
    }
}
