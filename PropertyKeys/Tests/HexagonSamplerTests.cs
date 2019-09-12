﻿using DataArcs.Samplers;
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
        float delta = 0.0001f;

        [TestInitialize]
        public void TestInitializer()
        {
            //series_1 = new FloatSeries(2, new[] { 0f, 0f, 100f, 200f }, virtualCount: 100);
        }
        [TestMethod]
        public void TestHexagonSamplerByT_1()
        {
            series_1 = new FloatSeries(2, new[] { 0f, 0f, 100f, 200f }, virtualCount: 100);
            HexagonSamplerByT_1();
            series_1.HardenToData();
            HexagonSamplerByT_1();
        }

        [TestMethod]
        public void TestHexagonSamplerByIndex_1()
        {
            series_1 = new FloatSeries(2, new[] { 0f, 0f, 100f, 200f }, virtualCount: 100);
            HexagonSamplerByIndex_1();
            series_1.HardenToData();
            HexagonSamplerByIndex_1();
        }

        [TestMethod]
        public void TestVisualSampleByIndex()
        {
            int cols = 10;
            int rows = 10;
            float totalWidth = 500f;
            float armLen = totalWidth / (float)(cols - 1) / 3f;
            float height = (armLen * (float)Math.Sqrt(3)) / 2f * (rows - 1f);
            float[] start = { 150, 150, 150 + totalWidth, 150 + height };
            series_1 = new FloatSeries(2, start, virtualCount: cols * rows);
            HexagonSampler sampler = new HexagonSampler(new[] { cols, 0 });

            VisualSampleByIndex(sampler);
            series_1.HardenToData();
            VisualSampleByIndex(sampler);
        }
        [TestMethod]
        public void TestVisualSampleByT()
        {
            int cols = 10;
            int rows = 10;
            float totalWidth = 500f;
            float armLen = totalWidth / (float)(cols - 1) / 3f;
            float height = (armLen * (float)Math.Sqrt(3)) / 2f * (rows - 1f);
            float[] start = { 150, 150, 150 + totalWidth, 150 + height };
            series_1 = new FloatSeries(2, start, virtualCount: cols * rows);
            HexagonSampler sampler = new HexagonSampler(new[] { cols, 0 });

            VisualSampleByT(sampler);
            series_1.HardenToData();
            VisualSampleByT(sampler);
        }

        public void VisualSampleByT(HexagonSampler sampler)
        {
            var startStore = new Store(series_1, sampler: sampler);

            Series sample;
            Series expected;
            float len = series_1.VirtualCount - 1f;

            sample = sampler.GetValueAtT(series_1, 0);
            expected = new FloatSeries(2, new[] { 150f, 150f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 9 / len);
            expected = new FloatSeries(2, new[] { 650f, 150f });
            //Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 10 / len);
            expected = new FloatSeries(2, new[] { 177.7777f, 166.0375f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 20 / len);
            expected = new FloatSeries(2, new[] { 150f, 182.075f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 60 / len);
            expected = new FloatSeries(2, new[] { 150f, 246.225f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

        }

        public void VisualSampleByIndex(HexagonSampler sampler)
        {
            var startStore = new Store(series_1, sampler: sampler);

            Series sample;
            Series expected;
            sample = sampler.GetValueAtIndex(series_1, 0);
            expected = new FloatSeries(2, new[] { 150f, 150f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 9);
            expected = new FloatSeries(2, new[] { 650f, 150f });
            //Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 10);
            expected = new FloatSeries(2, new[] { 177.7777f, 166.0375f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 20);
            expected = new FloatSeries(2, new[] { 150f, 182.075f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 60);
            expected = new FloatSeries(2, new[] { 150f, 246.225f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));


        }

        public void HexagonSamplerByT_1()
        {
            int[] strides = { 10, 0 };
            Series sample;
            Series expected;
            float len = series_1.VirtualCount - 1f;
            HexagonSampler sampler = new HexagonSampler(strides);

            // odd rows are offset to make hex grid, by 5.555 in this case.
            sample = sampler.GetValueAtT(series_1, -10 / len);
            expected = new FloatSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 0);
            expected = new FloatSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 1/len);
            expected = new FloatSeries(2, new[] { 11.11111f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 9 / len);
            expected = new FloatSeries(2, new[] { 100f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 10 / len);
            expected = new FloatSeries(2, new[] { 5.55555555f, 22.222222f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 11 / len);
            expected = new FloatSeries(2, new[] { 16.66666f, 22.222222f });

            sample = sampler.GetValueAtT(series_1, 20 / len);
            expected = new FloatSeries(2, new[] { 0f, 44.444444f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 21 / len);
            expected = new FloatSeries(2, new[] { 11.11111f, 44.444444f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 44 / len);
            expected = new FloatSeries(2, new[] { 44.44444f, 88.88888f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 55 / len);
            expected = new FloatSeries(2, new[] { 61.11111f, 111.11111f }); 
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 89 / len);
            expected = new FloatSeries(2, new[] { 100f, 177.77777f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 99 / len);
            expected = new FloatSeries(2, new[] { 105.555555f, 200f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtT(series_1, 999 / len);
            expected = new FloatSeries(2, new[] { 105.555555f, 200f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

        }
        public void HexagonSamplerByIndex_1()
        {
            int[] strides = { 10, 0 };
            Series sample;
            Series expected;
            HexagonSampler sampler = new HexagonSampler(strides);

            // odd rows are offset to make hex grid, by 5.555 in this case.
            sample = sampler.GetValueAtIndex(series_1, -10);
            expected = new FloatSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 0);
            expected = new FloatSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 1);
            expected = new FloatSeries(2, new[] { 11.11111f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 9);
            expected = new FloatSeries(2, new[] { 100f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 10);
            expected = new FloatSeries(2, new[] { 5.55555555f, 22.222222f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 11);
            expected = new FloatSeries(2, new[] { 16.66666f, 22.222222f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 20);
            expected = new FloatSeries(2, new[] { 0f, 44.444444f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 21);
            expected = new FloatSeries(2, new[] { 11.11111f, 44.444444f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 44);
            expected = new FloatSeries(2, new[] { 44.44444f, 88.88888f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 89);
            expected = new FloatSeries(2, new[] { 100f, 177.77777f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 99);
            expected = new FloatSeries(2, new[] { 105.555555f, 200f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetValueAtIndex(series_1, 999);
            expected = new FloatSeries(2, new[] { 105.555555f, 200f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

        }
    }
}
