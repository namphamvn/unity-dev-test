using Planetario.Geometry.Composite;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct QuadBezier : IComponentData, ICurve3D
	{
		public float3 a;
		public float3 b;
		public float3 c;

		/// <summary>
		///     Algorithm: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B%C3%A9zier_curves
		///     Cubic Bezier Curve
		/// </summary>
		public float3 Point(float coord)
		{
			var t1 = 1 - coord;
			return t1 * t1 * a
			       + 2 * t1 * coord * b
			       + coord * coord * c;
		}

		/// <summary>
		///     Algorithm: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B%C3%A9zier_curves
		///     Cubic Bezier Curve
		/// </summary>
		public float3 Tangent(float coord)
		{
			var t1 = 1 - coord;
			return 2 * t1 * (b - a)
			       + 2 * coord * (c - b);
		}

		private static readonly int _lengthSegmentCount = 50;

		public float Length()
		{
			return this.ApproximateLength(_lengthSegmentCount);
		}

		public ICurve3D OffsetCurve(float offset, float3 up)
		{
			return new OffsetCurve(this, offset, up);
		}
	}
}
