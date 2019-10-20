using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
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
    }
}
