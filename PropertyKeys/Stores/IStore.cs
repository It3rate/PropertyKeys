using DataArcs.SeriesData;

namespace DataArcs.Stores
{
	public interface IStore
	{
		CombineFunction CombineFunction { get; set; }
		CombineTarget CombineTarget { get; set; }

		int VirtualCount { get; set; }

		Series GetSeries(int index);

		void Reset();
		void Update(float time);
		void HardenToData();

		Series GetSeriesAtIndex(int index, int virtualCount = -1);

		Series GetSeriesAtT(float t, int virtualCount = -1);
		float GetTatT(float t);
	}

	public enum CombineFunction
	{
		Replace,
		Append,
		Add,
		Subtract,
		Multiply,
		Divide,
		Average,
		Interpolate,
		MultiplyT,
	}

	public enum CombineTarget
	{
		T,
		Destination,
	}
}