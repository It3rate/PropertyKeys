﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Motive.SeriesData
{
    public class DimensionedSeries 
    {
	    public int Dimensions { get; private set; }
	    public int VectorSize { get; private set; }
        public int Count => _seriesList[0].Count;
        public SeriesType Type => _seriesList[0].Type;
        public int DataSize => Count * Dimensions;

        public RectFSeries Frame => throw new NotImplementedException();
        public ISeries Size => throw new NotImplementedException();

        private readonly List<ISeries> _seriesList = new List<ISeries>();

        public DimensionedSeries(int dimensions, int vectorSize, params float[][] values)
        {
	        Assert.IsTrue(values.Length > 0);
	        Assert.IsTrue(values.Length == Dimensions);

	        Dimensions = dimensions;
	        VectorSize = vectorSize;
	        for (int i = 0; i < Dimensions; i++)
	        {
		        _seriesList.Add(new FloatSeries(VectorSize, values[i]));
	        }
        }
        public DimensionedSeries(int dimensions, int vectorSize, params ISeries[] seriesArray)
        {
	        Assert.IsTrue(seriesArray.Length > 0);
	        Assert.IsTrue(seriesArray.Length == Dimensions);

	        Dimensions = dimensions;
	        VectorSize = vectorSize;
	        for (int i = 0; i < Dimensions; i++)
	        {
		        _seriesList.Add(seriesArray[i]);
	        }
        }

        public ISeries GetSample(ParametricSeries seriesT)
        {
	        ISeries result;
	        if (Type == SeriesType.Int)
	        {
		        var values = new int[VectorSize];
		        for (var i = 0; i < values.Length; i++)
		        {
			        values[i] = _seriesList[i].GetVirtualValueAt(seriesT[i]).IntValueAt(0);
		        }

		        result = new IntSeries(VectorSize, values);
	        }
	        else
	        {
		        var values = new float[VectorSize];
		        for (var i = 0; i < values.Length; i++)
		        {
			        values[i] = _seriesList[i].GetVirtualValueAt(seriesT[i]).X;
		        }

		        result = new FloatSeries(VectorSize, values);
            }
	        return result;
        }
        
        public ISeries GetRawDataAt(int index)
        {
            throw new NotImplementedException();
        }
        public void SetRawDataAt(int index, ISeries series)
        {
            throw new NotImplementedException();
        }
        public ISeries GetVirtualValueAt(float t)
        {
            throw new NotImplementedException();
        }
        public ISeries GetVirtualValueAt(int index, int capacity)
        {
            throw new NotImplementedException();
        }


        public ISeries Copy()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}