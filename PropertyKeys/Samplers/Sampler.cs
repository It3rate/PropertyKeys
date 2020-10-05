using System;
using System.Linq;
using System.Xml.XPath;
using DataArcs.Components.Libraries;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
	// Sampler needs refactoring. Currently inputs can be int index, float t, Parametric t mods t, and Parametric gets result.
	// Instead, this can probably just be a t modifier, and t is *always* a floatSeries (no parametric needed).
	// A grid would be two samplers, on to split t into two values, and one just a linear scalar to the frame size.
	// (or maybe even the frame size scalar is just done on the renderer?).
	// Strides become 'dimensions', essentially just a way to express capacity (product, sum or continuous (0-1) style).
	// Continuous style requires an item list (or assumes no items), as there can't be virtual items.
	// Neighbors may need to move to Container, or at least the items element would need to be passed.

	// It seems composites should be a specific type of store (items/children are series, t division is sampler),
	// and stores should have properties like composites do, used for internal clocks, variables, scalars etc).

	// Can stores know how to ID and use their own properties? Ideally can get rid of PropertyIds as constant, 
	// and they become self contained modifiers. Maybe.
	// Counter: Location has many possible calculations independent of final use.
	// So a GridStore calculates a grid, but goes on a location property, which is universal?
	// Or location property is only known to things with locations via some interface?
	// Store types have a list of required properties, ideally enforced in ctor, by type even.
	// Required properties could be mandated in ctor and available via getter and store type, on interface.

	// series is always a matrix?

	public abstract class Sampler : IDefinition
	{
        public string Name { get; set; }
		public int Id { get; private set; }

        public int SampleCount { get; protected set; } = 1;
		public Slot[] SwizzleMap { get; set; }
		
		public int[] Strides { get; protected set; }

		private GrowthType _growthType;
		public GrowthType GrowthType
		{
			get => _growthType;
			set
			{
				_growthType = value;
				SampleCount = StridesToSampleCount(Strides);
			}
		}

		public ClampType[] ClampTypes { get; protected set; }
		public AlignmentType[] AlignmentTypes { get; protected set; }

        protected Sampler(Slot[] swizzleMap = null, int sampleCount = 1)
		{
			Player.CurrentSamplers.AddToLibrary(this);

            SwizzleMap = swizzleMap;
			SampleCount = sampleCount;
			Strides = new []{1};
			ClampTypes = new []{ClampType.None};
			AlignmentTypes = new []{AlignmentType.Left};
			_growthType = GrowthType.Product;
		}

		protected Sampler GetSamplerById(int id) => Player.CurrentSamplers[id];

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
	        int result = 0;
	        switch (GrowthType)
	        {
		        case GrowthType.Product:
			        result = strides.Aggregate(1, (a, b) => b != 0 ? a * b : a);
			        break;
		        case GrowthType.Widest:
			        result = Strides.Max() * Strides.Length;
			        break;
		        case GrowthType.Sum:
			        result = Strides.Sum();
			        break;
	        }
	        return result;
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