using DataArcs.Graphic;
using DataArcs.Stores;
using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using DataArcs.Mutators;

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
        Composite parent0;
        Composite object1;
        
        Random rnd = new Random();

        public const int versionCount = 4;

        public Circles(int version)
        {
            //todo: Rather than seed with multiples or lerp, use ranges.
            //graphic.PointCount.ElementCount = 6;
            SetVersion(version);
        }

        public void SetVersion(int version)
        {
            object1 = new Composite();
            PolyShape graphic = new PolyShape(pointCount: new int[] { 6, 7, 8, 9, 10, 11, 12 }, radius: new float[] { 10f, 20f },
                orientation: new float[] { 1f / 12f, 0.3f }, starness: new float[] { 0, -0.3f });

            if (version == 0 || version == 1)
            {
                int cols = 10;
                int rows =10;

                float totalWidth = 500f;
                float growth = 60;
                //graphic.Orientation = 0.5f;
                float armLen = totalWidth / (float)(cols - 1) / 3f;
                float height = (armLen * (float)Math.Sqrt(3)) / 2f * (rows - 1f);
                graphic.Radius = new Store(new FloatSeries(2, armLen, armLen, armLen, armLen * 1.5f));

                float[] start = { 150, 150,   150 + totalWidth, 150 + height };
                Sampler hexSampler = new HexagonSampler(new [] { cols, 0});
                var startStore = new Store(new FloatSeries(2, start, virtualCount: cols * rows), sampler: hexSampler);

                float[] end = {start[0] - growth, start[1] - growth, start[2] + growth, start[3] + growth};
                var endStore = new Store(new FloatSeries(2, end, virtualCount: rows * cols+11), sampler: hexSampler);
                if (version == 1)
                {
                    startStore.VirtualCount = rows * cols;
                    endStore.VirtualCount = rows * cols;
                    RandomSeries rs = new RandomSeries(2, SeriesType.Float, rows * cols, -3f, 3f, 1111, CombineFunction.Add);
                    Store randomStore = new Store(rs);
                   // rs.setMinMax(.98f, 1f/.98f);
                    //endStore.HardenToData();
                    FunctionalStore fs = new FunctionalStore(endStore, randomStore);
                    object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, fs }));
                }
                else
                {
                    object1.AddProperty(PropertyID.Location, new PropertyStore(new []{ startStore, endStore }));
                }

                //startStore.HardenToData();
                //endStore.HardenToData();
                //endStore.Series.Shuffle();
            }
            else if (version == 2)
            {
                graphic.Radius = new Store(new FloatSeries(2, 10f, 15f, 20f, 15f));
                const int count = 50;
                const int vectorSize = 2;
                float[] start = new float[count * vectorSize];
                float[] end = new float[count * vectorSize];
                for (int i = 0; i < count * vectorSize; i += vectorSize)
                {
                    start[i] = rnd.Next(500) + 100;
                    start[i + 1] = rnd.Next(300) + 50;
                    end[i] = start[i] + rnd.Next((int)start[i]) - start[i] / 2.0f;
                    end[i + 1] = start[i + 1] + rnd.Next(100) - 50;
                }
                var startStore = new Store(new FloatSeries(vectorSize, start));
                var endStore = new Store(new FloatSeries(vectorSize, end));
                endStore.HardenToData();
                SeriesUtils.Shuffle(endStore.Series);

                object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, endStore }));
                object1.shouldShuffle = true;
            }
            else if (version == 3)
            {
                Sampler ringSampler = new RingSampler();
                Sampler gridSampler = new GridSampler(new[] { 10, 0, 0 });
                graphic.Radius = new Store(new FloatSeries(2, 5f, 5f, 20f, 20f));
                int vectorSize = 2;
                float[] start = new float[] { 200, 40, 400, 200 };
                float[] end = new float[] { 100, 100, 500, 400 };
                var startStore = new Store(new FloatSeries(vectorSize, start, virtualCount: 100), sampler: ringSampler);
                var endStore = new Store(new FloatSeries(vectorSize, end, virtualCount: 50),
                    easingTypes: new EasingType[] { EasingType.EaseCenter, EasingType.EaseCenter }, sampler: gridSampler);
                endStore.HardenToData();
                object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, endStore }));
                
            }

            object1.AddProperty(PropertyID.FillColor, GetTestColors());
            object1.Graphic = graphic;

        }

        private PropertyStore GetTestColors()
        {
            Sampler linearSampler = new LineSampler();
            float[] start = new float[] { 0.3f, 0.1f, 0.2f,   1f, 1f, 0,  0, 0.15f, 1f,   0, 0.5f, 0.1f };
            float[] end = new float[] { 0, 0.2f, 0.7f,   0.8f, 0, 0.3f,    0.7f, 1f, 0.1f,   0.4f, 0, 1f };
            var colorStartStore = new Store(new FloatSeries(3, start), sampler:linearSampler);
            var colorEndStore = new Store(new FloatSeries(3, end), sampler: linearSampler, easingTypes: new EasingType[] { EasingType.Squared });
            return new PropertyStore(new Store[] { colorStartStore, colorEndStore });
        }

        public void Draw(Graphics g, float t)
        {
            object1.Update(t);
            object1.Draw(g);
        }
    }
}
