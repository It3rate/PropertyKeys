using DataArcs.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class CompositeTestObjects
    {
        private Composite GetTest0()
        {

            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            var cols = 10;
            var rows = 10;
            Store items = new IntSeries(1, new int[] { 0, rows * cols - 1 }, virtualCount: rows * cols).Store;
            composite.AddProperty(PropertyId.Items, new BlendStore(items));

            var totalWidth = 500f;
            float growth = 60;
            var armLen = totalWidth / (float)(cols - 1) / 3f;
            var height = armLen * (float)Math.Sqrt(3) / 2f * (rows - 1f);
            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, armLen, armLen, armLen, armLen * 1.5f).Store;

            float[] start = { 150, 150, 150 + totalWidth, 150 + height };
            Sampler hexSampler = new HexagonSampler(new[] { cols, 0 });
            var startStore = new Store(new FloatSeries(2, start, cols * rows), hexSampler);
            float[] end = { start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth };
            var endStore = new Store(new FloatSeries(2, end, rows * cols), hexSampler);

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut2), CombineFunction.Multiply, CombineTarget.T);
            composite.AddProperty(PropertyId.Location, new BlendStore(new IStore[]{startStore, endStore}, easeStore));

            return composite;
        }

        private Composite GetTest1()
        {
            var composite = GetTest0();

            BlendStore loc = (BlendStore)composite.GetStore(PropertyId.Location);
            Series minMax = new FloatSeries(2, -4f, -3f, 4f, 3f);
            var randomStore = new RandomSeries(2, SeriesType.Float, loc.VirtualCount, minMax, 1111, CombineFunction.ContinuousAdd).Store;

            var fs = new FunctionalStore(loc.GetStoreAt(1), randomStore);
            composite.AddProperty(PropertyId.Location, new BlendStore(loc.GetStoreAt(0), fs));
            return composite;
        }

        private Composite GetTest2()
        {
            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, 10f, 15f, 20f, 15f).Store;
            const int count = 50;
            Series maxMinA = new FloatSeries(2, 0, 0, 1000f, 500f);
            Series maxMinB = new FloatSeries(2, 200f, 100f, 600f, 300f);
            var startStore = new RandomSeries(2, SeriesType.Float, count, maxMinA).Store;
            var endStore = new RandomSeries(2, SeriesType.Float, count, maxMinB).Store;

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.SmoothStep4), CombineFunction.Multiply, CombineTarget.T);
            composite.AddProperty(PropertyId.Location, new BlendStore(new IStore[] { startStore, endStore }, easeStore));

            composite.shouldShuffle = true;
            return composite;
        }
        private Composite GetTest3()
        {
            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            IntSeries itemData = new IntSeries(1, new int[] { 0, 149 }, virtualCount: 150);
            itemData = (IntSeries)itemData.HardenToData();
            SeriesUtils.Shuffle(itemData);
            Store items = itemData.Store;
            composite.AddProperty(PropertyId.Items, new BlendStore(new Store[] { items }));

            ((PolyShape)composite.Graphic).PointCount = new float[] { 3f, 5.9f }.ToStore();
            Sampler ringSampler = new RingSampler();
            Sampler gridSampler = new GridSampler(new[] { 15, 0, 0 });
            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, 5f, 5f, 15f, 15f).Store;
            var vectorSize = 2;
            var start = new float[] { 100, 100, 500, 400 };
            var end = new float[] { 100, 100, 500, 400 };
            var startStore = new Store(new FloatSeries(vectorSize, start, 150), ringSampler);
            var endStore = new Store(new FloatSeries(vectorSize, end, 150), gridSampler);

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut2), CombineFunction.Multiply, CombineTarget.T);
            composite.AddProperty(PropertyId.Location, new BlendStore(new IStore[] { startStore, endStore }, easeStore));

            return composite;
        }

        private void AddGraphic(Composite composite)
        {
	        composite.Graphic = new PolyShape(
		        radius: new Store(new float[] { 10f, 20f }),
		        orientation: new Store(new float[] { 1f / 12f, 0.3f }),
		        starness: new Store(new float[] { 0, -0.3f }),
		        pointCount: new FloatSeries(1, new float[] { 6.0f, 10.99f }, 6).Store
	        );
        }

        private void AddColor(Composite composite)
        {
	        Sampler colorSampler = new LineSampler(); //new GridSampler(new []{10, 0});
	        var start = new float[] { 0.3f, 0.1f, 0.2f, 1f, 1f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f };
	        var end = new float[] { 0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f };
	        var colorStartStore = new Store(new FloatSeries(3, start), colorSampler);
	        var colorEndStore = new Store(new FloatSeries(3, end), colorSampler);
	        composite.AddProperty(PropertyId.FillColor, new BlendStore(colorStartStore, colorEndStore));
        }
    }
}
