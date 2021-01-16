using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motive.Vis
{
        // Center is a node, that has be successfully queried from another object, or is virtual (exterior, unset until usage) in the case of scaffolding.
        // Everything but center is a 'goal' value and may not be possible on usage, thus must always query properties.

        // Quad is a 4 sided rect, can be diamond, skewed etc, but sides are parallel.
        // Define using center, and a width and height radius. Always normalized and positive, so aspect is w/h.
        // Horz/vert flip is assumed false, may be specified (is that a negative radius? probably not)
        // Object rotation is assumed zero, but may be specified.
        // Angle of each side is assumed horz and vert, but skew may be specified (by using angle of bottom left corner? H or V offset? Or maybe just diamond or square?)

        // Center is a Store Point.
        // It is virtually a path around the quad, starting at top left, moving clockwise.
        // Getting top line returns a Store with a PathSampler and range [0, 0.25] (uses quad itself as ref)
        // Getting bottom line, left to right returns a range [0.75, 0.50]
        // PathSampler needs to be able to work in virtual lines, or total distance (in which case the series isn't divided into quarters for line segments)

	/// <summary>
    /// A rectangle defined by a center and corner (can not be rotated or skewed). This isn't a  path, but it's points and lines can be used for reference.
    /// For convenience it can be turned into four strokes if a box made of strokes is desired (though you may want more control over the stroke order).
    /// </summary>
    public class Quad : Point
    {
        // fusion: 2 point or center point,wh
        public Point Center => this;
        public Point TopLeft { get; private set; }

        public Point HalfSize { get; private set; }
        public Point Dimensions => HalfSize.Multiply(2f);

        public Quad(Point center, Point corner) : base(center.X, center.Y)
        {
            Initialize(corner.X, corner.Y);
        }
        public Quad(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
            Initialize(cornerX, cornerY);
        }
        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new Point(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = this.Subtract(TopLeft).Abs();
        }

        public Point GetPoint(float xRatio, float yRatio)
        {
            return new Point(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
        }


        public Line GetLine(CompassDirection direction, float offset = 0)
        {
            return direction.GetLineFrom(this, offset);
        }
        public Point GetPoint(CompassDirection direction)
        {
            return direction.GetPointFrom(this);
        }

        public Point NearestIntersectionTo(Point p) => null;
        public bool IntersectsWith(Point p) => false;
        public bool IntersectsWith(Line line) => Math.Abs(Center.X - line.Center.X) <= HalfSize.X + line.MidPoint.X && Math.Abs(Center.Y - line.Center.Y) <= HalfSize.Y + line.MidPoint.Y;
        public bool IntersectsWith(Quad rect) => Math.Abs(Center.X - rect.Center.X) <= HalfSize.X + rect.HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) <= HalfSize.Y + rect.HalfSize.Y;
        public bool Contains(Point p) => false;
        public bool Contains(Line line) => false;
        public bool Contains(Quad rect) => Math.Abs(Center.X - rect.Center.X) + rect.HalfSize.X <= HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) + rect.HalfSize.Y <= HalfSize.Y;



        public override string ToString()
        {
            return $"Rect:{TopLeft.X:0.##},{TopLeft.Y:0.##} {Dimensions.X:0.##},{Dimensions.Y:0.##}";
        }
    }
}
