using PropertyKeys.Graphic;
using PropertyKeys.Keys;
using PropertyKeys.Samplers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
namespace PropertyKeys.Components
{
    public class Circles
    {
        ObjectKeys object1;
        PropertyKey Location { get; set; }
        PropertyKey Color { get; set; }

        PropertyKey Wander { get; set; }
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
            if (version == 0 || version == 1)
            {
                int cols = 6;
                int rows = 6;
                float totalWidth = 300f;
                float growth = 25;
                //graphic.Orientation = 0.5f;
                float armLen = totalWidth / (float)(cols - 1) / 3f;
                float height = (armLen * (float)Math.Sqrt(3)) / 2f * (rows - 1f);
                graphic.Radius = armLen;

                float[] start = new float[] { 150, 150,   150 + totalWidth, 150 + height };
                var startKey = new FloatStore(2, start, elementCount: cols * cols, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Hexagon);

                float[] end = new float[] {
                    startKey[0][0] - growth, startKey[0][1] - growth,
                    startKey[1][0] + growth, startKey[1][1] + growth};
                var endKey = new FloatStore(2, end, elementCount: cols * cols, dimensions: new int[] { cols, 0, 0 }, sampleType: SampleType.Hexagon);
                //Location = new PropertyKey(startKey, null);
                Location = new PropertyKey(new BaseValueStore[] { startKey, endKey });
                SetColor(startKey, endKey);
                wanders = version == 1;
            }
            else if (version == 2)
            {
                graphic.Radius = 30;
                int count = 50;
                int vectorSize = 2;
                float[] start = new float[count * vectorSize];
                float[] end = new float[count * vectorSize];
                for (int i = 0; i < count * vectorSize; i += vectorSize)
                {
                    start[i] = rnd.Next(500) + 100;
                    start[i + 1] = rnd.Next(300) + 50;
                    end[i] = start[i] + rnd.Next((int)start[i]) - start[i] / 2.0f;
                    end[i + 1] = start[i + 1] + rnd.Next(100) - 50;
                }
                var startKey = new FloatStore(vectorSize, start);
                var endKey = new FloatStore(vectorSize, end);
                Location = new PropertyKey(new BaseValueStore[] { startKey, endKey });
                SetColor(startKey, endKey);
                wanders = true;
            }
            else if (version == 3)
            {
                graphic.Radius = 10;
                int vectorSize = 2;
                float[] start = new float[] { 200, 140, 400, 300 };
                float[] end = new float[] { 200, 200, 400, 400};
                var startKey = new FloatStore(vectorSize, start, elementCount: 66, dimensions: new int[] { 4, 0, 0 }, sampleType: SampleType.Ring);
                var endKey = new FloatStore(vectorSize, end, elementCount: 36, dimensions: new int[] { 6, 0, 0 },
                    easingTypes: new EasingType[] { EasingType.Squared, EasingType.Linear }, sampleType: SampleType.Grid);
                Location = new PropertyKey(new BaseValueStore[] { startKey, startKey, endKey, endKey }, easingType: EasingType.InverseSquared);

                SetColor(startKey, endKey);
                wanders = false;
            }
            
            int len = Location.ValueKeys[0].ElementCount * 2;
            Wander = new PropertyKey(new BaseValueStore[] {
                new FloatStore(2, new float[len], sampleType: SampleType.Line),
                new FloatStore(2, new float[len], sampleType: SampleType.Line)} );
            Wander.ValueKeys[0].ElementCount = Location.ValueKeys[0].ElementCount;
            Wander.ValueKeys[1].ElementCount = Location.ValueKeys[1].ElementCount;
        }

        private void SetColor(BaseValueStore startKey, BaseValueStore endKey)
        {
            float[] colorStart = new float[] { 0.3f, 0.1f, 0,   1f, 1f, 0,  0, 0.15f, 1f,   0, 0.5f, 0.1f };
            float[] colorEnd = new float[] { 0.8f, 0, 0.8f,   0, 1f, 0.1f,   0.4f, 1f, 0.1f,   0, 0, 1f };
            var colorStartKey = new FloatStore(3, colorStart, elementCount: startKey.ElementCount, sampleType: SampleType.Line);
            var colorEndKey = new FloatStore(3, colorEnd, elementCount: endKey.ElementCount, sampleType: SampleType.Line, easingTypes: new EasingType[] { EasingType.Squared });
            Color = new PropertyKey(new BaseValueStore[] { colorStartKey, colorEndKey }, easingType: EasingType.InverseSquared);
        }

        public void Draw(Graphics g, float t)
        {
            //t = 1f;
            int floorT = (int)t;
            t = t - floorT;
            if (floorT % 2 == 0) t = 1.0f - t;
            float easedT = Easing.GetValueAt(t, Location.EasingType);
            int count = Location.GetElementCountAt(easedT);
            GraphicsState state;
            for (int i = 0; i < count; i++)
            {
                float[] v = Location.GetValuesAtIndex(i, easedT);
                if (wanders)
                {
                    Wander.ValueKeys[0].NudgeValuesBy(0.4f);
                    Wander.ValueKeys[1].NudgeValuesBy(0.4f);
                    float[] wan = Wander.GetValuesAtIndex(i, easedT);
                    v[0] += wan[0];
                    v[1] += wan[1];
                }
                float it = i / (float)count;
                Color c = GraphicUtils.GetRGBColorFrom(Color.GetValuesAtIndex(i, easedT));
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
