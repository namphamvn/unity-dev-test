using System;
using System.Collections.Generic;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct Circle2D : IComponentData, ICurve2D
	{
		public float2 center;
		public float radius;

		public Circle2D(float2 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		/// <summary>
		///     Convert coordinate to 2d point
		/// </summary>
		/// <param name="coord">From 0 to 1</param>
		/// <returns>2d position</returns>
		public float2 Point(float coord)
		{
			var tAngle = coord * VectorExtensions.PIx2;
			return center + new float2(math.cos(tAngle), math.sin(tAngle)) * radius;
		}

		public float2 Tangent(float coord)
		{
			//TODO optimize using X, Y swap
			return math.cross((Point(coord) - center).To3D(), math.up()).To2D();
		}

		public float Length()
		{
			return radius * VectorExtensions.PIx2;
		}

		public ICurve2D OffsetCurve(float offset)
		{
			return OffsetCircle(offset);
		}

		private Circle2D OffsetCircle(float offset)
		{
			return new Circle2D(center, radius + offset);
		}

		public IEnumerable<float> IntersectCoord(ICurve2D other)
		{
			foreach (var result in Intersect(other))
			{
				var angle = CircularArc2D.PointToAngle(result, center);
				yield return angle / VectorExtensions.PIx2;
			}
		}

		public IEnumerable<float2> Intersect(ICurve2D other)
		{
			float2 point1;
			float2 point2;
			int count;
			if (other is Circle2D circle)
			{
				count = IntersectCircle(circle, out point1, out point2);
			}
			else if (other is LineSegment2D line)
			{
				count = IntersectLine(line, out point1, out point2);
			}
			else if (other is CircularArc2D arc)
			{
				count = IntersectArc(arc, out point1, out point2);
			}
			else
			{
				throw new NotImplementedException();
			}

			if (count >= 1)
			{
				yield return point1;
			}

			if (count >= 2)
			{
				yield return point2;
			}
		}

		/// <summary>
		///     http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
		///     Find the points where the two circles intersect.
		/// </summary>
		/// <returns>number of intersection point</returns>
		private static int FindCircleCircleIntersections(
			float cx0, float cy0, float radius0,
			float cx1, float cy1, float radius1,
			out float2 intersection1, out float2 intersection2)
		{
			// Find the distance between the centers.
			var dx = cx0 - cx1;
			var dy = cy0 - cy1;
			double dist = math.sqrt(dx * dx + dy * dy);

			// See how many solutions there are.
			if (dist > radius0 + radius1)
			{
				// No solutions, the circles are too far apart.
				intersection1 = new float2(float.NaN, float.NaN);
				intersection2 = new float2(float.NaN, float.NaN);
				return 0;
			}

			if (dist < math.abs(radius0 - radius1))
			{
				// No solutions, one circle contains the other.
				intersection1 = new float2(float.NaN, float.NaN);
				intersection2 = new float2(float.NaN, float.NaN);
				return 0;
			}

			if (dist == 0 && radius0 == radius1)
			{
				// No solutions, the circles coincide.
				intersection1 = new float2(float.NaN, float.NaN);
				intersection2 = new float2(float.NaN, float.NaN);
				return 0;
			}

			// Find a and h.
			var a = (radius0 * radius0 -
				radius1 * radius1 + dist * dist) / (2 * dist);
			var h = math.sqrt(radius0 * radius0 - a * a);

			// Find P2.
			var cx2 = cx0 + a * (cx1 - cx0) / dist;
			var cy2 = cy0 + a * (cy1 - cy0) / dist;

			// Get the points P3.
			intersection1 = new float2(
				(float)(cx2 + h * (cy1 - cy0) / dist),
				(float)(cy2 - h * (cx1 - cx0) / dist));
			intersection2 = new float2(
				(float)(cx2 - h * (cy1 - cy0) / dist),
				(float)(cy2 + h * (cx1 - cx0) / dist));

			// See if we have 1 or 2 solutions.
			if (dist == radius0 + radius1)
			{
				return 1;
			}

			return 2;
		}

		public int IntersectCircle(in Circle2D other, out float2 point1, out float2 point2)
		{
			return FindCircleCircleIntersections(center.x, center.y,
				radius, other.center.x, other.center.y, other.radius,
				out point1, out point2);
		}

		/// <summary>
		///     http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/#
		///     Find the points of intersection.
		/// </summary>
		/// <returns>number of intersection points</returns>
		private int FindLineCircleIntersections(
			float cx, float cy, float radius,
			float2 point1, float2 point2,
			out float2 intersection1, out float2 intersection2)
		{
			float dx, dy, a, b, c, det, t;

			dx = point2.x - point1.x;
			dy = point2.y - point1.y;

			a = dx * dx + dy * dy;
			b = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
			c = (point1.x - cx) * (point1.x - cx) +
			    (point1.y - cy) * (point1.y - cy) -
			    radius * radius;

			det = b * b - 4 * a * c;
			if (a <= VectorExtensions.Epsilon || det < 0)
			{
				// No real solutions.
				intersection1 = default;
				intersection2 = default;
				return 0;
			}

			if (det == 0)
			{
				// One solution.
				t = -b / (2 * a);
				if (t >= 0 && t <= 1)
				{
					intersection1 =
						new float2(point1.x + t * dx, point1.y + t * dy);
					intersection2 = default;
					return 1;
				}

				intersection1 = default;
				intersection2 = default;
				return 0;
			}

			// Two solutions.
			t = (-b + math.sqrt(det)) / (2 * a);
			if (t >= 0 && t <= 1)
			{
				intersection1 =
					new float2(point1.x + t * dx, point1.y + t * dy);

				t = (-b - math.sqrt(det)) / (2 * a);
				if (t >= 0 && t <= 1)
				{
					intersection2 =
						new float2(point1.x + t * dx, point1.y + t * dy);
					return 2;
				}

				intersection2 = default;
				return 1;
			}

			t = (-b - math.sqrt(det)) / (2 * a);
			if (t >= 0 && t <= 1)
			{
				intersection1 =
					new float2(point1.x + t * dx, point1.y + t * dy);
				intersection2 = default;
				return 1;
			}

			intersection1 = intersection2 = default;
			return 0;
		}

		public int IntersectLine(in LineSegment2D other, out float2 point1, out float2 point2)
		{
			return FindLineCircleIntersections(center.x, center.y,
				radius, other.start, other.end,
				out point1, out point2);
		}

		public int IntersectArc(in CircularArc2D other, out float2 point1, out float2 point2)
		{
			var count = FindCircleCircleIntersections(center.x, center.y,
				radius, other.center.x, other.center.y, other.radius,
				out point1, out point2);
			if (count == 2)
			{
				if (other.ContainsPoint(point2))
				{
					if (!other.ContainsPoint(point1))
					{
						point1 = point2;
						return 1;
					}
				}
				else
				{
					count--;
				}
			}

			if (count == 1 && !other.ContainsPoint(point1))
			{
				count--;
			}

			return count;
		}
	}
}
