using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Properties;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;
using DataArcs.Components.ExternalInput;

namespace DataArcs.Tests.GraphicTests
{
    public class BitmapTests : ITestScreen
    {
        private readonly Player _player;
        private Bitmap bmp;

        public BitmapTests(Player player)
        {
            _player = player;
            bmp = Resources.it3rate;
        }

        public void NextVersion()
        {
            var comp = GetComposite0();
            _player.AddActiveElement(comp);

            //comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
        }

        //private void CompOnEndTransitionEvent(object sender, EventArgs e)
        //{
        //    ITimeable anim = (ITimeable)sender;
        //}
        public Container GetComposite0()
        {
	        int w = 100;
	        int h = (int)(w * (bmp.Height / bmp.Width));
	        float r = 400f / w;

            var composite = new Container(Store.CreateItemStore(w * h));

            var sampler = new GridSampler(new int[] { w, h });
            //var sampler = new HexagonSampler(new int[] { w, h });
            Store loc = new Store(new RectFSeries(20, 20, w * r, h * r), sampler);
            composite.AddProperty(PropertyId.Location, loc);

            composite.AddProperty(PropertyId.FillColor, bmp.ToFloatSeries(w, h).Store);
            AddGraphic(composite, r);
            return composite;
        }
        private static void AddGraphic(Container container, float radius)
        {
            container.AddProperty(PropertyId.Radius, new FloatSeries(2, radius, radius).Store);
            container.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
            //container.AddProperty(PropertyId.PenColor, Colors.White.Store);
            //container.AddProperty(PropertyId.PenWidth, new IntSeries(1, 1).Store);
            container.Renderer = new PolyShape();
        }
    }
}
