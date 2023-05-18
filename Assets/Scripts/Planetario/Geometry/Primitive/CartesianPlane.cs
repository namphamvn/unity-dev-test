using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry.Primitive
{
	[Serializable]
	public struct CartesianPlane : IComponentData, IRaycastable
	{
		public float3 origin;
		public float3 right;
		public float3 forward;

		public CartesianPlane(float3 origin, float3 right, float3 forward)
		{
			this.origin = origin;
			this.right = right;
			this.forward = forward;
		}

		public CartesianPlane(Transform transform)
		{
			origin = transform.position;
			right = transform.right;
			forward = transform.forward;
		}

		public readonly bool Raycast(Ray ray, out float enter)
		{
			return GetPlane().Raycast(ray, out enter);
		}

		public readonly float3 GetPoint(float2 coord)
		{
			return origin + right * coord.x + forward * coord.y;
		}

		public readonly float3 GetPoint3d(float3 coord)
		{
			return origin + right * coord.x + GetUp() * coord.y + forward * coord.z;
		}

		public readonly float3 GetPoint3d(float2 coordXZ, float y)
		{
			return origin + right * coordXZ.x + GetUp() * y + forward * coordXZ.y;
		}

		public readonly float3 GetUp()
		{
			return math.cross(forward, right);
		}

		/// <summary>
		///     Project the 3d position to the plane and calculate the coordinate
		/// </summary>
		public readonly float2 GetCoord(float3 position)
		{
			var dir = position - origin;
			return new float2(math.dot(dir, right), math.dot(dir, forward));
		}

		/// <summary>
		///     Project the 3d position to the plane and calculate the coordinate
		/// </summary>
		public readonly float3 GetCoord3d(float3 position)
		{
			var dir = position - origin;
			return new float3(math.dot(dir, right),
				math.dot(dir, GetUp()),
				math.dot(dir, forward));
		}

		private readonly Plane GetPlane()
		{
			return new Plane(GetUp(), origin);
		}
	}
}
