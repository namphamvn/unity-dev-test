using System;
using System.Collections.Generic;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct CircularArc2D : IComponentData, ICurve2D
	{
		public float2 center;
		public float startAngle; //clockwise going along Up vector
		public float endAngle;
		public float radius;

		public CircularArc2D(float2 center, float2 right, float angle)
		{
			this.center = center;
			radius = math.length(right);
			startAngle = math.atan2(right.y, right.x);
			endAngle = startAngle + angle;

			if ((angle > 0 && startAngle < 0) ||
			    (angle < 0 && endAngle < 0))
			{
				startAngle += VectorExtensions.PIx2;
				endAngle += VectorExtensions.PIx2;
			}
		}

		public static float3 up = math.up();
		private static readonly float3 _down = -math.up();

		public float2 Point(float coord)
		{
			var tAngle = math.lerp(startAngle, endAngle, coord);
			return center + new float2(math.cos(tAngle), math.sin(tAngle)) * radius;
		}

		public float2 Tangent(float coord)
		{
			//TODO optimize using X, Y swap
			return math.cross((Point(coord) - center).To3D(), endAngle > startAngle ? up : _down).To2D();
		}

		public float Length()
		{
			return radius * math.abs(endAngle - startAngle);
		}

		public ICurve2D OffsetCurve(float offset)
		{
			return OffsetArc(offset);
		}

		public IEnumerable<float> IntersectCoord(ICurve2D other)
		{
			var angleRange = endAngle - startAngle;
			foreach (var result in Intersect(other))
			{
				var angle = PointToAngle(result, center);
				if (angle < startAngle)
				{
					angle += VectorExtensions.PIx2;
				}

				yield return (angle - startAngle) / angleRange;
			}
		}

		public IEnumerable<float2> Intersect(ICurve2D other)
		{
			if (other is Circle2D circle)
			{
				foreach (var result in circle.Intersect(this))
				{
					yield return result;
				}
			}
			else if (other is LineSegment2D line)
			{
				foreach (var result in line.Intersect(this))
				{
					yield return result;
				}
			}
			else if (other is CircularArc2D arc)
			{
				var arcCircle = new Circle2D(arc.center, arc.radius);
				foreach (var result in arcCircle.Intersect(this))
				{
					if (arc.ContainsPoint(result))
					{
						yield return result;
					}
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private CircularArc2D OffsetArc(float offset)
		{
			var newRadius = radius + offset;
			return new CircularArc2D
			{
				center = center,
				startAngle = startAngle,
				endAngle = endAngle,
				radius = newRadius
			};
		}

		public CircularArc2D Flip()
		{
			return new CircularArc2D
			{
				center = center,
				radius = radius,
				startAngle = endAngle,
				endAngle = startAngle
			};
		}

		public static float PointToAngle(float2 point, float2 center)
		{
			var dir = point - center;
			var angle = math.atan2(dir.y, dir.x);
			if (angle < 0)
			{
				angle += VectorExtensions.PIx2;
			}

			return angle;
		}

		/// <summary>
		///     Check if a point is inside the arc
		///     Assumed that the point is already on the circle of the arc
		/// </summary>
		public bool ContainsPoint(float2 point)
		{
			var angle = PointToAngle(point, center);
			var minAngle = math.min(startAngle, endAngle);
			var maxAngle = math.max(startAngle, endAngle);
			if (maxAngle <= VectorExtensions.PIx2)
			{
				return minAngle <= angle && angle <= maxAngle;
			}

			return (minAngle <= angle && angle <= VectorExtensions.PIx2) ||
			       (0 <= angle && angle <= maxAngle - VectorExtensions.PIx2);
		}
	}


	//old implementation
	//     [GenerateAuthoringComponent]
//     public struct CircularArc2D: IComponentData, ICurve2D
//     {
//         public float2 Center;
//         public float2 Right; //length equal to Radius
//         public float2 Forward; //length equal to Radius
//         public float Angle; //clockwise going along Up vector
//
//         public CircularArc2D(float2 center, float2 right, float2 forward, float angle)
//         {
//             Center = center;
//             Right = right;
//             Forward = forward;
//             var up = math.cross(Forward.To3D(), Right.To3D());
//             Angle = up.y > 0f ? math.abs(angle) : -math.abs(angle);
//         }
//
//         public float Radius => math.length(Right);
//
//         public static float3 Up = math.up();
//         private static float3 Down = -math.up();
//
//         public float2 Point(float t)
//         {
//             float tAngle = t * math.abs(Angle);
//             return Center + Right * math.cos(tAngle) + Forward * math.sin(tAngle);
//         }
//
//         public float2 Tangent(float t)
//         {
//             //TODO optimize using X, Y swap
//             var point = Point(t);
//             return math.cross((point - Center).To3D(), Angle > 0f ? Up : Down).To2D();
//         }
//
//         public float Length()
//         {
//             return Radius * math.abs(Angle);
//         }
//
//         public ICurve2D OffsetCurve(float offset)
//         {
//             return OffsetArc(offset);
//         }
//
//         private CircularArc2D OffsetArc(float offset)
//         {
//             float newRadius = Radius + offset; //(Angle > 0f ? offset : -offset);
//             return new CircularArc2D(Center, math.normalize(Right) * newRadius,
//                 math.normalize(Forward) * newRadius, Angle);
//         }
//
//         public CircularArc2D Flip()
//         {
//             var endPoint = Point(1f);
//             var right = endPoint - Center;
//             return new CircularArc2D
//             {
//                 Center = Center,
//                 Angle = -Angle,
//                 Right = right, //x
//
//                 //up is unit vector, forward will take length from right
//                 Forward = math.normalize(math.cross(right.To3D(), Angle < 0 ? Up : Down)).To2D() * Radius,
//             };
//         }
//     }
}
