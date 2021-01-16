using Motive.SeriesData;

namespace Motive.Vis
{
	public enum LinearDirection { Centered, Horizontal, Vertical, TLDiagonal, TRDiagonal }

    public interface IPrimitive// : ISeries
	{
		float Similarity(IPrimitive p);
        //Point Sample(Gaussian g);
	}

	public interface IPath
	{
		float Length { get; }
		Point StartPoint { get; }
		Point MidPoint { get; }
		Point EndPoint { get; }

		Point GetPoint(float position, float offset = 0);
		Point GetPointFromCenter(float centeredPosition, float offset = 0);

		Node NodeAt(float position, float offset = 0);
		Node StartNode { get; }
		Node MidNode { get; }
		Node EndNode { get; }
	}

    public interface IPrimitivePath : IPath
	{
		Point[] GetPolylinePoints(int pointCount = 24);
    }


    public interface IArea
    {
	    Point Center { get; }
	    float Area { get; }
	    Quad Bounds { get; }
	    bool IsClosed { get; }
	    bool IsConcave { get; }
	    int JointCount { get; }
	    int CornerCount { get; }
	    float Sharpness { get; }
    }
}