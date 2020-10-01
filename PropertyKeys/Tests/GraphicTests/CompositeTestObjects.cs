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
using DataArcs.SeriesData.Utils;

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
            if (lastComp == null)
            {
	            lastComp = comp;
            }
            _player.ActivateComposite(comp.Id);
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
		            comp = GetTest3(0, 3000f);
		            break;
	            case 1:
		            comp = GetTest0(0, 2000f);
		            break;
	            case 2:
		            comp = GetTest2(0, 2000f);
		            break;
                default:
                    comp = GetTest1(0, _player.CurrentMs, 2000f);
                    break;
            }
            comp.Runner.EndTimedEvent += CompOnEndTransitionEvent;
            return comp;
        }

        private BlendTransition lastComp;
        private void CompOnEndTransitionEvent(object sender, EventArgs e)
        {
            _count++;
            if (_count < 1)
            {
                ITimeable anim = (ITimeable)sender;
                anim.Reverse();
                anim.Restart();
            }
            else if (_count < 2)
            {
                BlendTransition nextComp = GetVersion(NextVersionIndex());

                var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3), CombineFunction.Multiply);
                //BlendTransition newBT = new BlendTransition(bt, comp, 0, _player.CurrentMs, 3000, easeStore);
                //_player.ActivateElement(newBT);

                nextComp.End = nextComp.Start;
                nextComp.Start = lastComp;
                nextComp.Easing = easeStore;
                nextComp.GenerateBlends();
                _player.ActivateComposite(nextComp.Id);
                lastComp = nextComp;
            }
            else
            {
                _count = 0;
                NextVersion();
            }
        }


        public static BlendTransition GetTest0(float delay, float duration)
        {
            var cols = 15;
            var rows = 10;

	        var composite = new Container(Store.CreateItemStore(rows * cols));
            AddGraphic(composite);
            AddColor(composite);


            var totalWidth = 700f;
            float growth = 60;
            var armLen = totalWidth / (float)(cols - 1) / 3f;
            var height = armLen * (float)Math.Sqrt(3) / 2f * (rows - 1f);
            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, armLen, armLen, armLen, armLen * 1.0f).Store());

            float[] start = { 35, 150, 35 + totalWidth, 150 + height };
            Sampler hexSampler = new HexagonSampler(new[] { cols, rows });
            var startStore = new Store(new FloatSeries(2, start), hexSampler);
            composite.AddProperty(PropertyId.Location, startStore);

            Container endComp = (Container)composite.CreateChild();
            float[] end = { start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth };
            endComp.AddProperty(PropertyId.Location, new Store(new FloatSeries(2, end), hexSampler, CombineFunction.Replace));

            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Replace);
            return new BlendTransition(composite, endComp, new Timer(delay, duration), easeStore);
        }

        public static BlendTransition GetTest1(float delay, double startTime, float duration)
        {
	        BlendTransition bt = (BlendTransition)GetTest0(delay, duration);

            IStore endLocStore = bt.End.GetStore(PropertyId.Location);
            RectFSeries minMax = new RectFSeries(-8f, -4f, 8f, 4f);
            var randomStore = new RandomSeries(2, SeriesType.Float, endLocStore.Capacity, minMax, 1111, CombineFunction.ContinuousAdd).CreateLinearStore(endLocStore.Capacity);
            randomStore.CombineFunction = CombineFunction.Add;
            var fs = new MergingStore(endLocStore, randomStore);
            ((Container)bt.End).AddProperty(PropertyId.Location, fs);
            return bt;
        }

        public static BlendTransition GetTest2(float delay, float duration)
        {
            const int count = 150;
            var composite = new Container(Store.CreateItemStore(count));
            AddGraphic(composite);
            AddColor(composite);

            composite.AddProperty(PropertyId.Radius, new FloatSeries(2, 8f, 8f, 12f, 9f, 8f, 8f).Store());
            RectFSeries maxMinA = new RectFSeries(100f, 50f, 700f, 400f); //0, 0, 800f, 400f);
            RectFSeries maxMinB = new RectFSeries(100f, 50f, 700f, 400f);
            var startStore = new RandomSeries(2, SeriesType.Float, count, maxMinA).CreateLinearStore(count);
            composite.AddProperty(PropertyId.Location, startStore);

            Container endComp = (Container)composite.CreateChild();
            var endStore = new RandomSeries(2, SeriesType.Float, count, maxMinB, 0, CombineFunction.Replace).CreateLinearStore(count);
            endComp.AddProperty(PropertyId.Location, endStore);

            startStore.Sampler = new RingSampler(new int[] {100, 25,15,10});

            Store easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply);

            composite.shouldShuffle = true;
            return new BlendTransition(composite, endComp, new Timer(delay, duration), easeStore);

        }
        public static BlendTransition GetTest3(float delay, float duration)
        {
            Store items = Store.CreateItemStore(150);
            var startComp = new Container(new BlendStore(items));
            AddGraphic(startComp);
            AddColor(startComp);
			
            // items.BakeData();
            //SeriesUtils.ShuffleElements(items.GetSeriesRef(0));

            var pointArray = new FloatSeries(1, 8f, 5f, 5f, 8f);
            startComp.AddProperty(PropertyId.PointCount, new Store(pointArray, new Easing(EasingType.EaseInOut)));

            Sampler ringSampler = new RingSampler(new int[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 });
            //Sampler ringSampler = new RingSampler(new int[] { 30, 20, 15, 15, 15, 15, 15, 10, 10, 5 });
            //Sampler ringSampler = new RingSampler(new int[] { 60, 50, 40 });
            startComp.AddProperty(PropertyId.Radius, new FloatSeries(2, 6f, 6f, 15f, 15f, 6f, 6f).Store());
            var startRect = new float[] { 50, 50, 500, 400 };
            var startStore = new Store(new FloatSeries(2, startRect), ringSampler);
            startComp.AddProperty(PropertyId.Location, startStore);

            Container endComp = (Container)startComp.CreateChild();

            IntSeries itemData2 = new IntSeries(1, new int[] { 0, 159 });
            Store items2 = itemData2.CreateLinearStore(160);
            endComp.AddProperty(PropertyId.Items, items2);

            GridSampler gridSampler = new GridSampler(new[] { 10, 16}, SlotUtils.YX);
            var endRect = new float[] { 50, 50, 700, 400 };
            var endStore = new Store(new FloatSeries(2, endRect), gridSampler, CombineFunction.Replace);
            endComp.AddProperty(PropertyId.Location, endStore);
            endComp.AddProperty(PropertyId.Radius, new FloatSeries(2, 12f, 12f, 21f, 21f, 12f, 12f).Store());

            var easeStore = new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut3AndBack), CombineFunction.Multiply);
            return new BlendTransition(startComp, endComp, new Timer(delay, duration), easeStore);
        }

        private static void AddGraphic(Container container)
        {
	        //container.AddProperty(PropertyId.Radius, new Store(new FloatSeries(1, 10f, 20f, 10f) ));
	        container.AddProperty(PropertyId.PointCount, new FloatSeries(1, new float[] { 6f, 9f, 6f }).CreateLinearStore(3));
	        container.AddProperty(PropertyId.Starness, new Store(new FloatSeries(1,  0, -0.3f, 0) ));
	        container.AddProperty(PropertyId.Orientation, new Store(new FloatSeries(1, 0.3f, 1f / 12f, 0.3f) ));
            container.Renderer = new PolyShape();
        }

        private static void AddColor(Container container)
        {
	        Sampler colorSampler = new LineSampler(); //new GridSampler(new []{10, 10});
	        var start = new float[] { 0.3f, 0.1f, 0.2f,   1f, 1f, 0,       0, 0.15f, 1f,      0, 0.5f, 0.1f };
	        var end = new float[] { 0, 0.2f, 0.7f,        0.8f, 0, 0.3f,   0.7f, 1f, 0.1f,    0.4f, 0, 1f };
	        var colorStartStore = new Store(new FloatSeries(3, start), colorSampler);
	        var colorEndStore = new Store(new FloatSeries(3, end), colorSampler);
	        container.AddProperty(PropertyId.FillColor, new BlendStore(colorStartStore, colorEndStore));

            container.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0f, 0f, 0f, .3f, 0.1f, 0.1f).Store());
            container.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 0f, 2f, 0f).Store());
        }
    }
}
