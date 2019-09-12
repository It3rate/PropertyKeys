using DataArcs.Samplers;
using DataArcs.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataArcs.Tests
{
    [TestClass]
    public class IndexFloatConversionTests
    {
        FloatSeries series_1;
        FloatSeries series_1_31;
        FloatSeries series_2;
        float delta = 0.00001f;
        [TestInitialize]
        public void TestInitializer()
        {
            series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
            series_1_31 = new FloatSeries(1, new float[] { 1f, 2f, 11f, 22f }, virtualCount:31);
            series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
        }
        [TestMethod]
        public void TestGetDimsForIndex()
        {
            Series series = new FloatSeries(1, new[] { 0f, 1f }, virtualCount: 100);
            int[] strides = { 10 };
            int[] sample;
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, -3);
            ArraysEqual(new int[] { 0, 0 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 0);
            ArraysEqual(new int[] { 0, 0 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 1);
            ArraysEqual(new int[] { 1, 0 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 10);
            ArraysEqual(new int[] { 0, 1 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 11);
            ArraysEqual(new int[] { 1, 1 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 53);
            ArraysEqual(new int[] { 3, 5 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 99);
            ArraysEqual(new int[] { 9, 9 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 100);
            ArraysEqual(new int[] { 9, 9 }, sample);
            sample = Sampler.GetDimsForIndex(series.VirtualCount, strides, 101);
            ArraysEqual(new int[] { 9, 9 }, sample);
        }
        [TestMethod]
        public void TestGetStrideTsForIndex()
        {
            Series series = new FloatSeries(1, new[] { 0f, 1f }, virtualCount: 100);
            int[] strides = { 10 };
            float cl = strides[0] - 1f;
            float[] sample;
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, -3);
            ArraysEqual(new float[] { 0, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 0);
            ArraysEqual(new float[] { 0, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 1);
            ArraysEqual(new float[] { 1f/cl, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 9);
            ArraysEqual(new float[] { 1f, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 10);
            ArraysEqual(new float[] { 0, 1f / cl }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 11);
            ArraysEqual(new float[] { 1f/cl, 1f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 53);
            ArraysEqual(new float[] { 3f/cl, 5f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 99);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 100);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForIndex(series.VirtualCount, strides, 101);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
        }
        [TestMethod]
        public void TestGetStrideTsForT()
        {
            Series series = new FloatSeries(1, new[] { 0f, 1f }, virtualCount: 100);
            int[] strides = { 10 };
            float cl = strides[0] - 1f;
            float len = series.VirtualCount - 1f;
            float[] sample;
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, -3f);
            ArraysEqual(new float[] { 0, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 0);
            ArraysEqual(new float[] { 0, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 1f / len);
            ArraysEqual(new float[] { 1f/cl, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 9f / len);
            ArraysEqual(new float[] { 1f, 0 }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 10f / len);
            ArraysEqual(new float[] { 0, 1f / cl }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 11f / len);
            ArraysEqual(new float[] { 1f/cl, 1f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 53f/len);
            ArraysEqual(new float[] { 3f/cl, 5f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 1f);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 100f/len);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
            sample = Sampler.GetStrideTsForT(series.VirtualCount, strides, 101/len);
            ArraysEqual(new float[] { 9f/cl, 9f/cl }, sample, delta);
        }
        [TestMethod]
        public void TestTVectorSize1()
        {
            TVectorSize1();
            series_1.HardenToData();
            TVectorSize1();

            IndexVectorSize1();
            series_1_31.HardenToData();
            IndexVectorSize1();
        }
        [TestMethod]
        public void TestTVectorSize2()
        {
            TVectorSize2();
            series_2.HardenToData();
            TVectorSize2();
        }
        public void TVectorSize1()
        {
            // series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
            float sample;
            sample = series_1.GetValueAtT(0f)[0];
            Assert.AreEqual(1f, sample, delta);

            sample = series_1.GetValueAtT(0.25f)[0];
            Assert.AreEqual(1.75f, sample, delta);
            sample = series_1.GetValueAtT(1f / 3f)[0];
            Assert.AreEqual(2f, sample, delta);
            sample = series_1.GetValueAtT(0.5f)[0];
            Assert.AreEqual(6.5f, sample, delta);
            sample = series_1.GetValueAtT(2f / 3f)[0];
            Assert.AreEqual(11f, sample, delta);
            sample = series_1.GetValueAtT(0.75f)[0];
            Assert.AreEqual(13.75f, sample, delta);

            sample = series_1.GetValueAtT(1f)[0];
            Assert.AreEqual(22f, sample, delta);
            sample = series_1.GetValueAtT(1.2f)[0];
            Assert.AreEqual(22f, sample, delta);
            sample = series_1.GetValueAtT(11f)[0];
            Assert.AreEqual(22f, sample, delta);
        }
        public void IndexVectorSize1()
        {
            //series_1_40 = new FloatSeries(1, new float[] { 1f, 2f, 11f, 22f }, virtualCount: 31);
            float sample;
            sample = series_1_31.GetSeriesAtIndex(-10)[0];
            Assert.AreEqual(1f, sample, delta);
            sample = series_1_31.GetSeriesAtIndex(0)[0];
            Assert.AreEqual(1f, sample, delta);
            sample = series_1_31.GetSeriesAtIndex(3)[0];
            Assert.AreEqual(22f, sample, delta);
            sample = series_1_31.GetSeriesAtIndex(99)[0];
            Assert.AreEqual(22f, sample, delta);

            sample = series_1_31.GetValueAtVirtualIndex(-10)[0];
            Assert.AreEqual(1f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(0)[0];
            Assert.AreEqual(1f, sample, delta);

            sample = series_1_31.GetValueAtVirtualIndex(1)[0];
            Assert.AreEqual(1.1f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(2)[0];
            Assert.AreEqual(1.2f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(3)[0];
            Assert.AreEqual(1.3f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(4)[0];
            Assert.AreEqual(1.4f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(5)[0];
            Assert.AreEqual(1.5f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(10)[0];
            Assert.AreEqual(2f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(15)[0];
            Assert.AreEqual(6.5f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(20)[0];
            Assert.AreEqual(11f, sample, delta);
            sample = series_1_31.GetValueAtVirtualIndex(30)[0];
            Assert.AreEqual(22f, sample, delta);
            
        }
        public void TVectorSize2()
        {
            //series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
            float[] sample;
            sample = series_2.GetValueAtT(-10f).FloatData;
            ArraysEqual(new float[] { 10f, 20f }, sample, delta);
            sample = series_2.GetValueAtT(0f).FloatData;
            ArraysEqual(new float[] { 10f, 20f }, sample, delta);

            sample = series_2.GetValueAtT(0.25f).FloatData;
            ArraysEqual(new float[] { 60f, 120f }, sample, delta);
            sample = series_2.GetValueAtT(0.5f).FloatData;
            ArraysEqual(new float[] { 110f, 220f }, sample, delta);
            sample = series_2.GetValueAtT(0.75f).FloatData;
            ArraysEqual(new float[] { 210f, 320f }, sample, delta);

            sample = series_2.GetValueAtT(1f).FloatData;
            ArraysEqual(new float[] { 310f, 420f }, sample, delta);
            sample = series_2.GetValueAtT(1.1f).FloatData;
            ArraysEqual(new float[] { 310f, 420f }, sample, delta);
            sample = series_2.GetValueAtT(10f).FloatData;
            ArraysEqual(new float[] { 310f, 420f }, sample, delta);

            //float testSample = series_2.GetValueAtT(1f / 3f)[0];
            //Assert.AreEqual(2f, testSample, delta);
            //testSample = series_2.GetValueAtT(2f / 3f)[0];
            //Assert.AreEqual(11f, testSample, delta);
            //testSample = series_2.GetValueAtT(0.25f)[0];
            //Assert.AreEqual(1.75f, testSample, delta);
            //testSample = series_2.GetValueAtT(0.5f)[0];
            //Assert.AreEqual(6.5f, testSample, delta);
            //testSample = series_2.GetValueAtT(0.75f)[0];
            //Assert.AreEqual(13.75f, testSample, delta);
        }

        public void ArraysEqual(float[] a, float[] b, float delta)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i], delta);
            }
        }
        public void ArraysEqual(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }
    }
}
