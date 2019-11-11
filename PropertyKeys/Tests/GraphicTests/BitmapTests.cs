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
        private Bitmap bmp;

        public BitmapTests(Player player)
        {
            _player = player;
            bmp = Resources.face;
        }

        public void NextVersion()
        {
	        Container image = GetImage();
            var comp = GetBlend(image);
            _player.AddActiveElement(comp);

            comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
        }

	    private void CompOnEndTransitionEvent(object sender, EventArgs e)
	    {
		    ITimeable anim = (ITimeable)sender;
            anim.Reverse();
            anim.Restart();
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
	    public Container GetImage()
	    {
		    var bounds = new RectFSeries(0, 0, width, width * (bmp.Height / (float)bmp.Width));
		    //var sampler = new GridSampler(new int[] { w, h });
		    var container = HexagonSampler.CreateBestFit(bounds, columns, out int rowCount, out HexagonSampler sampler);
		    rows = rowCount;
		    var colorStore = new Store(bmp.ToFloatSeriesHex(columns, rows));
		    container.AddProperty(PropertyId.FillColor, colorStore);
		    colorStore.BakeData();
		    IStore items = container.GetStore(PropertyId.Items);
		    items.BakeData();
		    return container;
	    }
        public BlendTransition GetBlend(Container container)
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

            //colorStore2.GetSeriesRef().SetByList(colors);
            //container2.AddProperty(PropertyId.FillColor, colorStore2);

            var items2 = container2.GetStore(PropertyId.Items).GetSeriesRef().Copy();
            //items2.ShouldIterpolate = false;
            var cap = items2.Count;
            IntSeries itemsSeries = (IntSeries) items2;

            //var list = itemsSeries.ToList();
            //list.Reverse();
            //itemsSeries.SetByList(list);

            for (int i = 0; i < cap; i++)
            {
                int idx = (int)(Math.Round(colors[i].GetRawDataAt(1).Z * countCapacity));
                itemsSeries.SetRawDataAt(i, new IntSeries(1, idx));
            }

            //container2.AddProperty(PropertyId.Items, new Store(itemsSeries));

            var orgStore = container2.GetStore(PropertyId.Location);

            var locStore = new Store(orgStore.GetSeriesRef().Copy(), orgStore.Sampler);
            locStore.BakeData();
            locStore.GetSeriesRef().MapFromItemToIndex(itemsSeries);
            //locStore.ShouldIterpolate = false;
            //locStore.GetSeriesRef().MapFromIndexToItem((IntSeries)container2.GetStore(PropertyId.Items).GetSeriesRef()); 
	        container2.AddProperty(PropertyId.Location, locStore);

            //colorStore2.GetSeriesRef().MapFromIndexToItem(itemsSeries);
            //colorStore2.GetSeriesRef().SetByList(colors);
            //container2.AddProperty(PropertyId.FillColor, colorStore2);


            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
			var result = new BlendTransition(container, container2, new Timer(0, 8000), easeStore);
			return result;
        }
        private static void AddGraphic(Container container, float radius)
        {
        }
    }
}
