using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Motive.Graphic;
using Motive.Stores;

namespace Motive.SeriesData
{
	/// <summary>
	/// Specialized series for 2D Beziers, holds the extra move data. Use a BezierSampler to render and interpolate.
	/// </summary>
	public class BezierSeries : FloatSeries, IDrawableSeries
	{
		public override SeriesType Type => SeriesType.Float; // todo: accommodate bezier here

        public static readonly int[] MoveSize = new int[] {2, 2, 4, 6, 0};

        public override int Count => (int)(_floatValues.Length / VectorSize);

        public BezierMove[] Moves { get; }

		private int _polyLineCount = 1000;
		private float[] _polylineDistances;
		private float _polylineLength;
        private bool _evenlySpaced = false;
        public bool EvenlySpaced
        {
	        get => _evenlySpaced;
	        set
	        {
		        // Use dirty flag if this gets repetitious.
                if(value)
                {
	                // Need to use normal bezier sampling to convert to polylines.
                    _evenlySpaced = false;
                    MakeEvenlySpaced();
                }

		        _evenlySpaced = value;
	        }
        }

        public BezierSeries(float[] values, BezierMove[] moves = null) : base(2, values)
		{
			// default to polyline if moves is empty
			if (moves == null)
			{
				var len = values.Length / 2;
				moves = new BezierMove[len];
				for (var i = 0; i < len; i++)
				{
					moves[i] = BezierMove.LineTo;
				}
			}

			Moves = moves;
		}

        private void MakeEvenlySpaced()
		{
			MeasureBezier();

		}
		private void MeasureBezier()
		{
			float len = 0;
			_polylineDistances = new float[_polyLineCount];

            if (Moves.Length > 1)
			{
				var pt0 = GetSeriesAtT(0);
				var x0 = pt0.FloatDataRef[0];
				var y0 = pt0.FloatDataRef[1];
                for (int i = 1; i < _polyLineCount; i++)
                {
                    var pt1 = GetSeriesAtT(i / (float) _polyLineCount);
                    var x1 = pt1.FloatDataRef[0];
                    var y1 = pt1.FloatDataRef[1];
                    var xDif = x1 - x0;
					var yDif = y1 - y0;
					len += (float)Math.Sqrt(xDif * xDif + yDif * yDif);
                    _polylineDistances[i] = len;
					x0 = x1;
					y0 = y1;
                }
			}
            _polylineLength = len;
		}
        public override Series GetVirtualValueAt(float t)
		{
			var index = (int) (t * Count);
			return GetSeriesAt(index);
		}

		public override Series GetSeriesAt(int index)
		{
			index = Math.Max(0, Math.Min(Moves.Length - 1, index));
			var start = 0;
			for (var i = 0; i < index; i++)
			{
				start += MoveSize[(int) Moves[i]];
			}

			var size = MoveSize[(int) Moves[index]];
			var result = new float[size];
			Array.Copy(_floatValues, start, result, 0, size);
			return new BezierSeries(result, new[] {Moves[index]});
		}
        // todo: need an insertDataAtIndex
		public override void SetSeriesAt(int index, Series series)
		{
			index = Math.Max(0, Math.Min(Moves.Length - 1, index));
			var start = 0;
			for (var i = 0; i < index; i++)
			{
				start += MoveSize[(int) Moves[i]];
			}

            // todo: need to adjust float array length if move size is different
			var size = MoveSize[(int) Moves[index]];
			if (series is BezierSeries)
			{
                var newMove = ((BezierSeries)series).Moves[0];
                var newSize = MoveSize[(int)newMove];
                if(newSize != size)
                {
                    int diff = newSize - size;
                    float[] newFloats = new float[_floatValues.Length + diff];
                    Array.Copy(_floatValues, 0, newFloats, 0, start);
                    Array.Copy(series.FloatDataRef, 0, newFloats, start, newSize);
                    Array.Copy(_floatValues, start + 1, newFloats, start + newSize, _floatValues.Length - start);
                    _floatValues = newFloats; // todo: make immutable, return a series (or this if no change) to store.
                }
                else
                {
                    Array.Copy(series.FloatDataRef, 0, _floatValues, start, newSize);
                }
                Moves[index] = newMove;
			}
            else
            {
                Array.Copy(series.FloatDataRef, 0, _floatValues, start, size);
            }
		}

