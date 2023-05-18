using System;
using System.Collections.Generic;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct LineSegment2D : IComponentData, ICurve2D
	{
		public float2 start;
		public float2 end;

		public float2 Point(float coord)
		{
			return math.lerp(start, end, coord);
		}

		public float2 Tangent(float coord)
		{
			return math.normalize(end - start);
		}

		public float Length()
		{
			return math.length(end - start);
		}

		public ICurve2D OffsetCurve(float offset)
		{
			return OffsetLine(offset);
		}

		public IEnumerable<float> IntersectCoord(ICurve2D other)
		{
			var length = math.length(end - start);
			foreach (var result in Intersect(other))
			{
				yield return math.length(result - start) / length;
			}
		}

		public IEnumerable<float2> Intersect(ICurve2D other)
		{
			if (other is LineSegment2D line)
			{
				if (IntersectLine(line, out var point))
				{
					yield return point;
				}
			}
			else if (other is Circle2D circle)
			{
				foreach (var result in circle.Intersect(this))
				{
					yield return result;
				}

				;
			}
			else if (other is CircularArc2D arc)
			{
				var arcCirle = new Circle2D(arc.center, arc.radius);
				foreach (var result in arcCirle.Intersect(this))
				{
					if (arc.ContainsPoint(result))
					{
						yield return result;
					}
				}

				;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public bool IntersectLine(in LineSegment2D other, out float2 point)
		{
			if (LinesIntersect2D(start, end, other.start, other.end,
				    true, true, true, out point, VectorExtensions.Epsilon))
			{
				return true;
			}

			point = default;
			return false;
		}

		public LineSegment2D OffsetLine(float offset)
		{
			var normal = this.Normal(0f);
			return new LineSegment2D
			{
				start = start + normal * offset,
				end = end + normal * offset
			};
		}

		private static bool LinesIntersect2D(float2 ptStart0, float2 ptEnd0,
			float2 ptStart1, float2 ptEnd1,
			bool firstIsSegment, bool secondIsSegment, bool coincidentEndPointCollisions,
			out float2 pIntersectionPt,
			float epsilon)
		{
			var d = (ptEnd0.x - ptStart0.x) * (ptStart1.y - ptEnd1.y) -
			        (ptStart1.x - ptEnd1.x) * (ptEnd0.y - ptStart0.y);
			if (math.abs(d) < epsilon)
			{
				//The lines are parallel.
				pIntersectionPt = default;
				return false;
			}

			var d0 = (ptStart1.x - ptStart0.x) * (ptStart1.y - ptEnd1.y) -
			         (ptStart1.x - ptEnd1.x) * (ptStart1.y - ptStart0.y);
			var d1 = (ptEnd0.x - ptStart0.x) * (ptStart1.y - ptStart0.y) -
			         (ptStart1.x - ptStart0.x) * (ptEnd0.y - ptStart0.y);
			var kOneOverD = 1 / d;
			var t0 = d0 * kOneOverD;
			var t1 = d1 * kOneOverD;

			if ((!firstIsSegment || (t0 >= 0.0 && t0 <= 1.0)) &&
			    (!secondIsSegment || (t1 >= 0.0 && t1 <= 1.0)) &&
			    (coincidentEndPointCollisions || (!(math.abs(t0) < epsilon) && !(math.abs(t1) < epsilon))))
			{
				pIntersectionPt = new float2
				{
					x = ptStart0.x + t0 * (ptEnd0.x - ptStart0.x),
					y = ptStart0.y + t0 * (ptEnd0.y - ptStart0.y)
				};

				return true;
			}

			pIntersectionPt = default;
			return false;
		}
	}
}
