using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Bitmap[] bitmaps;
        private Timer _timer;

        public BitmapTests(Player player)
        {
            _player = player;
            //bitmaps = Resources.face;
            bitmaps = new []{ Resources.face, Resources.face2, Resources.face3};
        }

        private int _step = 0;
        private int _bitmapIndex = 0;

        public void NextVersion()
        {
	        _player.Clear();
            Container image = GetImage(bitmaps[_bitmapIndex]);
            switch (_step)
            {
	            case 0:
		            _player.AddActiveElement(image);
		            _timer = new Timer(0, 1000, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_timer);
                    break;
	            case 1:
		            var comp = GetBlend(image, false);
		            _player.AddActiveElement(comp);
		            comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            break;
	            case 2:
		            var compRev = GetBlend(image, true);
		            _player.AddActiveElement(compRev);
		            compRev.Runner.EndTimedEvent += CompOnEndTransitionEvent;
		            break;
	            case 3:
		            _player.AddActiveElement(image);
		            _timer = new Timer(0, 1000, null);
		            _timer.EndTimedEvent += CompOnEndTransitionEvent;
		            _player.AddActiveElement(_timer);
		            break;
            }

        }

	    private void CompOnEndTransitionEvent(object sender, EventArgs e)
	    {
		    ITimeable anim = (ITimeable)sender;
		    _step++;
		    if (_step > 3)
		    {
			    _step = 0;
				_bitmapIndex = _bitmapIndex < bitmaps.Length - 1 ? _bitmapIndex + 1 : 0;
		    }
            NextVersion();
        }


    public static float RGBXYDistance1(Series series)
        {
	        float[] input = series.GetRawDataAt(0).FloatDataRef;
	        float[] location = series.GetRawDataAt(1).FloatDataRef;
	        var max = input.Max();
	        var min = input.Min();
	        var dif = max - min;
	        var avg = input.Average();
	        float x = location[0];
	        float y = location[1];
	        float t = location[2];
	        return dif;
        }

		private int columns = 120;
	    private int width = 675;
	    private int rows;
	    public Container GetImage(Bitmap bitmap)
	    {
		    var bounds = new RectFSeries(0, 0, width, width * (bitmap.Height / (float)bitmap.Width));
		    //var sampler = new GridSampler(new int[] { w, h });
		    var container = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out HexagonSampler sampler);
		    rows = rowCount;
		    var colorStore = new Store(bitmap.ToFloatSeriesHex(columns, rows));
		    container.AddProperty(PropertyId.FillColor, colorStore);
		    colorStore.BakeData();
		    IStore items = container.GetStore(PropertyId.Items);
		    items.BakeData();
		    return container;
	    }

        public BlendTransition GetBlend(Container container, bool reverse)
        {
            var container2 = container.CreateChild();
            var colorStore2 = container2.GetStore(PropertyId.FillColor).Clone();
            var colors = colorStore2.GetSeriesRef().ToList();
            float countCapacity = columns * rows - 1f;
            int index = 0;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    colors[index].Append(new FloatSeries(3, x / (float)(columns - 1f), y / (float)(rows - 1f), index / countCapacity));
                    index++;
                }
            }

            //colors.Sort((a, b) => (int)(a.RgbToHsl()[2] - b.RgbToHsl()[2]));
            colors.Sort((a, b) => (int)((RGBXYDistance1(a) - RGBXYDistance1(b)) * 10000));

            if (reverse)
            {
	            //colorStore2.GetSeriesRef().SetByList(colors);
	            //container2.AddProperty(PropertyId.FillColor, colorStore2);
            }

            var items2 = container2.GetStore(PropertyId.Items).GetSeriesRef().Copy();
            var cap = items2.Count;
            IntSeries itemsSeries = (IntSeries) items2;

            for (int i = 0; i < cap; i++)
            {
                int idx = (int)(Math.Round(colors[i].GetRawDataAt(1).Z * countCapacity));
                itemsSeries.SetRawDataAt(i, new IntSeries(1, idx));
            }
			
            var orgStore = container.GetStore(PropertyId.Location);
            var locStore = orgStore.Clone();
            locStore.BakeData();
            if (reverse)
            {
				orgStore.BakeData();
                orgStore.GetSeriesRef().MapFromItemToIndex(itemsSeries);
            }
            else
            {
	            locStore.GetSeriesRef().MapFromItemToIndex(itemsSeries);
            }
            
	        container2.AddProperty(PropertyId.Location, locStore);

            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.SmoothStart3));//, new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
			var result = new BlendTransition(container, container2, new Timer(0, 2000), easeStore);
			return result;
        }
        private static void AddGraphic(Container container, float radius)
        {
        }
    }
}
