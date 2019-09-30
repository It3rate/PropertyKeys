using DataArcs.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;
using DataArcs.Components.Transitions;

namespace DataArcs.Tests.GraphicTests
{
	public interface ITestScreen
	{
		void NextVersion();
	}

    public class CompositeTestObjects : ITestScreen
    {
	    public const int VersionCount = 4;
        private int _version = -1;
        private int _count = 0;
        private readonly Player _player;

        public CompositeTestObjects(Player player)
        {
	        _player = player;
        }

        public void NextVersion()
        {
            _version = NextVersionIndex();
            BlendTransition comp = GetVersion(_version);
            _player.AddActiveElement(comp);
        }

        private int NextVersionIndex()
        {
            int result = _version + 1;
            if (result >= VersionCount)
            {
                result = 0;
            }

            return result;
        }

        private BlendTransition GetVersion(int index)
        {
            _player.Clear();

            BlendTransition comp;
            switch (index)
            {
                case 0:
                    comp = GetTest3(0, _player.CurrentMs, 1000f);
                    break;
                case 1:
                    comp = GetTest0(0, _player.CurrentMs, 1000f);
                    break;
                case 2:
                    comp = GetTest2(0, _player.CurrentMs, 1000f);
                    break;
                default:
                    comp = GetTest1(0, _player.CurrentMs, 1000f);
                    break;
            }
            comp.EndTransitionEvent += CompOnEndTransitionEvent;
            return comp;
        }

        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            _count++;
            if (_count < 3)
            {
                BlendTransition bt = (BlendTransition)sender;
                bt.Reverse();
                bt.Restart();
            }
            else if (_count < 4)
            {
                BlendTransition bt = (BlendTransition)sender;
                BlendTransition nextComp = GetVersion(NextVersionIndex());

                var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3), CombineFunction.Multiply, CombineTarget.T);
                //BlendTransition newBT = new BlendTransition(bt, comp, 0, _player.CurrentMs, 3000, easeStore);
                //_player.AddActiveElement(newBT);

