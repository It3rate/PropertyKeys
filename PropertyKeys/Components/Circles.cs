using System;
using System.Drawing;
using DataArcs.Graphic;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

// Todo:
// Animate composites
// Sequencer
// convert to cuda
// hook up to commands
// definitions/instances
// basic UI
// Add algorithmic step sampler (physics, navier stokes, runge kutta, reaction diffusion etc)
// add ML in simple 'bacteria' test
// Property combination (vs simple override) so add, multiply etc.
// matrix support

namespace DataArcs.Components
{
	public class Circles
	{
		private Composite object1;
		private Store easeStore; // to animation eventually
		private int version;
		public const int versionCount = 4;
		private Random rnd = new Random();

		public Circles(int version)
		{
			//todo: Rather than seed with multiples or lerp, use ranges.
			//graphic.PointCount.ElementCount = 6;
			SetVersion(version);
		}

		public void SetVersion(int ver)
		{
			version = ver;
			easeStore = version == 2 ?
				new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.SmoothStep4), CombineFunction.Multiply, CombineTarget.T) :
				new Store(new FloatSeries(1, 0f, 1f), new Easing(EasingType.EaseInOut2), CombineFunction.Multiply, CombineTarget.T);
            object1 = new Composite();
			var graphic = new PolyShape( radius: new float[] {10f, 20f},
				orientation: new float[] {1f / 12f, 0.3f}, starness: new float[] {0, -0.3f});
            Store points = new FloatSeries(1, new float[] { 6.0f, 10.99f }, 6).Store;
            graphic.PointCount = points;

            if (version == 0 || version == 1)
			{
				var cols = 10;
				var rows = 10;
                Store items = new IntSeries(1, new int[] { 0, rows * cols - 1 }, virtualCount: rows * cols).Store;
                object1.AddProperty(PropertyId.Items, new BlendStore(items));

				var totalWidth = 500f;
				float growth = 60;
				//graphic.Orientation = 0.5f;
				var armLen = totalWidth / (float) (cols - 1) / 3f;
				var height = armLen * (float) Math.Sqrt(3) / 2f * (rows - 1f);
				graphic.Radius = new FloatSeries(2, armLen, armLen, armLen, armLen * 1.5f).Store;

				float[] start = {150, 150, 150 + totalWidth, 150 + height};
				Sampler hexSampler = new HexagonSampler(new[] {cols, 0});
				var startStore = new Store(new FloatSeries(2, start, cols * rows), hexSampler);

				float[] end = {start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth};
				var endStore = new Store(new FloatSeries(2, end, rows * cols), hexSampler);
				if (version == 1)
				{
                    Series minMax = new FloatSeries(2, -4f, -3f, 4f, 3f);
					var randomStore = new RandomSeries(2, SeriesType.Float, rows * cols, minMax, 1111, CombineFunction.ContinuousAdd).Store;
					// rs.setMinMax(.98f, 1f/.98f);
					//endStore.HardenToData();
					var fs = new FunctionalStore(endStore, randomStore);
					object1.AddProperty(PropertyId.Location, new BlendStore(startStore, fs));
				}
				else
				{
					object1.AddProperty(PropertyId.Location, new BlendStore(startStore, endStore));
				}

				//startStore.HardenToData();
				//endStore.HardenToData();
				//endStore.Series.Shuffle();
			}
			else if (version == 2)
			{
				graphic.Radius = new FloatSeries(2, 10f, 15f, 20f, 15f).Store;
				const int count = 50;
				const int vectorSize = 2;
                Series maxMinA = new FloatSeries(2, 0, 0, 1000f, 500f);
                Series maxMinB = new FloatSeries(2, 200f, 100f, 600f, 300f);
                var startStore = new RandomSeries(2, SeriesType.Float, count, maxMinA).Store;
                var endStore = new RandomSeries(2, SeriesType.Float, count, maxMinB).Store;
    //            var start = new float[count * vectorSize];
				//var end = new float[count * vectorSize];
				//for (var i = 0; i < count * vectorSize; i += vectorSize)
				//{
				//	start[i] = rnd.Next(500) + 100;
				//	start[i + 1] = rnd.Next(300) + 50;
				//	end[i] = start[i] + rnd.Next((int) start[i]) - start[i] / 2.0f;
				//	end[i + 1] = start[i + 1] + rnd.Next(100) - 50;
				//}

				//var startStore = new FloatSeries(vectorSize, start).Store;
				//var endStore = new FloatSeries(vectorSize, end).Store;
				//endStore.HardenToData();
				//SeriesUtils.Shuffle(endStore.GetSeries(0));

				object1.AddProperty(PropertyId.Location, new BlendStore(startStore, endStore));
				object1.shouldShuffle = true;
			}
			else if (version == 3)
            {
                IntSeries itemData = new IntSeries(1, new int[] { 0, 149 }, virtualCount: 150);
                itemData = (IntSeries)itemData.HardenToData();
                SeriesUtils.Shuffle(itemData);
                Store items = itemData.Store;
                object1.AddProperty(PropertyId.Items, new BlendStore(new Store[] { items }));

                graphic.PointCount = new float[]{3f,5.9f}.ToStore();
				Sampler ringSampler = new RingSampler();
				Sampler gridSampler = new GridSampler(new[] {15, 0, 0});
				graphic.Radius = new FloatSeries(2, 5f, 5f, 15f, 15f).Store;
				var vectorSize = 2;
				var start = new float[] {100, 100, 500, 400};
				var end = new float[] {100, 100, 500, 400};
				var startStore = new Store(new FloatSeries(vectorSize, start, 150), ringSampler);
				var endStore = new Store(new FloatSeries(vectorSize, end, 150), gridSampler);
				//var fnStore = new FunctionalStore(startStore, endStore);
				//endStore.HardenToData();
				object1.AddProperty(PropertyId.Location, new BlendStore( startStore, endStore));
			}

			object1.AddProperty(PropertyId.FillColor, GetTestColors());
			object1.Graphic = graphic;
		}

		private BlendStore GetTestColors()
		{
			Sampler linearSampler = new LineSampler();
			var start = new float[] {0.3f, 0.1f, 0.2f, 1f, 1f, 0, 0, 0.15f, 1f, 0, 0.5f, 0.1f};
			var end = new float[] {0, 0.2f, 0.7f, 0.8f, 0, 0.3f, 0.7f, 1f, 0.1f, 0.4f, 0, 1f};
			var colorStartStore = new Store(new FloatSeries(3, start), linearSampler);
			var colorEndStore =
				new Store(new FloatSeries(3, end), linearSampler);
			return new BlendStore(colorStartStore, colorEndStore);
		}

		public void Draw(Graphics g, float time)
		{
			time = (version == 2) ? time * 3f : time;
			var floorT = (int)time;
			float t = time - floorT;
			if (floorT % 2 == 0)
			{
				t = 1.0f - t;
			}

			t = easeStore.GetSeriesAtT(t).FloatDataAt(0);
			object1.Update(t);
			object1.Draw(g);
		}
	}
}