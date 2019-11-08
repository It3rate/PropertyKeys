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
            bmp = Resources.face;
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
	        int columns = 100;
	        int width = 620;
			var bounds = new RectFSeries(20, 20, width, width * (bmp.Height / (float)bmp.Width));
            //var sampler = new GridSampler(new int[] { w, h });
			var container = HexagonSampler.CreateBestFit(bounds, columns, out int rows);
			container.AddProperty(PropertyId.FillColor, bmp.ToFloatSeries(columns, rows).Store);
            return container;
        }
        private static void AddGraphic(Container container, float radius)
        {
        }
    }
}
