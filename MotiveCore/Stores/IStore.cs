using System;
using System.Collections;
using Motive.Components.Libraries;
using Motive.Samplers.Utils;
using Motive.SeriesData;

namespace Motive.Stores
{
	public interface IStore : IEnumerable, IDefinition
    {
	    //int Id { get; }
        //void Update(double currentTime, double deltaTime);

        /// <summary>
        /// Capacity is normally just the Sampler's capacity, but may need adjustment eg when merging or blending two stores.
        /// </summary>
        int Capacity { get; }
        CombineFunction CombineFunction { get; set; }
		Sampler Sampler { get; set; }
		bool IsBaked { get; set; }

        ISeries GetSeriesRef();
        void SetFullSeries(ISeries value);
		
		ISeries GetValuesAtT(float t);
        ParametricSeries GetSampledTs(ParametricSeries seriesT);
        ISeries GetNeighbors(int index, bool wrapEdges = true);

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

        public object Current => _instance.GetValuesAtT(_position / (_instance.Capacity - 1f));

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

        ModifyT, // special case where the input T is modified, rather than combining with the previous store.
	}
}