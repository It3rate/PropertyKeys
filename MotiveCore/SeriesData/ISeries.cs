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
		//RectFSeries Frame { get; }
		//Series Size { get; }
		//void Update(double currentTime, double deltaTime);
		//void OnActivate();
		//void OnDeactivate();
		//bool AssignIdIfUnset(int id);

		int Count { get; }
		SeriesType Type { get; }
		int VectorSize { get; set; }

		float FloatDataAt(int index);
		IStore Store(Sampler sampler = null);
		Series Copy();

		float X { get; }
		float Y { get; }
		float Z { get; }
		float W { get; }

		//int DataSize { get; }
		//Series GetRawDataAt(float t);
		//Series GetRawDataAt(int index);
		//void SetRawDataAt(int index, Series series);
		//Series GetVirtualValueAt(float t);
		//float FloatDataAt(int index);
		//int IntDataAt(int index);
		//float[] FloatDataRef { get; }
		//int[] IntDataRef { get; }

		//void ResetData();
		//void ReverseEachElement();
		//void Append(Series series);
		//void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
		//void InterpolateInto(Series b, float t);
		//void InterpolateInto(Series b, ParametricSeries seriesT);

		//Store CreateLinearStore(int capacity);
		//IStore Store(Sampler sampler = null);
		//List<Series> ToList();
		//void SetByList(List<Series> items);
		//Series Copy();

		//Series SumSlots(params Slot[] slots);
		//Series MultiplySlots(params Slot[] slots);
		//Series AverageSlots(params Slot[] slots);
		//Series MaxSlots(params Slot[] slots);
		//Series MinSlots(params Slot[] slots);
		//void Map(FloatEquation floatEquation);
		//void MapValuesToItemPositions(IntSeries items);
		//void MapOrderToItemPositions(IntSeries items);
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