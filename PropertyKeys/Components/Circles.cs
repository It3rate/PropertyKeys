using DataArcs.Graphic;
using DataArcs.Stores;
using DataArcs.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

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
        bool wanders = true;

        PolyShape graphic;
        Random rnd = new Random();

        public const int versionCount = 4;

        public Circles(int version)
        {
            graphic = new PolyShape(pointCount: 6, radius: 10f, orientation:1f/12f);
            SetVersion(version);
        }

        public void SetVersion(int version)
        {
            object1 = new Composite();

            if (version == 0 || version == 1)
            {
                int cols = 10;
                int rows = 6;
                float totalWidth = 500f;
                float growth = 25;
                //graphic.Orientation = 0.5f;
                float armLen = totalWidth / (float)(cols - 1) / 3f;
                float height = (armLen * (float)Math.Sqrt(3)) / 2f * (rows - 1f);
                graphic.Radius = armLen;

                float[] start = new float[] { 150, 150,   150 + totalWidth, 150 + height };
                var startStore = new FloatStore(2, start, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Hexagon);

                float[] end = new float[] {
                    startStore[0][0] - growth, startStore[0][1] - growth,
                    startStore[1][0] + growth, startStore[1][1] + growth};
                var endStore = new FloatStore(2, end, elementCount: cols * rows, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Hexagon);
                object1.AddProperty(PropertyID.Location, new PropertyStore(new FloatStore[] { startStore, endStore }));
                wanders = (version == 1);
            }
            else if (version == 2)
            {
                graphic.Radius = 30;
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
                var startStore = new FloatStore(vectorSize, start);
                var endStore = new FloatStore(vectorSize, end);

                object1.AddProperty(PropertyID.Location, new PropertyStore(new FloatStore[] { startStore, endStore }));
                wanders = true;
            }
            else if (version == 3)
            {
                graphic.Radius = 10;
                int vectorSize = 2;
                float[] start = new float[] { 200, 40, 400, 200 };
                float[] end = new float[] { 200, 200, 400, 400};
                var startStore = new FloatStore(vectorSize, start, elementCount: 66, dimensions: new int[] { 4, 0, 0 }, sampleType: SampleType.Ring);
                var endStore = new FloatStore(vectorSize, end, elementCount: 36, dimensions: new int[] { 6, 0, 0 },
                    easingTypes: new EasingType[] { EasingType.Squared, EasingType.InverseSquared }, sampleType: SampleType.Grid);

                object1.AddProperty(PropertyID.Location, new PropertyStore(new FloatStore[] { startStore, startStore, endStore, endStore }, easingType: EasingType.InverseSquared));
                
                
                wanders = false;
            }

            object1.AddProperty(PropertyID.FillColor, GetTestColors());

            if (wanders)
            {
                PropertyStore loc = object1.GetPropertyStore(PropertyID.Location);
                int len = loc.ValueStores[0].ElementCount * 2;
                PropertyStore  wander = new PropertyStore(new FloatStore[] {
                new FloatStore(2, new float[len], sampleType: SampleType.Line),
                new FloatStore(2, new float[len], sampleType: SampleType.Line)});
                wander.ValueStores[0].ElementCount = loc.ValueStores[0].ElementCount;
                wander.ValueStores[1].ElementCount = loc.ValueStores[1].ElementCount;
                object1.AddProperty(PropertyID.RandomMotion, wander);
            }

        }

        private PropertyStore GetTestColors()
        {
            float[] start = new float[] { 0.3f, 0.1f, 0,   1f, 1f, 0,  0, 0.15f, 1f,   0, 0.5f, 0.1f };
            float[] end = new float[] { 0.8f, 0, 0.8f,   0, 1f, 0.1f,   0.4f, 1f, 0.1f,   0, 0, 1f };
            var colorStartStore = new FloatStore(3, start, sampleType: SampleType.Line);
            var colorEndStore = new FloatStore(3, end, sampleType: SampleType.Line, easingTypes: new EasingType[] { EasingType.Squared });
            return new PropertyStore(new FloatStore[] { colorStartStore, colorEndStore }, easingType: EasingType.InverseSquared);
        }

        public void Draw(Graphics g, float t)
        {
            PropertyStore loc = object1.GetPropertyStore(PropertyID.Location);
            PropertyStore col = object1.GetPropertyStore(PropertyID.FillColor);
            PropertyStore wander = object1.GetPropertyStore(PropertyID.RandomMotion);
            //t = 1f;
            int floorT = (int)t;
            t = t - floorT;
            if (floorT % 2 == 0) t = 1.0f - t;
            float easedT = t;// Easing.GetValueAt(t, loc.EasingType);
            int count = loc.GetElementCountAt(easedT);
            GraphicsState state;
            for (int i = 0; i < count; i++)
            {
                float[] v = loc.GetValuesAtIndex(i, easedT);
                if (wander != null)
                {
                    wander.ValueStores[0].NudgeValuesBy(0.4f);
                    wander.ValueStores[1].NudgeValuesBy(0.4f);
                    float[] wan = wander.GetValuesAtIndex(i, easedT);
                    v[0] += wan[0];
                    v[1] += wan[1];
                }
                float it = i / (float)count;
                Color c = GraphicUtils.GetRGBColorFrom(col.GetValuesAtT(it, easedT));
                Brush b = new SolidBrush(c);
                state = g.Save();
                float scale = 1f; //  + t * 0.2f;
                g.ScaleTransform(scale, scale);
                g.TranslateTransform(v[0] / scale, v[1] / scale);
                graphic.Orientation = 1f/12f +  t / 12f;
                graphic.Draw(g, b, null, easedT);
                g.Restore(state);
            }
            //g.DrawRectangle(Pens.Blue, new Rectangle(200, 200, 200, 200));
        }
    }
}
