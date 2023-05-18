using System;
using System.Collections.Generic;
using Planetario.Geometry.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	[Serializable]
	public struct Plane : IComponentData, IRaycastable, IClosestPoint
	{
		/// <summary>
		///     <para>Normal vector of the plane.</para>
		/// </summary>
		public float3 normal;

		/// <summary>
		///     <para>The distance measured from the Plane to the origin, along the Plane's normal.</para>
		/// </summary>
		public float distance;

		/// <summary>
		///     <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="inPoint"></param>
		public Plane(float3 inNormal, float3 inPoint)
		{
			normal = math.normalize(inNormal);
			distance = -math.dot(normal, inPoint);
		}

		/// <summary>
		///     <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="d"></param>
		public Plane(float3 inNormal, float d)
		{
			normal = math.normalize(inNormal);
			distance = d;
		}

		/// <summary>
		///     <para>Creates a plane.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public Plane(float3 a, float3 b, float3 c)
		{
			normal = math.normalize(math.cross(b - a, c - a));
			distance = -math.dot(normal, a);
		}

		/// <summary>
		///     <para>Returns a copy of the plane that faces in the opposite direction.</para>
		/// </summary>
		public Plane Flipped => new(-normal, -distance);

		public float3 Origin => normal * -distance;

		/// <summary>
		///     <para>For a given point returns the closest point on the plane.</para>
		/// </summary>
		/// <param name="point">The point to project onto the plane.</param>
		/// <returns>
		///     <para>A point on the plane that is closest to point.</para>
		/// </returns>
		public float3 ClosestPointTo(float3 point)
		{
			var num = math.dot(normal, point) + distance;
			return point - normal * num;
		}

		public bool Raycast(Ray ray, out float enter)
		{
			var a = math.dot(ray.direction, normal);
			if (a.AlmostEqual(0f))
			{
				enter = 0.0f;
				return false;
			}

			var num = -math.dot(ray.origin, normal) - distance;
			enter = num / a;
			return enter > 0f;
		}

		/// <summary>
		///     <para>Sets a plane using a point that lies within it along with a normal to orient it.</para>
		/// </summary>
		/// <param name="inNormal">The plane's normal vector.</param>
		/// <param name="inPoint">A point that lies on the plane.</param>
		public void SetNormalAndPosition(float3 inNormal, float3 inPoint)
		{
			normal = math.normalize(inNormal);
			distance = -math.dot(inNormal, inPoint);
		}

		/// <summary>
		///     <para>
		///         Sets a plane using three points that lie within it.  The points go around clockwise as you look
		///         down on the top surface of the plane.
		///     </para>
		/// </summary>
		/// <param name="a">First point in clockwise order.</param>
		/// <param name="b">Second point in clockwise order.</param>
		/// <param name="c">Third point in clockwise order.</param>
		public void Set3Points(float3 a, float3 b, float3 c)
		{
			normal = math.normalize(math.cross(b - a, c - a));
			distance = -math.dot(normal, a);
		}

		/// <summary>
		///     <para>Makes the plane face in the opposite direction.</para>
		/// </summary>
		public void Flip()
		{
			normal = -normal;
			distance = -distance;
		}

		/// <summary>
		///     <para>Moves the plane in space by the translation vector.</para>
		/// </summary>
		/// <param name="translation">The offset in space to move the plane with.</param>
		public void Translate(float3 translation)
		{
			distance += math.dot(normal, translation);
		}

		/// <summary>
		///     <para>Returns a copy of the given plane that is moved in space by the given translation.</para>
		/// </summary>
		/// <param name="plane">The plane to move in space.</param>
		/// <param name="translation">The offset in space to move the plane with.</param>
		/// <returns>
		///     <para>The translated plane.</para>
		/// </returns>
		public static Plane Translate(Plane plane, float3 translation)
		{
			return new Plane(plane.normal, plane.distance += math.dot(plane.normal, translation));
		}

		/// <summary>
		///     <para>Returns a signed distance from plane to point.</para>
		/// </summary>
		/// <param name="point"></param>
		public float GetDistanceToPoint(float3 point)
		{
			return math.dot(normal, point) + distance;
		}

		/// <summary>
		///     <para>Is a point on the positive side of the plane?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool IsOnPositiveSide(float3 point)
		{
			return math.dot(normal, point) + (double)distance > 0.0;
		}

		/// <summary>
		///     <para>Are two points on the same side of the plane?</para>
		/// </summary>
		/// <param name="inPt0"></param>
		/// <param name="inPt1"></param>
		public bool SameSide(float3 inPt0, float3 inPt1)
		{
			var distanceToPoint1 = GetDistanceToPoint(inPt0);
			var distanceToPoint2 = GetDistanceToPoint(inPt1);
			return (distanceToPoint1 > 0f
			        && distanceToPoint2 > 0f)
			       || (distanceToPoint1 <= 0f
			           && distanceToPoint2 <= 0f);
		}

		public Func<float, float, float3> GetCoordFunc()
		{
			var (forward, right) = normal.FromUpToForwardRight();
			var origin = Origin;
			return (x, z) => origin + right * x + forward * z;
		}

		public IEnumerable<float3> GetGizmoPoints(float size)
		{
			var coordFunc = GetCoordFunc();
			yield return coordFunc(-size, -size);
			yield return coordFunc(+size, -size);
			yield return coordFunc(+size, +size);
			yield return coordFunc(-size, +size);
		}
	}
}
