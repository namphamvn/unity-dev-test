#define USE_CIRCULAR_ARC_2D_ANGLE

using System.Collections.Generic;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.Geometry.Extensions
{
	public static class CircularArc2DExtensions
	{
		//******************************************************************************
		// Compute a single arc based on an end point and the vector from the endpoint
		// to connection point. 
		//******************************************************************************
		public static ICurve2D ComputeArcByTangent(
			in float2 point,
			in float2 tangent,
			in float2 pointToMid,
			bool largeAngle,
			bool flip)
		{
			// assume that the tangent is normalized
			// assert(Vec_IsNormalized_Eps(tangent, 0.01f));

			// compute the normal to the arc plane
			var pointToMid3D = pointToMid.To3D();
			var tangent3D = tangent.To3D();
			var normal = math.cross(pointToMid3D, tangent3D);

			// Compute an axis within the arc plane that is perpendicular to the
			// tangent. This will be coliniear with the vector from the center to
			// the end point.
			var perpAxis = math.cross(tangent3D, normal).To2D();

			var denominator = 2.0f * math.dot(perpAxis, pointToMid);

			if (math.abs(denominator) < VectorExtensions.Epsilon)
			{
				// The radius is infinite, so use a straight line. Place the center
				// in the middle of the line.
				return flip
					? new LineSegment2D
					{
						start = point + pointToMid,
						end = point
					}
					: new LineSegment2D
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
				angle = math.acos(math.dot(centerToEndDir, centerToMidDir)) *
				        CircularArcExtensions.Sign(twist);
			}

			// output the radius and angle
			// var pRadius = radius;
			// var pAngle = angle;

			if (largeAngle)
			{
				angle = CircularArcExtensions.Sign(angle) * math.PI * 2f - angle;
			}
#if USE_CIRCULAR_ARC_2D_ANGLE
			var fromCenter = point - pCenter;
			if (math.cross(tangent.To3D(), fromCenter.To3D()).y < 0f)
			{
				angle = -angle;
			}

			var arc = new CircularArc2D(pCenter, fromCenter, angle);
#else
				var arc = new CircularArc2D(pCenter, point - pCenter, tangent * radius, angle);
#endif
			return flip ? arc.Flip() : arc;
		}

		public static IEnumerable<ICurve2D> ComputeBiArc(
			float2 p1,
			float2 t1,
			float2 p2,
			float2 t2)
		{
			ComputeBiArc(out var c1, out var c2, p1, t1, p2, t2);
			yield return c1;
			if (c2 != null)
			{
				yield return c2;
			}
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
			out ICurve2D pArc1,
			out ICurve2D pArc2,
			in float2 p1,
			in float2 t1,
			in float2 p2,
			in float2 t2)
		{
			// assert(Vec_IsNormalized_Eps(t1, 0.01f));
			// assert(Vec_IsNormalized_Eps(t2, 0.01f));

			var v = p2 - p1;

			var vDotV = math.dot(v, v);

			// if the control points are equal, we don't need to interpolate
			if (vDotV < VectorExtensions.Epsilon)
			{
				pArc1 = new LineSegment2D
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
			var denominator = CircularArcExtensions.ComputeDenominator(t1DotT2);

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
					var v3D = v.To3D();
					var planeNormal = math.cross(v3D, t2.To3D());

					// compute the axis perpendicular to the tangent direction and
					// aligned with the circles (this has length vMag*vMag)
					var perpAxis = math.cross(planeNormal, v3D).To2D();

					var radius = vMag * 0.25f;

					var centerToP1 = v * -0.25f;

					// interpolate across two semicircles
#if USE_CIRCULAR_ARC_2D_ANGLE
					var arc1 = new CircularArc2D(p1 - centerToP1, centerToP1, math.PI);
					var arc2 = new CircularArc2D(p2 + centerToP1, centerToP1, -math.PI);
#else
					var arc1 =
 new CircularArc2D(p1 - centerToP1, centerToP1, perpAxis * (radius * invVMagSqr),
						math.PI);
					var arc2 =
 new CircularArc2D(p2 + centerToP1, centerToP1, perpAxis * (-radius * invVMagSqr),
						math.PI);
#endif

					pArc1 = arc1;
					pArc2 = arc2;
					return;
				}

				var t1DotV = math.dot(t1, v);
				if (math.abs(CircularArcExtensions.ComputeDenominator(t1DotV)) < VectorExtensions.Epsilon)
				{
					pArc1 = new LineSegment2D
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
	}
}
