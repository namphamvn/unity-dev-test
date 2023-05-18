using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Planetario.Geometry.Composite
{
	public struct MultiCurve2D : ICurve2D
	{
		private readonly ICurve2D[] _curves;
		private readonly List<float> _lengths;
		private readonly float _totalLength;
		private readonly List<float> _coords;

		public IEnumerable<ICurve2D> Curves => _curves;

		public MultiCurve2D(IEnumerable<ICurve2D> curves)
		{
			_curves = curves.ToArray();
			var count = _curves.Length;
			_lengths = new List<float>(count);
			_totalLength = 0f;
			foreach (var curve in _curves)
			{
				var len = curve.Length();
				_totalLength += len;
				_lengths.Add(_totalLength);
			}

			_coords = new List<float>(count);
			for (var i = 0; i < count; i++)
			{
				_coords.Add(_lengths[i] / _totalLength);
			}
		}

		private int BinarySearch(float coord)
		{
			//TODO use binary search
			for (var i = 0; i < _coords.Count; i++)
			{
				if (_coords[i] >= coord)
				{
					return i;
				}
			}

			return -1;
		}

		private (float, int) ConvertCoord(float coord)
		{
			var index = BinarySearch(coord);
			var prevCoord = index > 0 ? _coords[index - 1] : 0f;
			var coordLen = _coords[index] - prevCoord;
			return ((coord - prevCoord) / coordLen, index);
		}

		public float2 Point(float coord)
		{
			(var newCoord, var index) = ConvertCoord(coord);
			return _curves[index].Point(newCoord);
		}

		public float2 Tangent(float coord)
		{
			(var newCoord, var index) = ConvertCoord(coord);
			return _curves[index].Tangent(newCoord);
		}

		public float Length()
		{
			return _totalLength;
		}

		public ICurve2D OffsetCurve(float offset)
		{
			var curves = _curves
				.Select(curve => curve.OffsetCurve(offset))
				.ToArray();
			return new MultiCurve2D(curves);
		}

		public IEnumerable<float2> Intersect(ICurve2D other)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<float> IntersectCoord(ICurve2D other)
		{
			throw new NotImplementedException();
		}
	}
}
