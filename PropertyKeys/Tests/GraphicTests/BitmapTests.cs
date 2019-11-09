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
	        int columns = 120;
	        int width = 675;
			var bounds = new RectFSeries(0, 0, width, width * (bmp.Height / (float)bmp.Width));
            //var sampler = new GridSampler(new int[] { w, h });
			var container = HexagonSampler.CreateBestFit(bounds, columns, out int rows, out HexagonSampler sampler);
            var colorStore = new Store(bmp.ToFloatSeriesHex(columns, rows));
            //colorStore.BakeData();
            //SeriesUtils.Shuffle(colorStore.GetFullSeries());

            IStore items = container.GetStore(PropertyId.Items);
			items.BakeData();
			SeriesUtils.Shuffle(items.GetFullSeries());
			//items.ShouldIterpolate = false;
            //Array.Sort(items.GetFullSeries().IntDataRef);
            //Array.Reverse(items.GetFullSeries().IntDataRef);


            container.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.3f, 0.4f, 0.3f, 0.4f, 1f).Store);
            container.AddProperty(PropertyId.FillColor, colorStore);
            return container;
        }
        private static void AddGraphic(Container container, float radius)
        {
        }
    }
}
