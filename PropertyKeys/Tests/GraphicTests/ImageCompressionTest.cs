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
using DataArcs.Components.ExternalInput;
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
	    private MouseInput _mouseInput;
	    private int _bitmapIndex;

        public ImageCompressionTest(Player player)
	    {
		    _player = player;
		    bitmaps = new[] { Resources.face, Resources.face2, Resources.face3 };
			NextVersion();
        }

        public void NextBlock()
        {
	        _bitmapIndex++;
	        if (_bitmapIndex >= bitmaps.Length)
	        {
		        _bitmapIndex = 0;
	        }
			NextVersion();
        }

        public void NextVersion()
	    {
			_player.Clear();
		    _mouseInput = new MouseInput();
		    _mouseInput.MouseClick = NextBlock;
		    _player.AddActiveElement(_mouseInput);

		    var bmp = bitmaps[_bitmapIndex];

		    //SampleImage(bmp);
		    AddQuadImage(bmp);
        }

        private void SampleImage(Bitmap bitmap)
        {
	        var comp = GetImage(bitmap, 10);
			_player.AddActiveElement(comp);
        }

        private Container GetImage(Bitmap bitmap, int columns)
	    {
		    int width = 675;
		    int rows;
		    var bounds = new RectFSeries(0, 0, width, width * (bitmap.Height / (float)bitmap.Width));
		    //var sampler = new GridSampler(new int[] { w, h });
		    var container = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out float radius, out HexagonSampler sampler);
		    rows = rowCount;

		    var colorStore = new Store(bitmap.ToFloatSeriesHex(columns, rows));
		    container.AddProperty(PropertyId.FillColor, colorStore);

		    return container;
	    }



        private void AddQuadImage(Bitmap bitmap)
        {
	        int root = 12;
	        IComposite comp0 = GetImage(bitmap, root * 2);
	        RemoveQuadrant(comp0, 0);
	        _player.AddActiveElement(comp0);

	        IComposite comp1 = GetImage(bitmap, root * 3);
	        RemoveQuadrant(comp1, 1);
	        _player.AddActiveElement(comp1);

	        IComposite comp2 = GetImage(bitmap, root * 6);
	        RemoveQuadrant(comp2, 2);
	        _player.AddActiveElement(comp2);

	        IComposite comp3 = GetImage(bitmap, root * 12);
	        RemoveQuadrant(comp3, 3);
	        _player.AddActiveElement(comp3);

        }

        private void RemoveQuadrant(IComposite container, int maskedQuadrant)
        {
	        if (maskedQuadrant >= 0)
	        {
		        var locSampler = container.GetStore(PropertyId.Location).Sampler;
		        int columns = locSampler.Strides[0];
		        int rows = locSampler.Strides[1];
                IStore items = container.GetStore(PropertyId.Items);
		        items.BakeData();
		        var newItems = new int[(int)(items.Capacity)];
		        int index = 0;
		        int loc = 0;
		        float adj = maskedQuadrant == 1 || maskedQuadrant == 3 ? 0.1f : 0;
		        for (int y = 0; y < rows; y++)
		        {
			        for (int x = 0; x < columns; x++)
			        {
				        int quad = (x <= columns * (0.4f - adj) || x >= columns * (.6f + adj)) ? 0 : 1;
				        quad += (y <= rows * (0.2f - adj) || y > rows * (0.6f + adj)) ? 0 : 2;
				        if (quad == maskedQuadrant && index <= newItems.Length)
				        {
					        newItems[index] = loc;
					        index++;
				        }
				        loc++;
			        }
		        }
		        container.AddProperty(PropertyId.Items, new IntSeries(1, newItems).Store);
	        }
        }
    }
}
