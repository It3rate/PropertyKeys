using System;
using DataArcs.SeriesData;
using System.Collections;

namespace DataArcs.Stores
{
	public interface IStore : IEnumerable
	{
		int StoreId { get; }
        CombineFunction CombineFunction { get; set; }
		CombineTarget CombineTarget { get; set; }
		int VirtualCount { get; set; }

		Series GetFullSeries(int index);
		Series GetSeriesAtIndex(int index, int virtualCount = -1);
		Series GetSeriesAtT(float t, int virtualCount = -1);
        ParametricSeries GetSampledT(float t);

        void Update(float deltaTime);
		void ResetData();
		void HardenToData();

    }

    public class IStoreEnumerator : IEnumerator
    {
        private readonly IStore _instance;
        private int _position = -1;
        public IStoreEnumerator(IStore instance)
        {
            _instance = instance;
        }
        public bool MoveNext()
        {
            _position++;
            return (_position < _instance.VirtualCount);
        }

        public object Current => _instance.GetSeriesAtIndex(_position);

        public void Reset()
        {
            _position = 0;
        }
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
        ContinuousAdd,

        MultiplyT,
	}

	public enum CombineTarget
	{
		T,
        Destination,
        ContinuousSelf,
    }
}