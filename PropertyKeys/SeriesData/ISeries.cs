using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
    public interface ISeriesBase : IEnumerable
    {
        SeriesType Type { get; }
        int VectorSize { get; }
        ISeriesElement GetSample(ParametricSeries seriesT);

        ISeriesBase Copy();
        void Reverse();

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

    public interface ISeries : ISeriesBase
    {
        int Count { get; }
        bool CanInterpolate { get; }

        int DataSize { get; }

        ISeriesElement GetRawDataAt(int index);
        void SetRawDataAt(int index, ISeriesElement series);
        ISeriesElement GetVirtualValueAt(int index, int capacity);
        ISeriesElement GetVirtualValueAt(float t);

        ISeries SeriesSum();
        ISeries SeriesAverage();
        ISeries SeriesMax();
        ISeries SeriesMin();

        //void ResetData();
        //void Update(float time);


        //Store CreateLinearStore(int capacity);
        //Store Store { get; }
        //Store BakedStore { get; }
        //Store ToStore(Sampler sampler);

        //Series GetZeroSeries(int elements);

    }

    public interface IDimensionedSeries : ISeries
    {
        int Dimensions { get; }
        ISeries GetRawSeriesAt(int index);
        void SetRawSeriesAt(int index, ISeries series);
        //List<Series> SeriesList { get; }

        RectFSeries Frame { get; }
        Series Size { get; }

    }
}