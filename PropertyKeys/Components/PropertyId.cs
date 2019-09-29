﻿using System;
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

		SampleAtT,
		CurrentT,
		EasedT,

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
        StartTime,
		Duration,
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
		Custom1 = 0x1001,
		Custom2 = 0x1002,
		Custom3 = 0x1003,
		Custom4 = 0x1004,
		Custom5 = 0x1005,
		Custom6 = 0x1006,
		Custom7 = 0x1007,
		Custom8 = 0x1008,
		Custom9 = 0x1009,
    }
}
