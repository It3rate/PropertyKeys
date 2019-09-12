using System;
using System.Drawing.Drawing2D;
using DataArcs.Stores;

namespace DataArcs.SeriesData
{
	// maybe store all values as quadratic, allowing easier blending?
	// try blending with len/angle from zero vs interpolate points.
	public enum BezierMove : int
	{
		MoveTo,
		LineTo,
		QuadTo,
		CubeTo,
		End,
	}

	/// <summary>
	/// Specialized series for 2D Beziers, holds the extra move data. Use a BezierSampler to render and interpolate.
	/// </summary>
	public class BezierSeries : FloatSeries
	{
		public static readonly int[] MoveSize = new int[] {2, 2, 4, 6, 0};

		public BezierMove[] Moves { get; }

		public BezierSeries(float[] values, BezierMove[] moves = null) : base(2, values,
			moves?.Length ?? values.Length / 2)
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

		public override Series GetValueAtT(float t)
		{
			var index = (int) (t * VirtualCount);
			return GetSeriesAtIndex(index);
		}

		public override Series GetSeriesAtIndex(int index)
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

		public override void SetSeriesAtIndex(int index, Series series)
		{
			index = Math.Max(0, Math.Min(Moves.Length - 1, index));
			var start = 0;
			for (var i = 0; i < index; i++)
			{
				start += MoveSize[(int) Moves[i]];
			}

			var size = MoveSize[(int) Moves[index]];
			Array.Copy(series.FloatData, 0, _floatValues, index * VectorSize, VectorSize);
			if (series is BezierSeries)
			{
				Moves[index] = ((BezierSeries) series).Moves[0];
			}
		}

		public override Series HardenToData(Store store = null)
		{
			Series result = this;
			var len = VirtualCount * VectorSize;
			if (_floatValues.Length != len)
			{
				var vals = new float[len];
				var moves = new BezierMove[VirtualCount];
				for (var i = 0; i < VirtualCount; i++)
				{
					var val = store == null ? GetSeriesAtIndex(i).FloatData : store.GetSeriesAtIndex(i).FloatData;
					Array.Copy(val, 0 * VectorSize, vals, i * VectorSize, VectorSize);
					moves[i] = i < moves.Length ? moves[i] : BezierMove.LineTo;
				}

				result = new BezierSeries(vals, moves);
			}

			return result;
		}

		public GraphicsPath Path()
		{
			var path = new GraphicsPath();
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
					case BezierMove.End:
					default:
						path.CloseFigure();
						break;
				}

				index += MoveSize[(int) moveType];
			}

			return path;
		}
	}
}