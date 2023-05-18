using Planetario.Geometry.Composite;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct CubicBezier : IComponentData, ICurve3D
	{
		public float3 a;
		public float3 b;
		public float3 c;
		public float3 d;

		public CubicBezier(
			float3 start, float3 startForward,
			float3 end, float3 endForward,
			float curveRatio)
		{
			a = start;
			b = a + startForward * curveRatio;
			d = end;
			c = d - endForward * curveRatio;
		}

		public CubicBezier(
			float3 start, float3 startForward,
			float3 end, float3 endForward,
			float curveRatio, float endToStartRatio)
		{
			a = start;
			b = a + startForward * curveRatio;
			d = end;
			c = d - endForward * curveRatio * endToStartRatio;
		}

		/// <summary>
		///     Algorithm: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B%C3%A9zier_curves
		///     Cubic Bezier Curve
		/// </summary>
		public readonly float3 Point(float coord)
		{
			var t1 = 1 - coord;
			var t12 = t1 * t1;
			var t13 = t1 * t1 * t1;
			return t13 * a
			       + 3 * t12 * coord * b
			       + 3 * t1 * coord * coord * c
			       + coord * coord * coord * d;
		}

		/// <summary>
		///     Algorithm: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B%C3%A9zier_curves
		///     Cubic Bezier Curve
		/// </summary>
		public readonly float3 Tangent(float coord)
		{
			var t1 = 1 - coord;
			var t12 = t1 * t1;
			return -3 * t12 * a
			       + (3 * t12 - 6 * t1 * coord) * b
			       + (-3 * coord * coord + 6 * coord * t1) * c
			       + 3 * coord * coord * d;
		}

		private readonly float RoughLength()
		{
			return math.length(d - a);
		}

		public readonly float Length()
		{
			var roughLen = (int)math.ceil(RoughLength());
			return this.ApproximateLength(roughLen);
		}

		public readonly ICurve3D OffsetCurve(float offset, float3 up)
		{
			return new OffsetCurve(this, offset, up);
		}
	}
}
