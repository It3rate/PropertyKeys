using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components.Libraries;
using Motive.Samplers;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public interface ISeries : IEnumerable, IDefinition
	{
        // From IDefinition:
        //string Name { get; set; }
        //int Id { get; }
        //void Update(double currentTime, double deltaTime);
        //void OnActivate();
        //void OnDeactivate();
        //bool AssignIdIfUnset(int id);

        int Count { get; }
		SeriesType Type { get; }
		int VectorSize { get; set; }
		DiscreteClampMode IndexClampMode { get; set; }

        RectFSeries Frame { get; }
        Series Size { get; }

		float X { get; }
		float Y { get; }
		float Z { get; }
		float W { get; }

        int DataSize { get; }
        Series GetSeriesAt(float t);
        Series GetSeriesAt(int index);
        void SetSeriesAt(int index, Series series);
        Series GetVirtualValueAt(float t);
        float FloatValueAt(int index);
        int IntValueAt(int index);
        float[] FloatDataRef { get; }
        int[] IntDataRef { get; }

        void ReverseEachElement();
        void Append(Series series);
        void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
        void InterpolateInto(Series b, float t);
        void InterpolateInto(Series b, ParametricSeries seriesT);

        Store CreateLinearStore(int capacity);
        IStore Store(Sampler sampler = null);
        List<Series> ToList();
        void SetByList(List<Series> items);
        ISeries Copy();

        void ResetData();
        void Map(FloatEquation floatEquation);
        void MapValuesToItemPositions(IntSeries items);
        void MapOrderToItemPositions(IntSeries items);
    }

	public class ISeriesEnumerator : IEnumerator
	{
		private ISeries _instance;
		private int _position = -1;
		public ISeriesEnumerator(ISeries instance)
		{
			_instance = instance;
		}
		public bool MoveNext()
		{
			_position++;
			return (_position < (int)(_instance.DataSize / _instance.VectorSize));
		}
		public object Current => _instance.GetSeriesAt(_position);

		public void Reset()
		{
			_position = 0;
		}
	}


    // experimental

    public interface ISeriesBase : IEnumerable
    {
        SeriesType Type { get; }
        int VectorSize { get; }

        ISeriesBase Copy();
        void Reverse();

        ISeriesElement GetSample(ParametricSeries seriesT);
        float ElementSum(int index = 0);
	    float ElementAverage(int index = 0);
	    float ElementMax(int index = 0);
	    float ElementMin(int index = 0);
    }

    public interface ISeriesFloatElement : ISeriesBase
    {
	    float this[int index] { get; set; }
	    float FloatDataAt(int index);
	    float[] FloatDataRef { get; }

	    float X { get; }
	    float Y { get; }
	    float Z { get; }
	    float W { get; }
    }

    public interface ISeriesIntElement : ISeriesBase
    {
	    int IntDataAt(int index);
	    int[] IntDataRef { get; }

	    int A { get; }
	    int B { get; }
	    int C { get; }
	    int D { get; }
    }


    public interface ISeriesElement : ISeriesFloatElement, ISeriesIntElement
    {
	    void CombineInto(ISeriesElement b, CombineFunction combineFunction, float t = 0);
	    void InterpolateInto(ISeriesElement b, float t);
	    void InterpolateInto(ISeriesElement b, ParametricSeries seriesT);
			
	    ISeriesElement GetZeroSeries();
	    ISeriesElement GetMinSeries();
	    ISeriesElement GetMaxSeries();
	    Series GetZeroSeries(int elements);
    }


    public interface IDimensionedSeries : ISeries
    {
        int Dimensions { get; }
        ISeries GetRawSeriesAt(int index);
        void SetRawSeriesAt(int index, ISeries series);
        //List<CurrentSeries> SeriesList { get; }

        RectFSeries Frame { get; }
        Series Size { get; }

    }
}