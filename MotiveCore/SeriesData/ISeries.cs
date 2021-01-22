﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components.Libraries;
using Motive.Samplers.Utils;
using Motive.SeriesData.Utils;
using Motive.Stores;

namespace Motive.SeriesData
{
	public enum SeriesType
	{
		Int,
		Float,
		Parametric,
		RectF,
		Bezier,
	}

    public interface ISeries : IEnumerable, IDefinition
	{
        // From IDefinition:
        //string Name { get; set; }
        //int Id { get; }
        //void Update(double currentTime, double deltaTime);
        //void OnActivate();
        //void OnDeactivate();

        int Count { get; }
		SeriesType Type { get; }
		int VectorSize { get; set; }
		ClampMode IndexClampMode { get; set; }

        // todo: this frame and size implementation is weak and temporary
        RectFSeries Frame { get; }
        ISeries Size { get; }

		float X { get; set; }
		float Y { get; set; }
		float Z { get; set; }
		float W { get; set; }

        int DataSize { get; }
        ISeries GetSeriesAt(float t);
        ISeries GetSeriesAt(int index);
        void SetSeriesAt(int index, ISeries series);
        ISeries GetInterpolatedSeriesAt(float t);
        float FloatValueAt(int index);
        void SetFloatValueAt(int index, float value);
        int IntValueAt(int index);
        void SetIntValueAt(int index, int value);
        float[] FloatDataRef { get; }
        int[] IntDataRef { get; }

        void ReverseEachElement();
        void Append(ISeries series);
        void CombineInto(ISeries b, CombineFunction combineFunction, float t = 0);
        void InterpolateInto(ISeries b, float t);
        void InterpolateInto(ISeries b, ParametricSeries seriesT);

        Store CreateLinearStore(int capacity);
        IStore Store(Sampler sampler = null);
        List<ISeries> ToList();
        void SetByList(List<ISeries> items);
        ISeries Copy();

        void ResetData();
        void Map(FloatEquation floatEquation);
        void MapValuesToItemPositions(IntSeries items);
        void MapOrderToItemPositions(IntSeries items);
    }

	public class SeriesEnumerator : IEnumerator
	{
		private ISeries _instance;
		private int _position = -1;
		public SeriesEnumerator(ISeries instance)
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
	    ISeries GetZeroSeries(int elements);
    }


    public interface IDimensionedSeries : ISeries
    {
        int Dimensions { get; }
        ISeries GetRawSeriesAt(int index);
        void SetRawSeriesAt(int index, ISeries series);
        //List<CurrentSeries> SeriesList { get; }

        RectFSeries Frame { get; }
        ISeries Size { get; }

    }
}