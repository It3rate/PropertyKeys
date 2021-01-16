using System.Collections.Generic;

namespace Motive.Vis
{
	public class Shape
	{
		public float Length => 0;
		public Point Anchor { get; } = null;

		public List<Stroke> Strokes { get; } = new List<Stroke>();

		// computed
		public List<Joint> ComputedJoints { get; } = new List<Joint>();

		public float IsInside(IPath element) => 0;

		public Shape(params Stroke[] strokes)
		{
			Strokes.AddRange(strokes);
		}
    }
}