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

        PolyShape graphic;
        Random rnd = new Random();

        public const int versionCount = 4;

        public Circles(int version)
        {
            //todo: Rather than seed with multiples or lerp, use ranges.
            graphic = new PolyShape(pointCount: new int[] { 6,7,8,9,10,11, 12 }, radius: new float[]{ 10f, 20f },
                orientation: new float[] { 1f / 12f, 0.3f }, starness: new float[] { 0, -0.3f });
            //graphic.PointCount.ElementCount = 6;
            SetVersion(version);
        }

        public void SetVersion(int version)
        {
            object1 = new Composite();

            Series test = new FloatSeries(1, new float[4], 11);
            int[] testr = Sampler.GetDimsForIndex(test, new int[] { 3 }, 4);
            float[] testf = Sampler.GetStrideTsForIndex(test, new int[] { 3 }, 4);
            Debug.WriteLine(testr.ToString());

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
                var endStore = new Store(new FloatSeries(2, end, virtualCount: cols * rows), sampler: hexSampler);
                if (version == 1)
                {
                    RandomSeries rs = new RandomSeries(2, SeriesType.Float, rows * cols, -25f, 25f, 1111, CombineFunction.Multiply);
                    Store randomStore = new Store(rs);
                    rs.setMinMax(.93f, 1f/.93f);
                    //endStore.HardenToData();
                    FunctionalStore fs = new FunctionalStore(endStore, randomStore);
                    object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, fs }, easingType: EasingType.Linear));
                }
                else
                {
                    object1.AddProperty(PropertyID.Location, new PropertyStore(new []{ startStore, endStore }));
                }

                startStore.HardenToData();
                endStore.HardenToData();
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

                object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, endStore }));
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
                var endStore = new Store(new FloatSeries(vectorSize, end, virtualCount: 100),
                    easingTypes: new EasingType[] { EasingType.EaseCenter, EasingType.EaseCenter }, sampler: gridSampler);
                endStore.HardenToData();
                object1.AddProperty(PropertyID.Location, new PropertyStore(new Store[] { startStore, endStore }, easingType: EasingType.Linear));
                
            }

            object1.AddProperty(PropertyID.FillColor, GetTestColors());


        }

        private PropertyStore GetTestColors()
        {
            Sampler linearSampler = new LineSampler();
            float[] start = new float[] { 0.3f, 0.1f, 0.2f,   1f, 1f, 0,  0, 0.15f, 1f,   0, 0.5f, 0.1f };
            float[] end = new float[] { 0, 0.2f, 0.7f,   0.8f, 0, 0.3f,    0.7f, 1f, 0.1f,   0.4f, 0, 1f };
            var colorStartStore = new Store(new FloatSeries(3, start), sampler:linearSampler);
            var colorEndStore = new Store(new FloatSeries(3, end), sampler: linearSampler, easingTypes: new EasingType[] { EasingType.Squared });
            return new PropertyStore(new Store[] { colorStartStore, colorEndStore }, easingType: EasingType.InverseSquared);
        }

        public void Draw(Graphics g, float t)
        {
            PropertyStore loc = object1.GetPropertyStore(PropertyID.Location);
            PropertyStore col = object1.GetPropertyStore(PropertyID.FillColor);
            PropertyStore wander = object1.GetPropertyStore(PropertyID.RandomMotion);
            
            loc.Update();

            //t = 1f;
            int floorT = (int)t;
            t = t - floorT;
            if (floorT % 2 == 0) t = 1.0f - t;
            float easedT = t;// Easing.GetTAtT(t, loc.EasingType);
            int count = loc.GetElementCountAt(easedT);
            float[] v = {0,0};
            for (int i = 0; i < count; i++)
            {
                //if (i > 88 && i < 111)//count - 1)
                //{
                //    float itx = i / (float)(count - 1f);
                //    var vx = v = loc.GetValuesAtT(itx, easedT).Floats;
                //    Debug.WriteLine(i + "::" + vx[0] + " : " + vx[1]);
                //}
                float it = i / (float)(count - 1f);
                v = loc.GetValuesAtT(it, easedT).Floats;

                Color c = GraphicUtils.GetRGBColorFrom(col.GetValuesAtT(it, easedT));
                Brush b = new SolidBrush(c);
                GraphicsState state = g.Save();
                float scale = 1f; //  + t * 0.2f;
                g.ScaleTransform(scale, scale);
                g.TranslateTransform(v[0] / scale, v[1] / scale);
                graphic.Draw(g, b, null, easedT);
                g.Restore(state);
            }
            //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
        }
    }
}
