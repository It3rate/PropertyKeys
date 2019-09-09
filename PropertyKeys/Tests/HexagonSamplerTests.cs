using DataArcs.Samplers;
using DataArcs.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Tests
{
    [TestClass]
    public class HexagonSamplerTests
    {
        FloatSeries series_1;
        FloatSeries series_1_30;
        FloatSeries series_2;
        float delta = 0.0001f;

        [TestInitialize]
        public void TestInitializer()
        {
            series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
            series_1_30 = new FloatSeries(1, new float[] { 1f, 2f, 11f, 22f }, virtualCount: 30);
            series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
        }
        [TestMethod]
        public void TestHexagonSamplerByIndex_1()
        {
            Series series = new FloatSeries(2, new[] { 0f, 0f, 100f, 200f }, virtualCount: 100);
            int[] strides = { 10 };
            Series sample;
            Series expected;
            HexagonSampler sampler = new HexagonSampler(strides);

            sample = sampler.GetValueAtIndex(series, 0);
            expected = new FloatSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(Series.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series, 1);
            expected = new FloatSeries(2, new[] { 11.11111f, 0f });
            Assert.IsTrue(Series.IsEqual(sample, expected));
            sample = sampler.GetValueAtIndex(series, 10);
            expected = new FloatSeries(2, new[] { 5.55555555f, 22.222222f });
            Assert.IsTrue(Series.IsEqual(sample, expected));
            sample = sampler.GetValueAtIndex(series, 11);
            expected = new FloatSeries(2, new[] { 16.66666f, 22.222222f });
            Assert.IsTrue(Series.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series, 99);
            expected = new FloatSeries(2, new[] { 105.555555f, 200f }); // even rows are offset to make hex grid
            Assert.IsTrue(Series.IsEqual(sample, expected));

        }
    }
}
