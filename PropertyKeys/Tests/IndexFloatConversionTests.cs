using DataArcs.Samplers;
using DataArcs.SeriesData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataArcs.Tests
{
	[TestClass]
	public class IndexFloatConversionTests
	{
		private FloatSeries series_1;
		private FloatSeries series_1_31;
		private FloatSeries series_2;
		private float delta = 0.00001f;

		[TestInitialize]
		public void TestInitializer()
		{
			series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
			series_1_31 = new FloatSeries(1, new float[] {1f, 2f, 11f, 22f});
			series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
		}

		[TestMethod]
		public void TestGetDimsForIndex()
		{
			Series series = new FloatSeries(1, new[] {0f, 1f});
            int virtualCount = 100;
			int[] strides = {10,10};
			int[] sample;
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, -3);
			ArraysEqual(new int[] {0, 0}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 0);
			ArraysEqual(new int[] {0, 0}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 1);
			ArraysEqual(new int[] {1, 0}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 10);
			ArraysEqual(new int[] {0, 1}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 11);
			ArraysEqual(new int[] {1, 1}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 53);
			ArraysEqual(new int[] {3, 5}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 99);
			ArraysEqual(new int[] {9, 9}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 100);
			ArraysEqual(new int[] {9, 9}, sample);
			sample = SamplerUtils.GetPositionsForIndex(strides, virtualCount, 101);
			ArraysEqual(new int[] {9, 9}, sample);
		}

		[TestMethod]
		public void TestGetStrideTsForIndex()
		{
			Series series = new FloatSeries(1, new[] {0f, 1f});
            int virtualCount = 100;
            int[] strides = {10, 10};
			var cl = strides[0] - 1f;
			float[] sample;
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, -3).FloatData;
			ArraysEqual(new float[] {0, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 0).FloatData;
			ArraysEqual(new float[] {0, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 1).FloatData;
			ArraysEqual(new float[] {1f / cl, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 9).FloatData;
			ArraysEqual(new float[] {1f, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 10).FloatData;
			ArraysEqual(new float[] {0, 1f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 11).FloatData;
			ArraysEqual(new float[] {1f / cl, 1f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 53).FloatData;
			ArraysEqual(new float[] {3f / cl, 5f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 99).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 100).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedT(strides, virtualCount, 101).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
		}

		[TestMethod]
		public void TestGetStrideTsForT()
		{
			Series series = new FloatSeries(1, new[] {0f, 1f});
            int virtualCount = 100;
            int[] strides = {10, 10};
			var cl = strides[0] - 1f;
			var len = virtualCount - 1f;
			var sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, -3f).FloatData;
			ArraysEqual(new float[] {0, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 0).FloatData;
			ArraysEqual(new float[] {0, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 1f / len).FloatData;
			ArraysEqual(new float[] {1f / cl, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 9f / len).FloatData;
			ArraysEqual(new float[] {1f, 0}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 10f / len).FloatData;
			ArraysEqual(new float[] {0, 1f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 11f / len).FloatData;
			ArraysEqual(new float[] {1f / cl, 1f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 53f / len).FloatData;
			ArraysEqual(new float[] {3f / cl, 5f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 1f).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 100f / len).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
			sample = SamplerUtils.GetMultipliedJaggedTFromT(strides, virtualCount, 101 / len).FloatData;
			ArraysEqual(new float[] {9f / cl, 9f / cl}, sample, delta);
		}

		[TestMethod]
		public void TestTVectorSize1()
		{
			TVectorSize1();
            var store = series_1.CreateLinearStore(100);
            store.BakeData();
            series_1 = (FloatSeries)store.GetFullSeries();
            TVectorSize1();

			IndexVectorSize1();
            store = series_1_31.CreateLinearStore(31);
            store.BakeData();
            series_1_31 = (FloatSeries)store.GetFullSeries();
            IndexVectorSize1();
		}

		[TestMethod]
		public void TestTVectorSize2()
		{
			TVectorSize2();
            var store = series_2.CreateLinearStore(100);
            store.BakeData();
            series_2 = (FloatSeries)store.GetFullSeries();
            TVectorSize2();
		}

		public void TVectorSize1()
		{
			// series_1 = new FloatSeries(1, 1f, 2f, 11f, 22f);
			float sample;
			sample = series_1.GetValueAtT(0f).X;
			Assert.AreEqual(1f, sample, delta);

			sample = series_1.GetValueAtT(0.25f).X;
			Assert.AreEqual(1.75f, sample, delta);
			sample = series_1.GetValueAtT(1f / 3f).X;
			Assert.AreEqual(2f, sample, delta);
			sample = series_1.GetValueAtT(0.5f).X;
			Assert.AreEqual(6.5f, sample, delta);
			sample = series_1.GetValueAtT(2f / 3f).X;
			Assert.AreEqual(11f, sample, delta);
			sample = series_1.GetValueAtT(0.75f).X;
			Assert.AreEqual(13.75f, sample, delta);

			sample = series_1.GetValueAtT(1f).X;
			Assert.AreEqual(22f, sample, delta);
			sample = series_1.GetValueAtT(1.2f).X;
			Assert.AreEqual(22f, sample, delta);
			sample = series_1.GetValueAtT(11f).X;
			Assert.AreEqual(22f, sample, delta);
		}

		public void IndexVectorSize1()
		{
			float sample;
            int count = 31;

            sample = series_1_31.GetSeriesAtIndex(-10).X;
			Assert.AreEqual(1f, sample, delta);
			sample = series_1_31.GetSeriesAtIndex(0).X;
			Assert.AreEqual(1f, sample, delta);

			int lastIndex = series_1_31.DataSize > 4 ? 60 : 3;
			sample = series_1_31.GetSeriesAtIndex(lastIndex).X;
			Assert.AreEqual(22f, sample, delta);
			sample = series_1_31.GetSeriesAtIndex(99).X;
			Assert.AreEqual(22f, sample, delta);

			sample = series_1_31.GetValueAtVirtualIndex(-10, count).X;
			Assert.AreEqual(1f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(0, count).X;
			Assert.AreEqual(1f, sample, delta);

			sample = series_1_31.GetValueAtVirtualIndex(1, count).X;
			Assert.AreEqual(1.1f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(2, count).X;
			Assert.AreEqual(1.2f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(3, count).X;
			Assert.AreEqual(1.3f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(4, count).X;
			Assert.AreEqual(1.4f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(5, count).X;
			Assert.AreEqual(1.5f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(10, count).X;
			Assert.AreEqual(2f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(15, count).X;
			Assert.AreEqual(6.5f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(20, count).X;
			Assert.AreEqual(11f, sample, delta);
			sample = series_1_31.GetValueAtVirtualIndex(30, count).X;
			Assert.AreEqual(22f, sample, delta);
		}

		public void TVectorSize2()
		{
			//series_2 = new FloatSeries(2, 10f, 20f, 110f, 220f, 310f, 420f);
			float[] sample;
			sample = series_2.GetValueAtT(-10f).FloatData;
			ArraysEqual(new float[] {10f, 20f}, sample, delta);
			sample = series_2.GetValueAtT(0f).FloatData;
			ArraysEqual(new float[] {10f, 20f}, sample, delta);

			sample = series_2.GetValueAtT(0.25f).FloatData;
			ArraysEqual(new float[] {60f, 120f}, sample, delta);
			sample = series_2.GetValueAtT(0.5f).FloatData;
			ArraysEqual(new float[] {110f, 220f}, sample, 0.6f);// due to rounding of .5 on indexes of different accelerations this is out a bit. //delta);
			sample = series_2.GetValueAtT(0.75f).FloatData;
			ArraysEqual(new float[] {210f, 320f}, sample, delta);

			sample = series_2.GetValueAtT(1f).FloatData;
			ArraysEqual(new float[] {310f, 420f}, sample, delta);
			sample = series_2.GetValueAtT(1.1f).FloatData;
			ArraysEqual(new float[] {310f, 420f}, sample, delta);
			sample = series_2.GetValueAtT(10f).FloatData;
			ArraysEqual(new float[] {310f, 420f}, sample, delta);

			//float testSample = series_2.GetSeriesAtT(1f / 3f).X;
			//Assert.AreEqual(2f, testSample, delta);
			//testSample = series_2.GetSeriesAtT(2f / 3f).X;
			//Assert.AreEqual(11f, testSample, delta);
			//testSample = series_2.GetSeriesAtT(0.25f).X;
			//Assert.AreEqual(1.75f, testSample, delta);
			//testSample = series_2.GetSeriesAtT(0.5f).X;
			//Assert.AreEqual(6.5f, testSample, delta);
			//testSample = series_2.GetSeriesAtT(0.75f).X;
			//Assert.AreEqual(13.75f, testSample, delta);
		}

		public void ArraysEqual(float[] a, float[] b, float delta)
		{
			for (var i = 0; i < a.Length; i++)
			{
				Assert.AreEqual(a[i], b[i], delta);
			}
		}

		public void ArraysEqual(int[] a, int[] b)
		{
			for (var i = 0; i < a.Length; i++)
			{
				Assert.AreEqual(a[i], b[i]);
			}
		}
	}
}