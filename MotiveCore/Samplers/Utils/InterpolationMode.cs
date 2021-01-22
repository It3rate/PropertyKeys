namespace Motive.Samplers.Utils
{
    /// <summary>
    /// Determines how values are calculated when the queried index/t falls between actual data positions. This will usually be continuous, but some elements require discrete values.
    /// </summary>
    public enum InterpolationMode
	{
		Continuous = 0,
		Floor,
		Ceiling,
		Round,
		Region, // 10%,30%,20%,30%,10% (corner, toMid, mid, toEnd, end)
	}
}