                nextComp.End = nextComp.Start;
                nextComp.Start = bt.Start;
                nextComp.Easing = easeStore;
                nextComp.GenerateBlends();
                _player.AddActiveElement(nextComp);
            }
            else
            {
                _count = 0;
                NextVersion();
            }
        }


        public static BlendTransition GetTest0(float delay, float startTime, float duration)
        {
	        var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            var cols = 15;
            var rows = 10;
            composite.AddProperty(PropertyId.Items, Store.CreateItemStore(rows * cols));

            var totalWidth = 500f;
            float growth = 60;
            var armLen = totalWidth / (float)(cols - 1) / 3f;
            var height = armLen * (float)Math.Sqrt(3) / 2f * (rows - 1f);
            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, armLen, armLen, armLen, armLen * 1.5f).Store);

            float[] start = { 150, 150, 150 + totalWidth, 150 + height };
            Sampler hexSampler = new HexagonSampler(new[] { cols, rows });
            var startStore = new Store(new FloatSeries(2, start), hexSampler);
            composite.AddProperty(PropertyId.Location, startStore);

            Composite endComp = (Composite)composite.CreateChild();
            float[] end = { start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth };
            endComp.AddProperty(PropertyId.Location, new Store(new FloatSeries(2, end), hexSampler, CombineFunction.Replace));

            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Replace, CombineTarget.T);
            return new BlendTransition(composite, endComp, delay, startTime, duration, easeStore);
        }

        public static BlendTransition GetTest1(float delay, float startTime, float duration)
        {
	        BlendTransition bt = (BlendTransition)GetTest0(delay, startTime, duration);

            IStore endLocStore = bt.End.GetStore(PropertyId.Location);
            Series minMax = new FloatSeries(2, -200f, -100f, 200f, 100f);
            var randomStore = new RandomSeries(2, SeriesType.Float, endLocStore.Capacity, minMax, 1111, CombineFunction.ContinuousAdd).CreateLinearStore(endLocStore.Capacity);
            randomStore.CombineFunction = CombineFunction.Add;
            var fs = new FunctionalStore(endLocStore, randomStore);
            ((Composite)bt.End).AddProperty(PropertyId.Location, fs);
            return bt;
        }

        public static BlendTransition GetTest2(float delay, float startTime, float duration)
        {
            var composite = new Composite();
            AddGraphic(composite);
            AddColor(composite);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 6f, 6f, 9f, 9f, 6f, 6f).Store);
            const int count = 150;
            Series maxMinA = new FloatSeries(2, 200f, 100f, 600f, 300f); //0, 0, 800f, 400f);
            Series maxMinB = new FloatSeries(2, 200f, 100f, 600f, 300f);
            var startStore = new RandomSeries(2, SeriesType.Float, count, maxMinA).CreateLinearStore(count);
            composite.AddProperty(PropertyId.Location, startStore);

            Composite endComp = (Composite)composite.CreateChild();
            var endStore = new RandomSeries(2, SeriesType.Float, count, maxMinB, 0, CombineFunction.Replace).CreateLinearStore(count);
            endComp.AddProperty(PropertyId.Location, endStore);

            startStore.Sampler = new RingSampler(new int[] {100, 25,15,10});

            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);

            composite.shouldShuffle = true;
            return new BlendTransition(composite, endComp, delay, startTime, duration, easeStore);

        }
        public static BlendTransition GetTest3(float delay, float startTime, float duration)
        {
            var startComp = new Composite();
            AddGraphic(startComp);
            AddColor(startComp);
			
            Store items = Store.CreateItemStore(150);
            // items.BakeData();
            //SeriesUtils.Shuffle(items.GetFullSeries(0));
            startComp.AddProperty(PropertyId.Items, new BlendStore(items));

            var pointArray = new FloatSeries(1, 8f, 5f, 5f, 8f);
            startComp.AddProperty(PropertyId.PointCount, new Store(pointArray, new Easing(EasingType.EaseInOut)));

            Sampler ringSampler = new RingSampler(new int[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 });
            //Sampler ringSampler = new RingSampler(new int[] { 30, 20, 15, 15, 15, 15, 15, 10, 10, 5 });
            //Sampler ringSampler = new RingSampler(new int[] { 60, 50, 40 });
            startComp.AddProperty(PropertyId.Radius, new FloatSeries(2, 6f, 6f, 15f, 15f, 6f, 6f).Store);
            var startRect = new float[] { 50, 50, 500, 400 };
            var startStore = new Store(new FloatSeries(2, startRect), ringSampler);
            startComp.AddProperty(PropertyId.Location, startStore);

            Composite endComp = (Composite)startComp.CreateChild();

            IntSeries itemData2 = new IntSeries(1, new int[] { 0, 199 });
            Store items2 = itemData2.CreateLinearStore(200);
            endComp.AddProperty(PropertyId.Items, items2);

            GridSampler gridSampler = new GridSampler(new[] { 15, 16});
            gridSampler.IsRowCol = false;
            var endRect = startRect; // new float[] { 50, 50, 500, 400 };
            var endStore = new Store(new FloatSeries(2, endRect), gridSampler, CombineFunction.Replace);
            endComp.AddProperty(PropertyId.Location, endStore);

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply, CombineTarget.T);
            return new BlendTransition(startComp, startComp, delay, startTime, duration, easeStore);
        }

        private static void AddGraphic(Composite composite)
        {
	        composite.AddProperty(PropertyId.Radius, new Store(new FloatSeries(1, 10f, 20f, 10f) ));
	        composite.AddProperty(PropertyId.PointCount, new FloatSeries(1, new float[] { 6f, 9f, 6f }).CreateLinearStore(3));
	        composite.AddProperty(PropertyId.Starness, new Store(new FloatSeries(1,  0, -0.3f, 0) ));
	        composite.AddProperty(PropertyId.Orientation, new Store(new FloatSeries(1, 0.3f, 1f / 12f, 0.3f) ));
            composite.Graphic = new PolyShape();
        }

        private static void AddColor(Composite composite)
        {
	        Sampler colorSampler = new LineSampler(); //new GridSampler(new []{10, 10});
	        var start = new float[] { 0.3f, 0.1f, 0.2f, 1f, 1f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f };
	        var end = new float[] { 0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f };
	        var colorStartStore = new Store(new FloatSeries(3, start), colorSampler);
	        var colorEndStore = new Store(new FloatSeries(3, end), colorSampler);
	        composite.AddProperty(PropertyId.FillColor, new BlendStore(colorStartStore, colorEndStore));

            composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0, 0, 0, .4f, 0, 0).Store);
            composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 0f, 2f, 0f).Store);
        }
    }
}
