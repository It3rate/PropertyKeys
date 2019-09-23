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
using DataArcs.Transitions;

namespace DataArcs.Tests.GraphicTests
{
    public class CompositeTestObjects
    {
	    public const int VersionCount = 4;
        public static CompositeBase GetTest0(float delay, float startTime, float duration)
        {
	        var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            var cols = 15;
            var rows = 10;
            Store items = new IntSeries(1, new int[] { 0, rows * cols - 1 }).Store;
            items.VirtualCount = rows * cols;
            composite.AddProperty(PropertyId.Items, new BlendStore(items));

            var totalWidth = 500f;
            float growth = 60;
            var armLen = totalWidth / (float)(cols - 1) / 3f;
            var height = armLen * (float)Math.Sqrt(3) / 2f * (rows - 1f);
            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, armLen, armLen, armLen, armLen * 1.5f).Store;

            float[] start = { 150, 150, 150 + totalWidth, 150 + height };
            Sampler hexSampler = new HexagonSampler(new[] { cols, rows });
            var startStore = new Store(new FloatSeries(2, start), hexSampler);
            startStore.VirtualCount = rows * cols;

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            composite.AddProperty(PropertyId.Location, startStore);


            Composite endComp = (Composite)composite.CreateChild();
            float[] end = { start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth };
            endComp.AddProperty(PropertyId.Location, new Store(new FloatSeries(2, end), hexSampler, CombineFunction.Replace, virtualCount: rows * cols));

            return new BlendTransition(composite, endComp, delay, startTime, duration, easeStore);
        }

        public static CompositeBase GetTest1(float delay, float startTime, float duration)
        {
	        BlendTransition bt = (BlendTransition)GetTest0(delay, startTime, duration);

            IStore endLocStore = bt.End.GetStore(PropertyId.Location);
            Series minMax = new FloatSeries(2, -4f, -3f, 4f, 3f);
            var randomStore = new RandomSeries(2, SeriesType.Float, endLocStore.VirtualCount, minMax, 1111, CombineFunction.ContinuousAdd).Store;
            randomStore.VirtualCount = endLocStore.VirtualCount;
            var fs = new FunctionalStore(endLocStore, randomStore);
            ((Composite)bt.End).AddProperty(PropertyId.Location, fs);
            return bt;
        }

        public static CompositeBase GetTest2(float delay, float startTime, float duration)
        {
            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, 10f, 15f, 20f, 15f).Store;
            const int count = 50;
            Series maxMinA = new FloatSeries(2, 0, 0, 1000f, 500f);
            Series maxMinB = new FloatSeries(2, 200f, 100f, 600f, 300f);
            var startStore = new RandomSeries(2, SeriesType.Float, count, maxMinA).Store;
            composite.AddProperty(PropertyId.Location, startStore);

            Composite endComp = (Composite)composite.CreateChild();
            var endStore = new Store(new RandomSeries(2, SeriesType.Float, count, maxMinB, 0, CombineFunction.Replace));
            endComp.AddProperty(PropertyId.Location, endStore);

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);

            composite.shouldShuffle = true;
            return new BlendTransition(composite, endComp, delay, startTime, duration, easeStore);

        }
        public static CompositeBase GetTest3(float delay, float startTime, float duration)
        {
            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            IntSeries itemData = new IntSeries(1, new int[] { 0, 149 });
            Store items = new Store(itemData, virtualCount: 150);
            items.HardenToData();
            
            SeriesUtils.Shuffle(items.GetFullSeries(0));
            composite.AddProperty(PropertyId.Items, new BlendStore(new Store[] { items }));

            float[] pointArray = new float[] { 15f, 5f, 5f, 15f };
            ((PolyShape)composite.Graphic).PointCount = new Store(pointArray, new Easing(EasingType.EaseInOut));

            Sampler ringSampler = new RingSampler(new int[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 });
            //Sampler ringSampler = new RingSampler(new int[] { 30, 20, 15, 15, 15, 15, 15, 10, 10, 5 });
            //Sampler ringSampler = new RingSampler(new int[] { 60, 50, 40 });
            Sampler gridSampler = new GridSampler(new[] { 15, 10});
            ((PolyShape)composite.Graphic).Radius = new FloatSeries(2, 6f, 6f, 15f, 15f, 6f, 6f).Store;
            var vectorSize = 2;
            var start = new float[] { 100, 100, 500, 400 };
            var end = new float[] { 100, 100, 500, 400 };
            var startStore = new Store(new FloatSeries(vectorSize, start), ringSampler, virtualCount:150);

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            composite.AddProperty(PropertyId.Location, startStore);

            Composite endComp = (Composite)composite.CreateChild();
            var endStore = new Store(new FloatSeries(vectorSize, end), gridSampler, CombineFunction.Replace, virtualCount: 150);
            endComp.AddProperty(PropertyId.Location, endStore);

            return new BlendTransition(composite, endComp, delay, startTime, duration, easeStore);
        }

        private static void AddGraphic(Composite composite)
        {
            composite.Graphic = new PolyShape(
                radius: new Store(new float[] { 10f, 20f, 10f }),
                orientation: new Store(new float[] { 0.3f, 1f / 12f, 0.3f }),
                starness: new Store(new float[] { 0, -0.3f, 0 }),
                pointCount: new Store(new FloatSeries(1, new float[] { 6.0f, 10.99f }), virtualCount: 6)
	        );
        }

        private static void AddColor(Composite composite)
        {
	        Sampler colorSampler = new LineSampler(); //new GridSampler(new []{10, 10});
	        var start = new float[] { 0.3f, 0.1f, 0.2f, 1f, 1f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f };
	        var end = new float[] { 0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f };
	        var colorStartStore = new Store(new FloatSeries(3, start), colorSampler);
	        var colorEndStore = new Store(new FloatSeries(3, end), colorSampler);
	        composite.AddProperty(PropertyId.FillColor, new BlendStore(colorStartStore, colorEndStore));
        }
    }
}
