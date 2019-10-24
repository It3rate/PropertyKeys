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
    public class RingSamplerTests
    {
	    private RingSampler sampler1;
	    private RingSampler sampler2;
        private float delta = SamplerUtils.TOLERANCE;

        [TestInitialize]
	    public void TestInitializer()
	    {
		    sampler1 = new RingSampler(new int[] { 7 });
		    sampler2 = new RingSampler(new int[] { 6, 4 });
        }

	    [TestMethod]
	    public void TestRingSampler1()
	    {
		    Series sample;
		    Series expected;

		    RingSampler sampler = sampler1;
		    int expectedCapacity = 7;
		    float len = expectedCapacity - 1f;

            sample = sampler.GetSampledTs(new ParametricSeries(1, -1));
		    expected = new ParametricSeries(2, new[] { 0f, 0f });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 0));
		    expected = new ParametricSeries(2, new[] { 0f, 0f });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 2f / len));
		    expected = new ParametricSeries(2, new[] { 0f, 2f / expectedCapacity });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 0.5f)); // round down
		    expected = new ParametricSeries(2, new[] { 0f, 3f / expectedCapacity });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 1));
		    expected = new ParametricSeries(2, new[] { 0f, 6f / expectedCapacity });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 2));
		    expected = new ParametricSeries(2, new[] { 0f, 6f / expectedCapacity });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));


		    RingSampler evenSampler = new RingSampler(new int[] { 8 });
		    sample = evenSampler.GetSampledTs(new ParametricSeries(1, 0.5f));
		    expected = new ParametricSeries(2, new[] { 0f, 4f / 8 });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

	    }
	    [TestMethod]
	    public void TestRingSampler2()
	    {
		    Series sample;
		    Series expected;

		    RingSampler sampler = sampler2;
		    float ringCount = 2f;
		    int outer = 6;
            int inner = 4;
		    int expectedCapacity = outer + inner; // 10
		    float len = expectedCapacity - 1f;

            sample = sampler.GetSampledTs(new ParametricSeries(1, -1));
		    expected = new ParametricSeries(2, new[] { 0f, 0f });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 0));
		    expected = new ParametricSeries(2, new[] { 0f, 0f });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 1f / len));
		    expected = new ParametricSeries(2, new[] { 0f, 1f / outer }); // ring zero, 1/6th along
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 2f / len));
		    expected = new ParametricSeries(2, new[] { 0f, 2f / outer });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetSampledTs(new ParametricSeries(1, 0.5f));
		    expected = new ParametricSeries(2, new[] { 0f, 5f / outer });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 6f / len));
		    expected = new ParametricSeries(2, new[] { 1 / ringCount, 0f });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 7f / len));
		    expected = new ParametricSeries(2, new[] { 1 / ringCount, 1f / inner });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 9f / len));
		    expected = new ParametricSeries(2, new[] { 1 / ringCount, 3f / inner });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

            sample = sampler.GetSampledTs(new ParametricSeries(1, 1));
		    expected = new ParametricSeries(2, new[] { 1 / ringCount, 3f / inner });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

		    sample = sampler.GetSampledTs(new ParametricSeries(1, 2));
		    expected = new ParametricSeries(2, new[] { 1 / ringCount, 3f / inner });
		    Assert.IsTrue(SeriesUtils.IsEqual(sample, expected));

	    }

    }
}
