using Unity.Mathematics;

namespace Planetario.Geometry.Extensions
{
	public static class CurveExtensions2D
	{
		private static readonly float3 UpVector = new(0f, 1f, 0f);

		public static float2 Normal<T>(this T curve, float coord) where T : ICurve2D
		{
			//TODO optimise with 2D normal computation (swap x, y)
			return math.normalize(math.cross(curve.Tangent(coord).To3D(), UpVector)).To2D();
		}

		public static ICurve3D To3D(this ICurve2D curve)
		{
			return new Curve2DWrap(curve);
		}

		private class Curve2DWrap : ICurve3D
		{
			private readonly ICurve2D _source;

			public Curve2DWrap(ICurve2D source)
			{
				_source = source;
			}

			public float3 Point(float coord)
			{
				return _source.Point(coord).To3D();
			}

			public float3 Tangent(float coord)
			{
				return _source.Tangent(coord).To3D();
			}

			public float Length()
			{
				return _source.Length();
			}

			public ICurve3D OffsetCurve(float offset, float3 up)
			{
				return new Curve2DWrap(_source.OffsetCurve(offset));
			}
		}
	}
}
