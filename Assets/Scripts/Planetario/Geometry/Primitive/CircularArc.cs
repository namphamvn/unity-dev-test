using Planetario.Geometry.Composite;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct CircularArc : IComponentData, ICurve3D
	{
		public float3 center;
		public float3 right; //length equal to Radius
		public float3 forward; //length equal to Radius
		public float3 up; //unit length
		public float angle; //clockwise going along Up vector

		public float Radius()
		{
			return math.length(right);
		}

		public float3 Point(float coord)
		{
			var tAngle = coord * angle;
			return center + right * math.cos(tAngle) + forward * math.sin(tAngle);
		}

		public float3 Tangent(float coord)
		{
			var point = Point(coord);
			return math.cross(point - center, up);
		}

		public float Length()
		{
			return Radius() * angle;
		}

		public ICurve3D OffsetCurve(float offset, float3 up)
		{
			return new OffsetCurve(this, offset, up);
		}
	}
}
