using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Components
{
	public enum PropertyId : int // will change to local render defined, combo of type and property
	{
		None = 0,
		TModifier,

		Items,
		Shape,
		Transform,
		Location,
		Size,
		Scale,
		Rotation,
		FillColor,
		PenColor,
		PenWidth,
		T,
		CurrentT,
        StartTime,
		Duration,
		Easing,
		SampleType,

		Graphic,
		// polyShape
		Orientation,
		PointCount,
		Starness,
		Roundness,
		Radius,
		RandomMotion,

		Custom = 0x1000,
	}
}
