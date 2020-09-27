﻿using System;
using System.Linq;
using DataArcs.Components.Libraries;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
	public abstract class Sampler : IDefinition
	{
        public string Name { get; set; }
		public int Id { get; private set; }

        public int SampleCount { get; protected set; } = 1;
		public Slot[] SwizzleMap { get; set; }
		
		public int[] Strides { get; protected set; }
		
		public GrowthType GrowthType { get; protected set; }
		public ClampType[] ClampType { get; protected set; }
		public AlignmentType[] AlignmentType { get; protected set; }

        protected Sampler(Slot[] swizzleMap = null, int sampleCount = 1)
		{
			Player.CurrentSamplers.AddToLibrary(this);

            SwizzleMap = swizzleMap;
			SampleCount = sampleCount;
			Strides = new int[SampleCount];
			ClampType = new ClampType[SampleCount];
			AlignmentType = new AlignmentType[SampleCount];
			GrowthType = GrowthType.Product;
		}

		protected Sampler GetSamplerById(int id) => Player.CurrentSamplers[id];

        public virtual Series GetValueAtIndex(Series series, int index)
        {
	        var indexT = SamplerUtils.TFromIndex(SampleCount, index); // index / (SampleCount - 1f);
            return GetValuesAtT(series, indexT);
        }

        public virtual Series GetValuesAtT(Series series, float t)
        {	
	        var seriesT = GetSampledTs(new ParametricSeries(1, t));
	        return GetSeriesSample(series, seriesT);
        }

		// todo: Why is this in sample? Needs to move to series, and a rect vs grid series can use different algorithms to generate values (needed).
		// counter: only samplers know about sampleCount. CurrentSeries only knows it's own count, not the virtual count it represents.
		// counter counter: Why do samplers care about sampleCount? A 10x20 sampler should be able to handle a series with 1000 elements, or [200,400]/1000 elements on a page.
		// ans: A grid knows it's max size by strides. Need an infinite scroll mode where sampleCount is read from series, but also want (more common) option to set size from sampler.
        public virtual Series GetSeriesSample(Series series, ParametricSeries seriesT)
        {
            var result = ArrayExtension.GetFloatZeroArray(series.VectorSize);
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = series.GetVirtualValueAt(seriesT[i]).FloatDataAt(i);
            }
            return SeriesUtils.CreateSeriesOfType(series, result);
        }
        public virtual ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            return Swizzle(seriesT, seriesT);
        }

        public virtual int NeighborCount => 2;
		private int WrappedIndex(int index, int capacity) => index >= capacity ? 0 : index < 0 ? capacity - 1 : index;
        public virtual Series GetNeighbors(Series series, int index, bool wrapEdges = true)
        {
	        var outLen = SwizzleMap?.Length ?? series.VectorSize;
            var result = SeriesUtils.CreateSeriesOfType(series, new float[outLen * NeighborCount], outLen);
	        result.SetRawDataAt(0, series.GetVirtualValueAt(WrappedIndex(index - 1, SampleCount), SampleCount));
	        result.SetRawDataAt(1, series.GetVirtualValueAt(WrappedIndex(index + 1, SampleCount), SampleCount));
            return result;
        }

        /// <summary>
        /// Generate a result with values mapped according the internal SwizzleMap.
        /// Extra values can be preserved if the original series is longer than the source, and the swizzle map calls for them.
        /// </summary>
        /// <param name="source">The series to be mapped using the SwizzleMap.</param>
        /// <param name="original">The original unmodified values that could be used if the SwizzleMap asks for slots out of the source range.</param>
        /// <returns></returns>
        public ParametricSeries Swizzle(ParametricSeries source, ParametricSeries original)
        {
	        ParametricSeries result = source;
	        if (SwizzleMap != null)
            {
                int len = SwizzleMap.Length;
                result = new ParametricSeries(len, new float[len]);
		        for (int i = 0; i < len; i++)
		        {
			        int index = (int)SwizzleMap[i];
			        result[i] = index < source.VectorSize ? source[index] : index < original.VectorSize ? original[index] : source[len - 1];
		        }
	        }
            else if(source.VectorSize < original.VectorSize)
            {
                // No slots, but don't destroy data in the original source if it isn't overwritten
                result = (ParametricSeries)original.Copy();
                for (int i = 0; i < source.VectorSize; i++)
                {
                    result[i] = source[i];
                }
            }

	        return result;
        }

        protected int StridesToSampleCount(int[] strides)
        {
	        return (GrowthType == GrowthType.Sum) ? strides.Sum() : strides.Aggregate(1, (a, b) => b != 0 ? a * b : a);
        }

        public IntSeries GetBakedStrideIndexes()
        {
            int strideLen = Strides.Length;
            var result = new IntSeries(strideLen, new int[SampleCount * strideLen]);
            int capacity = SampleCount;
            
            var t = new ParametricSeries(1, 0);
            for (int i = 0; i < capacity; i++)
            {
                t.FloatDataRef[0] = i / (float)capacity;
                var ts = GetSampledTs(t);
                for (int j = 0; j < ts.VectorSize; j++)
                {
                    result.IntDataRef[i * strideLen + j] = (int)(ts.FloatDataAt(j) * Strides[j]);
                }
            }

            return result;
        }

        public bool AssignIdIfUnset(int id)
        {
	        bool result = false;
	        if (Id == 0 && id > 0)
	        {
		        Id = id;
		        result = true;
	        }
	        return result;
        }
        public void Update(double currentTime, double deltaTime)
        {
        }
        public void OnActivate()
        {
        }
        public void OnDeactivate()
        {
        }
	}
}