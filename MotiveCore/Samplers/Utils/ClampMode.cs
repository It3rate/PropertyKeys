using System;

namespace Motive.Samplers
{
	public enum ClampMode
	{
        None, // Index lookups will still be Clamped to avoid out of bounds errors.
		Clamp, // 0..0..1..1..1.. 0001222
		Wrap, // 0..1->0..1.. 012012012012
		ReverseWrap, // 1..0->1..0.. 210210210210
        Mirror, // 0..1,1..0,0..1,1.. 012210012210
		Bounce, // 0121012
	}

	public static class ClampModeExtension
	{
		public static int GetClampedValue(this ClampMode clampMode, int index, int count)
		{
			int result = index;
			if (index < 0 || index >= count)
			{
				int mod = index % count;
				switch (clampMode)
				{
					case ClampMode.Wrap:
						result = mod;
						break;
					case ClampMode.ReverseWrap:
						result = (count - 1) - mod;
						break;
                    case ClampMode.Mirror:
						result = ((index / count) & 1) == 0 ? mod : (count - 1) - mod;
						break;
					case ClampMode.Clamp:
						result = Math.Min(count - 1, Math.Max(0, index));
                        break;
					case ClampMode.Bounce:
						var isUp = ((index / (count - 1)) & 1) == 0;
						mod = index % (count - 1);
                        result = isUp? mod : (count - 1) - mod;
						break;
                }
			}

			return result;
		}
	}
}