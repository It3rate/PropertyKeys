using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.SeriesData.Utils
{
	public enum Slot
	{
		All = -1,
		X = 0,
        Y = 1,
		Z = 2,
		W = 3,
		R = 0,
		G = 1,
		B = 2,
		A = 3,
		S0 = 0,
		S1 = 1,
		S2 = 2,
		S3 = 3,
		S4 = 4,
		S5 = 5,
		S6 = 6,
		S7 = 7,
		S8 = 8,
		S9 = 9,


		Combinatorial = 1000,
		Sum,
		Average,
		Max,
		Min,
		Count,
		Clamp01,
	}

    public class SlotUtils
	{
		public static readonly Slot[] X = new Slot[] { Slot.X };
		public static readonly Slot[] Y = new Slot[] { Slot.Y };
		public static readonly Slot[] Z = new Slot[] { Slot.Z };
		public static readonly Slot[] W = new Slot[] { Slot.W };
		public static readonly Slot[] XY = new Slot[] { Slot.X, Slot.Y };
		public static readonly Slot[] YX = new Slot[] { Slot.Y, Slot.X };
		public static readonly Slot[] XZ = new Slot[] { Slot.X, Slot.Z };
		public static readonly Slot[] ZX = new Slot[] { Slot.Z, Slot.X };
		public static readonly Slot[] YZ = new Slot[] { Slot.Y, Slot.Z };
		public static readonly Slot[] ZY = new Slot[] { Slot.Z, Slot.Y };
		public static readonly Slot[] XYZ = new Slot[] { Slot.X, Slot.Y, Slot.Z };
		public static readonly Slot[] XYZW = new Slot[] { Slot.X, Slot.Y, Slot.Z, Slot.W };
		public static readonly Slot[] ZYX = new Slot[] { Slot.Z, Slot.Y, Slot.X };
		public static readonly Slot[] WZYX = new Slot[] { Slot.W, Slot.Z, Slot.Y, Slot.Z };
		public static readonly Slot[] RGB = new Slot[] { Slot.R, Slot.G, Slot.B };
		public static readonly Slot[] RGBA = new Slot[] { Slot.R, Slot.G, Slot.B, Slot.A };
		public static readonly Slot[] ARGB = new Slot[] { Slot.A, Slot.R, Slot.G, Slot.B };

		public static readonly Slot[] Sum = new Slot[] { Slot.Sum };
		public static readonly Slot[] Average = new Slot[] { Slot.Average };
		public static readonly Slot[] Max = new Slot[] { Slot.Max };
		public static readonly Slot[] Min = new Slot[] { Slot.Min };
		public static readonly Slot[] Count = new Slot[] { Slot.Count };
		public static readonly Slot[] Clamp01 = new Slot[] { Slot.Clamp01 };
		public static readonly Slot[] All = new Slot[] { Slot.All };

        public static float GetFloatAt(Series series, Slot slot)
		{
			float result;
			if (slot < Slot.Combinatorial)
			{
				result = series.FloatDataAt((int)slot);
			}
			else
			{
				var floats = series.GetRawDataAt(0).FloatDataRef;
				switch (slot)
				{
					case Slot.Sum:
						result = floats.Sum();
						break;
					case Slot.Average:
						result = floats.Average();
						break;
					case Slot.Max:
						result = floats.Max();
						break;
					case Slot.Min:
						result = floats.Min();
						break;
					case Slot.Count:
						result = floats.Length;
						break;
					case Slot.Clamp01:
						result = Math.Max(0, Math.Min(1f, floats.Max()));
						break;
					default:
						result = floats.Last();
						break;
				}
			}
			return result;
		}

		public static int GetIntAt(Series series, Slot slot)
		{
			int result;
			if (slot < Slot.Combinatorial)
			{
				int index = Math.Max(0, Math.Min(series.Count, (int)slot));
				result = series.IntDataAt(index);
			}
			else
			{
				var ints = series.GetRawDataAt(0).IntDataRef;
				switch (slot)
				{
					case Slot.Sum:
						result = ints.Sum();
						break;
					case Slot.Average:
						result = (int)(ints.Sum() / (float)ints.Length);
						break;
					case Slot.Max:
						result = ints.Max();
						break;
					case Slot.Min:
						result = ints.Min();
						break;
					case Slot.Count:
						result = ints.Length;
						break;
					case Slot.Clamp01:
						result = Math.Max(0, Math.Min(1, ints.Max()));
						break;
					default:
						result = ints.Last();
						break;
				}
			}
			return result;
		}
	}
}
