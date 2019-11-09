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
    }

    public interface ISeriesFloatElement : ISeriesBase
    {
        void CombineInto(ISeriesElement b, CombineFunction combineFunction, float t = 0);
        void InterpolateInto(ISeriesElement b, float t);
        void InterpolateInto(ISeriesElement b, ParametricSeries seriesT);

        float Sum();
        float Average();
        float Max();
        float Min();

        float this[int index] { get; set; }
        float FloatDataAt(int index);
        float[] FloatDataRef { get; }

        float X { get; }
        float Y { get; }
        float Z { get; }
        float W { get; }

        ISeriesElement GetZeroSeries();
        ISeriesElement GetMinSeries();
        ISeriesElement GetMaxSeries();
    }

    public interface ISeriesElement : ISeriesFloatElement
    {
        int IntDataAt(int index);
        int[] IntDataRef { get; }

        int A { get; }
        int B { get; }
        int C { get; }
        int D { get; }
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
        //List<Series> SeriesList { get; }

        RectFSeries Frame { get; }
        Series Size { get; }

    }
}