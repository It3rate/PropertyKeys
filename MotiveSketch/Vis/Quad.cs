using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotiveSketch.Vis
{
    public class Quad
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

    }
}
