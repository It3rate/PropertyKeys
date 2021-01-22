using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.Components.ExternalInput;
using Motive.Samplers.Utils;
using Motive.SeriesData;
using Motive.Stores;
using Motive.Components.Transitions;
using Motive.SeriesData.Utils;
using MotiveScratch.Properties;

namespace Motive.Tests.GraphicTests
{
    public class ImageCompressionTest : ITestScreen
    {
	    private readonly Runner _runner;
	    private Bitmap[] bitmaps;
	    private MouseInput _mouseInput;
	    private int _bitmapIndex;

        public ImageCompressionTest(Runner runner)
	    {
		    _runner = runner;
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
			_runner.Clear();
		    _mouseInput = new MouseInput();
		    _mouseInput.MouseClick = NextBlock;
		    _runner.ActivateComposite(_mouseInput.Id);

		    var bmp = bitmaps[_bitmapIndex];

		    //SampleImage(bmp);
		    AddQuadImage(bmp);
        }

        private void SampleImage(Bitmap bitmap)
        {
	        var comp = GetImage(bitmap, 10);
			_runner.ActivateComposite(comp.Id);
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
	        _runner.ActivateComposite(comp0.Id);

	        IComposite comp1 = GetImage(bitmap, root * 3);
	        RemoveQuadrant(comp1, 1);
	        _runner.ActivateComposite(comp1.Id);

	        IComposite comp2 = GetImage(bitmap, root * 6);
	        RemoveQuadrant(comp2, 2);
	        _runner.ActivateComposite(comp2.Id);

	        IComposite comp3 = GetImage(bitmap, root * 12);
	        RemoveQuadrant(comp3, 3);
	        _runner.ActivateComposite(comp3.Id);

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
		        container.AddProperty(PropertyId.Items, new IntSeries(1, newItems).Store());
	        }
        }
    }
}
