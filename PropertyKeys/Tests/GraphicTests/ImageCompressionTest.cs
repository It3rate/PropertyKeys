using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Players;
using DataArcs.Properties;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class ImageCompressionTest : ITestScreen
    {
	    private readonly Player _player;
	    private Bitmap[] bitmaps;

        public ImageCompressionTest(Player player)
	    {
		    _player = player;
	    }

	    public void NextVersion()
	    {
		    int root = 12;
		    bitmaps = new[] { Resources.face, Resources.face2, Resources.face3 };
		    var bmp = bitmaps[1];
            IComposite comp0 = GetImage(bmp, root*2, 0);
            _player.AddActiveElement(comp0);
            IComposite comp1 = GetImage(bmp, root * 3, 2);
            _player.AddActiveElement(comp1);
            IComposite comp2 = GetImage(bmp, root * 6, 1);
            _player.AddActiveElement(comp2);
            IComposite comp3 = GetImage(bmp, root * 12, 3);
            _player.AddActiveElement(comp3);

            //comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
        }

        //private void CompOnEndTransitionEvent(object sender, EventArgs e)
        //{
        // ITimeable anim = (ITimeable)sender;
        //   }

        public Container GetImage(Bitmap bitmap, int columns, int maskedQuadrant)
	    {
		    int width = 675;
		    int rows;
		    var bounds = new RectFSeries(0, 0, width, width * (bitmap.Height / (float)bitmap.Width));
		    //var sampler = new GridSampler(new int[] { w, h });
		    var container = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out HexagonSampler sampler);
		    rows = rowCount;

		    var colorStore = new Store(bitmap.ToFloatSeriesHex(columns, rows));
		    container.AddProperty(PropertyId.FillColor, colorStore);
		    colorStore.BakeData();

		    IStore items = container.GetStore(PropertyId.Items);
		    items.BakeData();
		    var newItems = new int[(int)(items.Capacity)];
		    int index = 0;
		    int loc = 0;
		    float adjX = maskedQuadrant == 1 || maskedQuadrant == 3 ? 0.1f : 0;
		    for (int y = 0; y < rows; y++)
		    {
			    for (int x = 0; x < columns; x++)
			    {
				    int quad = (x <= columns * (0.4f - adjX) || x >= columns * (.6f + adjX)) ? 0 : 1;
				    quad += (y <= rows * .2f || y > rows * .6f) ? 0 : 2;
				    if (quad == maskedQuadrant && index <= newItems.Length)
				    {
					    newItems[index] = loc;
					    index++;
				    }
				    loc++;
			    }
		    }
            container.AddProperty(PropertyId.Items, new IntSeries(1, newItems).Store);
			//items.SetFullSeries(new IntSeries(1, newItems));
		 //   IStore rads = container.GetStore(PropertyId.Radius);
			//rads.BakeData();


		    return container;
	    }
    }
}
