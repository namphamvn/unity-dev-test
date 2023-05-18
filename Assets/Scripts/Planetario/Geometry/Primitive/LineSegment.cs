using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct LineSegment : IComponentData, ICurve3D, IClosestPoint
	{
		public float3 start;
		public float3 end;

		public LineSegment(float3 start, float3 end)
		{
			this.start = start;
			this.end = end;
		}

		public readonly float3 Point(float coord)
		{
			return math.lerp(start, end, coord);
		}

		public readonly float3 Tangent(float coord)
		{
			return math.normalize(end - start);
		}

		public readonly float Length()
		{
			return math.length(end - start);
		}

		public readonly ICurve3D OffsetCurve(float offset, float3 up)
		{
			var normal = this.Normal(0f, up);
			return new LineSegment
			{
				start = start + normal * offset,
				end = end + normal * offset
			};
		}

		public float3 ClosestPointTo(float3 point)
		{
			var direction = math.normalize(end - start);
			var pointDirection = point - start;
			var distanceAlongLine = math.dot(pointDirection, direction);
			return start + direction * distanceAlongLine;
		}

		public float DistanceTo(float3 point)
		{
			var closestPoint = ClosestPointTo(point);
			return math.distance(point, closestPoint);
		}
	}
}
