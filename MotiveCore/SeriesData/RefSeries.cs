using System.Reflection;
using Motive.Stores;

namespace Motive.SeriesData
{
	public class RefSeries // Should all ref elements be stores? Maybe that makes sense given series isn't supposed to have context.
	{
		private IStore Reference;
		private ISeries Position; // positions into each reference start - interpolated if a float series, indexed by element if an int series.

        // path version
        // Reference must be stroke, shape, bezier, or some kind of path to be useful
	}
	public class RefChainSeries
	{
		private IntSeries References; // collection of store indexes, all use the same index if Count is one. Always gets nearest listed index (no int interpolation)
        private ISeries Positions; // positions into each reference start - interpolated if a float series, indexed by element if an int series.
        //                              to incorporate a polyline or arc, either use a path, or multiple Store refs. An arc would be 2 refs of the same circle, and two position.
        private FloatSeries Speeds; // allows setting directions along a path, could be used for easing along path when splitting into short polyline segments.
        private FloatSeries Offsets; // offsets perpendicularly from each position. Needs a distance metric, maybe a key line index?

	}
}