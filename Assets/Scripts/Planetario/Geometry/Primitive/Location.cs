using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Planetario.Geometry.Primitive
{
	/// <summary>
	///     3D Position with Rotation
	///     Similar to a Transform without a scale
	/// </summary>
	[Serializable]
	public struct Location : IComponentData, IEquatable<Location>
	{
		[FormerlySerializedAs("Position")] public float3 position;
		[FormerlySerializedAs("Rotation")] public quaternion rotation;

		public Location(float3 position, quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public Location(float3 position)
		{
			this.position = position;
			rotation = quaternion.identity;
		}

		public Location(Transform transform)
		{
			position = transform.position;
			rotation = transform.rotation;
		}

		public bool Equals(Location other)
		{
			return position.Equals(other.position) && rotation.Equals(other.rotation);
		}

		public static implicit operator Location(Transform source)
		{
			return new Location(source);
		}

		public Location Offset(float3 direction)
		{
			return new Location
			{
				position = position + direction,
				rotation = rotation
			};
		}

		public float3 Forward()
		{
			return math.forward(rotation);
		}

		public float3 Right()
		{
			return math.mul(rotation, math.right());
		}

		public float3 Up()
		{
			return math.mul(rotation, math.up());
		}

		public override bool Equals(object obj)
		{
			return obj is Location other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (position.GetHashCode() * 397) ^ rotation.GetHashCode();
			}
		}
	}
}
