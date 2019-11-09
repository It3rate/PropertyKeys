using System;
using DataArcs.SeriesData;
using System.Collections;
using DataArcs.Samplers;

namespace DataArcs.Stores
{
	public interface IStore : IEnumerable
    {
        int StoreId { get; }
        int Capacity { get; }
        CombineFunction CombineFunction { get; set; }
		CombineTarget CombineTarget { get; set; }
		Sampler Sampler { get; set; }
		bool ShouldIterpolate { get; set; }

        Series GetFullSeries();
        void SetFullSeries(Series value);

        Series GetValuesAtIndex(int index);
		Series GetValuesAtT(float t);
        ParametricSeries GetSampledTs(ParametricSeries seriesT);
        Series GetNeighbors(int index, bool wrapEdges = true);

        void Update(float deltaTime);
		void ResetData();
		void BakeData();
		IStore Clone();
		void CopySeriesDataInto(IStore target);

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
            return (_position < _instance.Capacity);
        }

        public object Current => _instance.GetValuesAtIndex(_position);

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
        SubtractFrom,
        Multiply,
        Divide,
        DivideFrom,
        Average,
		Interpolate,
        ContinuousAdd,
        Final,

        MultiplyT,
	}

	public enum CombineTarget
	{
		T,
        Destination,
        ContinuousSelf,
    }
}