using System;

namespace Motive.SeriesData.Utils
{
	public enum DiscreteClampMode
	{
		Wrap, // 0..1->0..1.. 012012012012
		Mirror, // 0..1,1..0,0..1,1.. 012210012210
		Clamp, // 0..0..1..1..1.. 0001222
		Bounce, // 0121012
	}

	public static class ClampModeExtension
	{
		public static int GetClampedValue(this DiscreteClampMode clampMode, int index, int count)
		{
			int result = index;
			if (index < 0 || index >= count)
			{
				int mod = index % count;
				switch (clampMode)
				{
					case DiscreteClampMode.Wrap:
						result = mod;
						break;
					case DiscreteClampMode.Mirror:
						result = ((index / count) & 1) == 0 ? mod : (count - 1) - mod;
						break;
					case DiscreteClampMode.Clamp:
						result = Math.Min(count - 1, Math.Max(0, index));
                        break;
					case DiscreteClampMode.Bounce:
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