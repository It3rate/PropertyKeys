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
	public interface ISeries : IEnumerable
	{
		int VectorSize { get; }
		int Count { get; }
		SeriesType Type { get; }
		int DataSize { get; }
		RectFSeries Frame { get; }
		Series Size { get; }

		Series GetSeriesAtIndex(int index);
		void SetSeriesAtIndex(int index, Series series);
		Series GetValueAtVirtualIndex(int index, int capacity);
		Series GetValueAtT(float t);

		Series Copy();

		//void CombineInto(Series b, CombineFunction combineFunction, float t = 0);
		//void InterpolateInto(Series b, float t);
		//void InterpolateInto(Series b, ParametricSeries seriesT);

		//void ResetData();
		//void Update(float time);
		//void Reverse();

		//Series Sum();
		//Series Average();
		//Series Max();
		//Series Min();

		//float X { get; }
		//float Y { get; }
		//float Z { get; }
		//float W { get; }
		//float FloatDataAt(int index);
		//int IntDataAt(int index);
		//bool BoolDataAt(int index);
		//float[] FloatDataRef { get; }
		//int[] IntDataRef { get; }
		//bool[] BoolDataRef { get; }

		//Store CreateLinearStore(int capacity);
		//Store Store { get; }
		//Store BakedStore { get; }
		//Store ToStore(Sampler sampler);

		//Series GetZeroSeries();
		//Series GetZeroSeries(int elements);
		//Series GetMinSeries();
		//Series GetMaxSeries();
	}
}