        public FloatSeries GetSeriesAtT(float t)
		{
			float vT;
			int startIndex, endIndex;

			if (EvenlySpaced)
			{
				GetEvenSpacedSegmentFromT(t, out vT, out startIndex, out endIndex);
			}
			else
			{
				GetSegmentFromT(t, out vT, out startIndex, out endIndex);
			}

			var aSeries = GetSeriesAt(startIndex);
			var a = new[] { aSeries.FloatDataRef[aSeries.DataSize - 2], aSeries.FloatDataRef[aSeries.DataSize - 1] };
			var b = GetSeriesAt(endIndex).FloatDataRef;
			var moveType = endIndex < Moves.Length ? Moves[endIndex] : BezierMove.End;

			var p2Index = b.Length - 2;
			float[] result = { 0, 0 };
			var it = 1f - vT;
			switch (moveType)
			{
				case BezierMove.MoveTo:
				case BezierMove.LineTo:
					result[0] = a[0] + (b[p2Index] - a[0]) * vT;
					result[1] = a[1] + (b[p2Index + 1] - a[1]) * vT;
					break;
				case BezierMove.QuadTo:
					result[0] = it * it * a[0] + 2 * it * vT * b[0] + vT * vT * b[p2Index];
					result[1] = it * it * a[1] + 2 * it * vT * b[1] + vT * vT * b[p2Index + 1];
					break;
				case BezierMove.CubeTo:
					// todo: cubic bezier calc
					break;
				case BezierMove.End: // special case when t == 1
					result[0] = a[a.Length - 2];
					result[1] = a[a.Length - 1];
					break;
				default:
					result = b;
					break;
			}
			return new FloatSeries(2, result);
		}

        public void GetSegmentFromT(float t, out float remainder, out int startIndex, out int endIndex)
		{
			int drawableSegments = Moves.Length - 1;
			startIndex = (int)Math.Floor(t * drawableSegments);
			remainder = (t - startIndex / (float)drawableSegments) * drawableSegments;
			endIndex = startIndex + 1;
		}
		public void GetEvenSpacedSegmentFromT(float t, out float remainder, out int startIndex, out int endIndex)
		{
			var targetLength = _polylineLength * t;
			int pos = 0;
			for (; pos < _polyLineCount - 1; pos++)
			{
				if (_polylineDistances[pos + 1] > targetLength)
				{
					break;
				}
			}

			float newT = 1;
			if (pos < _polyLineCount - 1)
			{
				newT = pos / (float)(_polyLineCount - 1);
				float rem = (targetLength - _polylineDistances[pos]) / (_polylineDistances[pos + 1] - _polylineDistances[pos]);
				newT += rem / (_polyLineCount - 1);
			}
			
			int drawableSegments = Moves.Length - 1;
			startIndex = (int)Math.Floor(newT * drawableSegments);
			remainder = (newT - startIndex / (float)drawableSegments) * drawableSegments;
			endIndex = startIndex + 1;
		}
        public override void ReverseEachElement()
		{
			base.ReverseEachElement();
			Array.Reverse(Moves);
		}

        public void AppendToGraphicsPath(GraphicsPath path)
		{
			path.FillMode = FillMode.Alternate;
			float posX = 0;
			float posY = 0;
			var index = 0;
			foreach (var moveType in Moves)
			{
				switch (moveType)
				{
					case BezierMove.MoveTo:
						posX = _floatValues[index];
						posY = _floatValues[index + 1];
						break;
					case BezierMove.LineTo:
						path.AddLine(posX, posY, _floatValues[index], _floatValues[index + 1]);
						posX = _floatValues[index];
						posY = _floatValues[index + 1];
						break;
					case BezierMove.QuadTo:
						// must convert to cubic for gdi
						var cx = _floatValues[index];
						var cy = _floatValues[index + 1];
						var a1x = _floatValues[index + 2];
						var a1y = _floatValues[index + 3];
						var c1x = (cx - posX) * 2 / 3 + posX;
						var c1y = (cy - posY) * 2 / 3 + posY;
						var c2x = a1x - (a1x - cx) * 2 / 3;
						var c2y = a1y - (a1y - cy) * 2 / 3;
						path.AddBezier(posX, posY, c1x, c1y, c2x, c2y, a1x, a1y);
						posX = a1x;
						posY = a1y;
						break;
					case BezierMove.CubeTo:
						path.AddBezier(posX, posY,
							_floatValues[index], _floatValues[index + 1],
							_floatValues[index + 2], _floatValues[index + 3],
							_floatValues[index + 4], _floatValues[index + 5]);
						posX = _floatValues[index + 4];
						posY = _floatValues[index + 5];
						break;
					default:
						path.CloseFigure();
						break;
				}

				index += MoveSize[(int) moveType];
			}
		}
		public override ISeries Copy()
		{
			BezierSeries result = new BezierSeries((float[])FloatDataRef.Clone(), (BezierMove[])Moves.Clone());
			return result;
		}
    }

	// maybe store all values as quadratic, allowing easier blending?
	// todo: split into all quadratic beziers and all poly lines. 
	// Use identical first and second coordinate for lines in quadratic, *or use midpoint and Move data determines lines etc.
	// try blending with len/angle from zero vs interpolate points.
	public enum BezierMove : int
	{
		MoveTo,
		LineTo,
		QuadTo,
		CubeTo,
		End,
	}

}