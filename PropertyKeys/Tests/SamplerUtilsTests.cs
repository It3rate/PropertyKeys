using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataArcs.Tests
{
    [TestClass]
    public class SamplerUtilsTests
    {
        private float delta = SamplerUtils.TOLERANCE;

        [TestInitialize]
        public void TestInitializer()
        {
        }

        [TestMethod]
        public void TestIndexAndRemainder1()
        {
            int index;
            float remainder;

            SamplerUtils.InterpolatedIndexAndRemainder(1, 0.5f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(2, -1f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(2, 0f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(2, 0.5f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0.5f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(2, 1f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(1f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(2, 2f, out index, out remainder); // last element can sample larger than len for overflow
            Assert.AreEqual(0, index);
            Assert.AreEqual(2f, remainder, delta);
        }

        [TestMethod]
        public void TestIndexAndRemainder2()
        {
            int index;
            float remainder;
            SamplerUtils.InterpolatedIndexAndRemainder(13, 0.5f, out index, out remainder);
            Assert.AreEqual(6, index);
            Assert.AreEqual(0, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(14, 0.5f, out index, out remainder);
            Assert.AreEqual(6, index);
            Assert.AreEqual(0.5f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(12, -1f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(12, 0f, out index, out remainder);
            Assert.AreEqual(0, index);
            Assert.AreEqual(0, remainder, delta);

            // returns zero based index to sample forward from, so 0-11 measure...
            SamplerUtils.InterpolatedIndexAndRemainder(12, 1f / 11, out index, out remainder);
            Assert.AreEqual(1, index);
            Assert.AreEqual(0, remainder, delta);

            // ...and result index will always be 0 - 10
            SamplerUtils.InterpolatedIndexAndRemainder(12, 11f / 11, out index, out remainder);
            Assert.AreEqual(10, index);
            Assert.AreEqual(1f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(12, 5.4f / 11, out index, out remainder);
            Assert.AreEqual(5, index);
            Assert.AreEqual(0.4f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(12, 9.3f / 11, out index, out remainder);
            Assert.AreEqual(9, index);
            Assert.AreEqual(0.3f, remainder, delta);

            SamplerUtils.InterpolatedIndexAndRemainder(12, 2f, out index, out remainder); // last element can sample larger than len for overflow
            Assert.AreEqual(10, index);
            Assert.AreEqual(12f, remainder, delta);
        }
        [TestMethod]
        public void TestGetSummedJaggedT()
        {
            Series sample;
            Series expected;

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 0);
            expected = new ParametricSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 1);
            expected = new ParametricSeries(2, new[] { 0f, 1f / 6 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 6);
            expected = new ParametricSeries(2, new[] { 0f, 6f / 6 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 7);
            expected = new ParametricSeries(2, new[] { 1f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 15);
            expected = new ParametricSeries(2, new[] { 2f / 4, 1f / 6 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 27);
            expected = new ParametricSeries(2, new[] { 3f / 4, 4f / 4 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 28);
            expected = new ParametricSeries(2, new[] { 4f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 29);
            expected = new ParametricSeries(2, new[] { 4f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));
        }
        [TestMethod]
        public void TestGetSummedJaggedTDiscrete()
        {
            Series sample;
            Series expected;

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 0, true);
            expected = new ParametricSeries(2, new[] { 0f, 0f });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 1, true);
            expected = new ParametricSeries(2, new[] { 0f, 1f / 7 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 6, true);
            expected = new ParametricSeries(2, new[] { 0f, 6f / 7 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 7, true);
            expected = new ParametricSeries(2, new[] { 1f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 15, true);
            expected = new ParametricSeries(2, new[] { 2f / 4, 1f / 7 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 27, true);
            expected = new ParametricSeries(2, new[] { 3f / 4, 6f/7 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 28, true);
            expected = new ParametricSeries(2, new[] { 4f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = SamplerUtils.GetSummedJaggedT(new[] { 7, 7, 7, 7 }, 29, true);
            expected = new ParametricSeries(2, new[] { 4f / 4, 0 });
            Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));
        }
    }
}
