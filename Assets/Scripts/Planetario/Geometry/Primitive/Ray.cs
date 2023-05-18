using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	[Serializable]
	public struct Ray : IComponentData, IClosestPoint
	{
		public float3 origin;

		/// <summary>
		///     Direction must be a unit vector
		/// </summary>
		public float3 direction;

		/// <summary>
		///     <para>Creates a ray starting at origin along direction.</para>
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="direction"></param>
		public Ray(float3 origin, float3 direction)
		{
			this.origin = origin;
			this.direction = math.normalize(direction);
		}

		public Ray(UnityEngine.Ray other)
		{
			origin = other.origin;
			direction = other.direction;
		}

		public readonly float3 ClosestPointTo(float3 point)
		{
			return GetPoint(ClosestCoordTo(point));
		}

		public static implicit operator Ray(UnityEngine.Ray source)
		{
			return new Ray(source);
		}

		public static implicit operator UnityEngine.Ray(Ray source)
		{
			return new UnityEngine.Ray(source.origin, source.direction);
		}

		/// <summary>
		///     <para>Returns a point at distance units along the ray.</para>
		/// </summary>
		/// <param name="distance"></param>
		public readonly float3 GetPoint(float distance)
		{
			return origin + direction * distance;
		}

		public readonly float ClosestCoordTo(float3 point)
		{
			var pointDirection = point - origin;
			var distanceAlongLine = math.dot(pointDirection, direction);
			return distanceAlongLine;
		}

		public readonly float ClosestCoordTo(in Ray another)
		{
			//algorithm here:
			//https://math.stackexchange.com/questions/1033419/line-perpendicular-to-two-other-lines-data-sufficiency

			var rayOrigin = another.origin;
			var rayDir = another.direction;

			var UU = math.dot(direction, direction);
			var UV = math.dot(direction, rayDir);
			var QP = rayOrigin - origin;
			var UQ = math.dot(direction, QP);
			var VV = math.dot(rayDir, rayDir);
			var VQ = math.dot(rayDir, QP);

			var coord = (UQ * VV - VQ * UV) / (UU * VV - UV * UV);
			return coord;
		}

		public readonly float3 ClosestPointTo(in Ray another)
		{
			var coord = ClosestCoordTo(in another);
			return GetPoint(coord);
		}
	}
}
