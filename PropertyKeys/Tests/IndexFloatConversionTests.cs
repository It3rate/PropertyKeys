using DataArcs.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataArcs.Tests
{
    [TestClass]
    public class IndexFloatConversionTests
    {
        FloatSeries series_1;
        FloatSeries series_2;
        float delta = 0.00001f;
        [TestInitialize]
        public void TestInitializer()
        {
            series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
            series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
        }
        [TestMethod]
        public void TestTVectorSize1()
        {
            TVectorSize1();
            series_1.HardenToData();
            TVectorSize1();
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
        public void TVectorSize2()
        {
            //series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
            float[] sample;
            sample = series_2.GetValueAtT(-10f).Floats;
            ArraysEqual(new float[] { 10f, 20f }, sample, delta);
            sample = series_2.GetValueAtT(0f).Floats;
            ArraysEqual(new float[] { 10f, 20f }, sample, delta);

            sample = series_2.GetValueAtT(0.25f).Floats;
            ArraysEqual(new float[] { 60f, 120f }, sample, delta);
            sample = series_2.GetValueAtT(0.5f).Floats;
            ArraysEqual(new float[] { 110f, 220f }, sample, delta);
            sample = series_2.GetValueAtT(0.75f).Floats;
            ArraysEqual(new float[] { 210f, 320f }, sample, delta);

            sample = series_2.GetValueAtT(1f).Floats;
            ArraysEqual(new float[] { 310f, 420f }, sample, delta);
            sample = series_2.GetValueAtT(1.1f).Floats;
            ArraysEqual(new float[] { 310f, 420f }, sample, delta);
            sample = series_2.GetValueAtT(10f).Floats;
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

        public void ArraysEqual(float[] a, float[]b, float delta)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i], delta);
            }
        }
    }
}
