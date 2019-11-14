using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.Players;
using DataArcs.Properties;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;
using DataArcs.Components.ExternalInput;
using Timer = DataArcs.Components.Transitions.Timer;

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
            bitmaps = new[] { Resources.face, Resources.face2, Resources.face3 };
        }

        private int _step = 0;
        private int _bitmapIndex = 0;

        public void NextVersion()
        {
            _player.Clear();
            Container containerA = GetImage(bitmaps[_bitmapIndex]);
            IContainer containerB;
            switch (_step)
            {
                case 0:
                    _player.AddActiveElement(containerA);
                    _timer = new Timer(0, 500, null);
                    _timer.EndTimedEvent += CompOnEndTransitionEvent;
                    _player.AddActiveElement(_timer);
                    break;
                case 1:
                    containerB = GetHistogram(containerA, false, true, false);
                    var comp = GetBlend(containerA, containerB, 1000);
                    _player.AddActiveElement(comp);
                    comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
                    break;
                case 2:
                    containerB = GetHistogram(containerA, true, true, false);
                    var compRev = GetBlend(containerA, containerB,1000);
                    _player.AddActiveElement(compRev);
                    compRev.Runner.EndTimedEvent += CompOnEndTransitionEvent;
                    break;
                case 3:
                    _player.AddActiveElement(containerA);
                    _timer = new Timer(0, 500, null);
                    _timer.EndTimedEvent += CompOnEndTransitionEvent;
                    _player.AddActiveElement(_timer);
                    break;
                case 4:
	                int nextImageIndex = _bitmapIndex < bitmaps.Length - 1 ? _bitmapIndex + 1 : 0;
	                var compBlend = BlendImages(_bitmapIndex, nextImageIndex);

					//var image2 = GetImage(bitmaps[nextImageIndex]);
     //               var compBlend = GetBlend(containerA, image2);

	                _player.AddActiveElement(compBlend);
	                compBlend.Runner.EndTimedEvent += CompOnEndTransitionEvent;
	                break;
            }

        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            ITimeable anim = (ITimeable)sender;
            _step++;
            if (_step > 4)
            {
                _step = 0;
                _bitmapIndex = _bitmapIndex < bitmaps.Length - 1 ? _bitmapIndex + 1 : 0;
            }
            NextVersion();
        }

        private int cl;
        private int rw;
        private BlendTransition BlendImages(int indexA, int indexB)
        {
	        IContainer containerA = GetImage(bitmaps[indexA]);
	        IContainer containerB = GetImage(bitmaps[indexB]);
	        var indexesA = GetHistogramIndexes(containerA);
	        var indexesB = GetHistogramIndexes(containerB);

            // todo: blend locations A -> AHist -> invBHist  ---- ColorB -> invLocs?
            var colsA = containerA.GetStore(PropertyId.FillColor);
            colsA.BakeData();
            var items = GetHistogramIndexes(containerA);

            var colsB = containerB.GetStore(PropertyId.FillColor);
            colsB.BakeData();
            var attrB = AppendLocationToColors(colsB, items);
            attrB.Sort((a, b) => (int)((RGBXYDistance1(a) - RGBXYDistance1(b)) * 10000));
            IntSeries sortedB = SetItemsFromAttributedColorOrder(attrB);

            var locsB = containerB.GetStore(PropertyId.Location);
            locsB.BakeData();
            locsB.GetSeriesRef().MapFromItemToIndex(sortedB);

            colsB.GetSeriesRef().MapFromItemToIndex(sortedB);
	        return GetBlend(containerA, containerB, 6000);
        }

        private List<Series> AppendLocationToColors(IStore colorStore, IntSeries itemSeries)
        {
            float cap = itemSeries.Count - 1f;
            var colors = colorStore.GetSeriesRef().ToList();
            for (int i = 0; i < itemSeries.Count; i++)
            {
                colors[i].Append(new FloatSeries(3, 0, 0, itemSeries.FloatDataAt(i)/ cap));
            }
            return colors;
        }
        private IntSeries GetHistogramIndexes(IContainer container)
        {
	        var colorStore2 = container.GetStore(PropertyId.FillColor).Clone();
	        var attributedColors = AppendLocationToColors(colorStore2, cl, rw);
	        attributedColors.Sort((a, b) => (int)((RGBXYDistance1(a) - RGBXYDistance1(b)) * 10000));
	        IntSeries itemsSeries = SetItemsFromAttributedColorOrder(attributedColors);
	        return itemsSeries;
        }

        public Container GetImage(Bitmap bitmap)
        {
            int columns = 100;
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
            cl = columns;
            rw = rows;
            return container;
        }


        private List<Series> AppendLocationToColors(IStore colorStore, int columns, int rows)
        {
	        var colors = colorStore.GetSeriesRef().ToList();
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
	        return colors;
        }

        private IntSeries SetItemsFromAttributedColorOrder(List<Series> attributedColors)
        {
	        int len = attributedColors.Count;
            var result = new IntSeries(1, new int[len]);
	        for (int i = 0; i < len; i++)
	        {
		        int idx = (int)(Math.Round(attributedColors[i].GetRawDataAt(1).Z * (attributedColors.Count - 1f)));
		        result.SetRawDataAt(i, new IntSeries(1, idx));
	        }
	        return result;
        }

        public IContainer GetHistogram(Container imageContainer, bool reverse, bool adjustLocations, bool adjustColors)
        {
	        var result = imageContainer.CreateChild();

	        int[] strides = imageContainer.GetStore(PropertyId.Location).Sampler.Strides;
	        int columns = strides[0];
	        int rows = strides[1];

	        var colorStore2 = result.GetStore(PropertyId.FillColor).Clone();

	        var attributedColors = AppendLocationToColors(colorStore2, columns, rows);
	        //colors.Sort((a, b) => (int)(a.RgbToHsl()[2] - b.RgbToHsl()[2]));
	        attributedColors.Sort((a, b) => (int)((RGBXYDistance1(a) - RGBXYDistance1(b)) * 10000));


	        IntSeries itemsSeries = SetItemsFromAttributedColorOrder(attributedColors);

	        if (adjustLocations)
	        {
		        var orgLocStore = imageContainer.GetStore(PropertyId.Location);
		        var newLocStore = orgLocStore.Clone();
		        newLocStore.BakeData();
		        if (reverse)
		        {
			        orgLocStore.BakeData();
			        orgLocStore.GetSeriesRef().MapFromItemToIndex(itemsSeries);
		        }
		        else
		        {
			        newLocStore.GetSeriesRef().MapFromItemToIndex(itemsSeries);
		        }

		        result.AddProperty(PropertyId.Location, newLocStore);
	        }

	        if (adjustColors)
	        {
		        if (reverse)
		        {
			        var colorStore = imageContainer.GetStore(PropertyId.FillColor);
			        colorStore.GetSeriesRef().SetByList(attributedColors);
		        }
		        else
		        {
					colorStore2.GetSeriesRef().SetByList(attributedColors);
					result.AddProperty(PropertyId.FillColor, colorStore2);
		        }
	        }

	        return result;
        }

        public BlendTransition GetBlend(IContainer containerA, IContainer containerB, int ms)
        {
	        Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.SmoothStart3));//, new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            var result = new BlendTransition(containerA, containerB, new Timer(0, ms), easeStore);
            return result;
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

    }
}
