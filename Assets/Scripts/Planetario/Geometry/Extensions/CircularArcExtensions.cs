using System.Collections.Generic;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.Geometry.Extensions
{
	public static class CircularArcExtensions
	{
		public static CircularArc ComputeArc(float3 p1, float3 p2, float3 p3)
		{
			var v1 = p2 - p1;
			var v2 = p3 - p1;
			float v1v1, v2v2, v1v2;
			v1v1 = math.dot(v1, v1);
			v2v2 = math.dot(v2, v2);
			v1v2 = math.dot(v1, v2);

			var b = 0.5f / (v1v1 * v2v2 - v1v2 * v1v2);
			var k1 = b * v2v2 * (v1v1 - v1v2);
			var k2 = b * v1v1 * (v2v2 - v1v2);
			var center = p1 + v1 * k1 + v2 * k2;
			var radius = math.length(p1 - center);
			var right = math.normalize(p1 - center);
			var p3center = p3 - center;
			// var up = math.normalize(math.cross(right, p3center));
			var up = math.normalize(math.cross(p1 - p2, p3 - p2));
			var forward = math.normalize(math.cross(right, up));
			var x = math.dot(p3center, right);
			var z = math.dot(p3center, forward);
			var angle = math.atan2(z, x);
			if (angle < 0f)
			{
				angle += 2 * math.PI;
			}

			return new CircularArc
			{
				center = center,
				right = right * radius,
				forward = forward * radius,
				up = up,
				angle = angle
			};
		}

		/// <summary>
		///     calculate the error between spline and circular arc, from start to end
		/// </summary>
		private static float ErrorToArc(this ICurve3D curve, CircularArc arc, float3 pointAtS, float start,
			float end)
		{
			var q = (end - start) / 4;
			var c1 = curve.Point(start + q);
			var c2 = curve.Point(end - q);
			var referenceDist = math.distance(arc.center, pointAtS);
			var d1 = math.distance(arc.center, c1);
			var d2 = math.distance(arc.center, c2);
			return math.abs(d1 - referenceDist) + math.abs(d2 - referenceDist);
		}

		public static IEnumerable<CircularArc> ToArcs(this ICurve3D curve, float errorThreshold = 0.5f)
		{
			float coordStart = 0;
			var coordEnd = 1f;
			int safety;
			// we do a binary search to find the "good `t` closest to no-longer-good"
			do
			{
				safety = 0;

				// step 1: start with the maximum possible arc
				coordEnd = 1;

				// points:
				var pStart = curve.Point(coordStart);
				float3 pMid;
				float3 pEnd;
				CircularArc arc;

				// booleans:
				var curr_good = false;
				var prev_good = false;
				bool done;

				// numbers:
				float coordMiddle;
				var prev_end = coordEnd;

				// step 2: find the best possible arc
				do
				{
					prev_good = curr_good;
					// prev_arc = arc;
					coordMiddle = (coordStart + coordEnd) / 2;

					pMid = curve.Point(coordMiddle);
					pEnd = curve.Point(coordEnd);

					arc = ComputeArc(pStart, pMid, pEnd);

					var error = curve.ErrorToArc(arc, pStart, coordStart, coordEnd);
					curr_good = error <= errorThreshold;

					done = prev_good && !curr_good;
					// if (!done) prev_end = coordEnd;

					// this arc is fine: we can move 'e' up to see if we can find a wider arc
					if (curr_good)
					{
						// // if e is already at max, then we're done for this arc.
						// if (coordEnd >= 1)
						// {
						// 	break;
						// }
						//
						// // if not, move it up by half the iteration distance
						// coordEnd = coordEnd + (coordEnd - coordStart) / 2;
						done = true;
						prev_end = coordEnd;
					}
					else
					{
						// this is a bad arc: we need to move 'e' down to find a good arc
						coordEnd = coordMiddle;
					}
				} while (!done && safety++ < 100);

				if (safety >= 100)
				{
					break;
				}

				yield return arc;
				coordStart = prev_end;
			} while (coordEnd < 1);
		}

		public static float Sign(float val)
		{
			return val < 0.0f ? -1.0f : 1.0f;
		}

		//******************************************************************************
		// Compute a single arc based on an end point and the vector from the endpoint
		// to connection point. 
		//******************************************************************************
		public static ICurve3D ComputeArcByTangent(
			in float3 point,
			in float3 tangent,
			in float3 pointToMid,
			bool largeAngle,
			bool flip)
		{
			// assume that the tangent is normalized
			// assert(Vec_IsNormalized_Eps(tangent, 0.01f));

			// compute the normal to the arc plane
			var normal = math.cross(pointToMid, tangent);

			// Compute an axis within the arc plane that is perpendicular to the
			// tangent. This will be coliniear with the vector from the center to
			// the end point.
			var perpAxis = math.cross(tangent, normal);

			var denominator = 2.0f * math.dot(perpAxis, pointToMid);

			if (math.abs(denominator) < VectorExtensions.Epsilon)
			{
				// The radius is infinite, so use a straight line. Place the center
				// in the middle of the line.
				return flip
					? new LineSegment
					{
						start = point + pointToMid,
						end = point
					}
					: new LineSegment
					{
						start = point,
						end = point + pointToMid
					};
			}

			// Compute the distance to the center along perpAxis
			var centerDist = math.dot(pointToMid, pointToMid) / denominator;
			var pCenter = point + perpAxis * centerDist;

			// Compute the radius in absolute units
			var perpAxisMag = math.length(perpAxis);
			var radius = math.abs(centerDist * perpAxisMag);

			// Compute the arc angle
			float angle;
			if (radius < VectorExtensions.Epsilon)
			{
				angle = 0.0f;
			}
			else
			{
				var invRadius = 1.0f / radius;

				// Compute normalized directions from the center to the connection
				// point and from the center to the end point.
				var centerToMidDir = point - pCenter;
				var centerToEndDir = centerToMidDir * invRadius;
				centerToMidDir += pointToMid;
				centerToMidDir *= invRadius;

				// Compute the rotation direction
				var twist = math.dot(perpAxis, pointToMid);

				// Compute angle.
				angle = math.acos(math.dot(centerToEndDir, centerToMidDir)) * Sign(twist);
			}

			// output the radius and angle
			// var pRadius = radius;
			// var pAngle = angle;

			if (largeAngle)
			{
				angle = Sign(angle) * math.PI * 2f - angle;
			}

			var arc = new CircularArc
			{
				center = pCenter,
				right = point - pCenter,
				forward = tangent * radius,
				up = normal,
				angle = angle
			};
			return flip ? arc.Flip() : arc;
		}

		/// <summary>
		///     Compute a pair of arcs
		///     http://www.ryanjuckett.com/programming/biarc-interpolation/
		/// </summary>
		/// <param name="pArc1">arc1 output</param>
		/// <param name="pArc2">arc2 output</param>
		/// <param name="p1">point 1</param>
		/// <param name="t1">tangent 1, must be normalised</param>
		/// <param name="p2">point 2</param>
		/// <param name="t2">tangent 2, must be normalised</param>
		public static void ComputeBiArc(
			out ICurve3D pArc1,
			out ICurve3D pArc2,
			in float3 p1,
			in float3 t1,
			in float3 p2,
			in float3 t2)
		{
			// assert(Vec_IsNormalized_Eps(t1, 0.01f));
			// assert(Vec_IsNormalized_Eps(t2, 0.01f));

			var v = p2 - p1;

			var vDotV = math.dot(v, v);

			// if the control points are equal, we don't need to interpolate
			if (vDotV < VectorExtensions.Epsilon)
			{
				pArc1 = new LineSegment
				{
					start = p1,
					end = p2
				};
				pArc2 = null;
				return;
			}

			// compute the denominator for the quadratic formula
			var t = t1 + t2;

			var vDotT = math.dot(v, t);
			var t1DotT2 = math.dot(t1, t2);
			var denominator = ComputeDenominator(t1DotT2);

			// if the quadratic formula denominator is zero, the tangents are equal
			// and we need a special case
			float d;
			if (denominator < VectorExtensions.Epsilon)
			{
				var vDotT2 = math.dot(v, t2);

				// if the special case d is infinity, the only solution is to
				// interpolate across two semicircles
				if (math.abs(vDotT2) < VectorExtensions.Epsilon)
				{
					var vMag = math.sqrt(vDotV);
					var invVMagSqr = 1.0f / vDotV;

					// compute the normal to the plane containing the arcs
					// (this has length vMag)
					var planeNormal = math.cross(v, t2);

					// compute the axis perpendicular to the tangent direction and
					// aligned with the circles (this has length vMag*vMag)
					var perpAxis = math.cross(planeNormal, v);

					var radius = vMag * 0.25f;

					var centerToP1 = v * -0.25f;

					// interpolate across two semicircles
					var arc1 = new CircularArc
					{
						center = p1 - centerToP1,
						right = centerToP1,
						forward = perpAxis * (radius * invVMagSqr),
						angle = math.PI
					};
					arc1.up = math.normalize(math.cross(arc1.forward, arc1.right));
					pArc1 = arc1;

					var arc2 = new CircularArc
					{
						center = p2 + centerToP1,
						right = centerToP1,
						forward = perpAxis * (-radius * invVMagSqr),
						angle = math.PI
					};
					arc2.up = math.normalize(math.cross(arc2.forward, arc2.right));
					pArc2 = arc2;

					return;
				}

				var t1DotV = math.dot(t1, v);
				if (math.abs(ComputeDenominator(t1DotV)) < VectorExtensions.Epsilon)
				{
					pArc1 = new LineSegment
					{
						start = p1,
						end = p2
					};
					pArc2 = null;
					return;
				}

				// compute distance value for equal tangents
				d = vDotV / (4.0f * vDotT2);
			}
			else
			{
				// use the positive result of the quadratic formula
				var discriminant = vDotT * vDotT + denominator * vDotV;
				d = (-vDotT + math.sqrt(discriminant)) / denominator;
			}

			// compute the connection point (i.e. the mid point)
			var pm = t1 - t2;
			pm = p2 + pm * d;
			pm += p1;
			pm *= 0.5f;

			// compute vectors from the end points to the mid point
			var p1ToPm = pm - p1;
			var p2ToPm = pm - p2;

			var largeAngle = d < 0.0f;

			pArc1 = ComputeArcByTangent(p1, t1, p1ToPm, largeAngle, false);
			pArc2 = ComputeArcByTangent(p2, -t2, p2ToPm, largeAngle, true);
			// pArc1->m_arcLen = (radius1 == 0.0f) ? Vec_Magnitude(p1ToPm) : fabs(radius1 * angle1);
		}

		public static IEnumerable<ICurve3D> ComputeBiArc(
			float3 p1,
			float3 t1,
			float3 p2,
			float3 t2)
		{
			ComputeBiArc(out var c1, out var c2, p1, t1, p2, t2);
			yield return c1;
			if (c2 != null)
			{
				yield return c2;
			}
		}

		public static float ComputeDenominator(float input)
		{
			return 2.0f * (1.0f - input);
		}

		private static CircularArc Flip(this CircularArc input)
		{
			var endPoint = input.Point(1f);
			var right = endPoint - input.center;
			var up = -input.up;
			return new CircularArc
			{
				center = input.center,
				up = up, //y
				angle = input.angle,
				right = right, //x
				forward = math.normalize(math.cross(right, up)) *
				          input.Radius() //up is unit vector, forward will take length from right
			};
		}
	}
}
