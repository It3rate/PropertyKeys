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

		InterpolationT,

        SampleAtT,
        SampleAtTCombined,
        EasedT,
        EasedTCombined,

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
		Orientation,
		PointCount,
		Starness,
		Roundness,
		Radius,
		RandomMotion,

		Mouse, // composite of all mouse states in single series, mask with MousePropertiesEnum
		MouseX, // mouse events on mouse composite
		MouseY,
		MouseLocation, // Mouse X and Y 
		MouseLocationT, // Mouse X and Y normalized to container size
		MouseLocationTCombined, // Mouse X and Y normalized to container size
        MouseWheel,
        MouseButtonStates,
        PenPressure,
        PenAngle,
        KeyboardState, // keyboard input events and text on composite

        Automata,

        MpuAcceleration = 500,
        MpuAccelerationX,
        MpuAccelerationY,
        MpuAccelerationZ,

        MpuGyroscope,
        MpuGyroscopeX,
        MpuGyroscopeY,
        MpuGyroscopeZ,

        MpuMagnetometer,
        MpuMagnetometerX,
        MpuMagnetometerY,
        MpuMagnetometerZ,

        MpuTemperature,

        BmeHumidity,
        BmePressure,
        BmeTemperature,
		

        User = 0x1000,
		User1 = 0x1001,
		User2 = 0x1002,
		User3 = 0x1003,
		User4 = 0x1004,
		User5 = 0x1005,
		User6 = 0x1006,
		User7 = 0x1007,
		User8 = 0x1008,
        User9 = 0x1009,
    }
    static class PropertyIdSet
    {
        public static bool IsTSampling(PropertyId propId)
        {
            bool result = false;
            switch (propId)
            {
	            case PropertyId.SampleAtT:
                case PropertyId.SampleAtTCombined:
                case PropertyId.EasedT:
                case PropertyId.EasedTCombined:
	            case PropertyId.MouseLocationT:
	            case PropertyId.MouseLocationTCombined:
                    result = true;
                    break;
            }
            return result;
        }
        public static bool IsTCombining(PropertyId propId)
        {
            bool result = false;
            switch (propId)
            {
	            case PropertyId.MouseLocationTCombined:
                case PropertyId.SampleAtTCombined:
                case PropertyId.EasedTCombined:
                    result = true;
                    break;
            }
            return result;
        }
    }
}
