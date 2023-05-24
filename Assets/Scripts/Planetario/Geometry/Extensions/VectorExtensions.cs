using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry.Extensions
{
	public static class VectorExtensions
	{
		public const float Epsilon = 0.0001f;
		public const float PIx2 = math.PI * 2f;
		public const int ComputeLengthSegmentCount = 100;

		/// <summary>
		///     This is used to convert float into integer. 1 unit of float is converted to 1000 units of integer
		/// </summary>
		public const int IntGranularity = 1000;

		public static int DistanceTo(this float3 from, float3 to)
		{
			return (int)(math.distance(from, to) * IntGranularity);
		}

		public static float2 To2D(this float3 value)
		{
			return value.xz;
		}

		public static float2 To2D(this Vector3 value)
		{
			return new float2(value.x, value.z);
		}

		public static float3 To3D(this float2 value)
		{
			return new float3(value.x, 0f, value.y);
		}

		public static bool AlmostEqual(this float value, float other)
		{
			var dist = math.abs(value - other);
			return dist < Epsilon;
		}

		public static bool AlmostEqual(this float2 value, float2 other)
		{
			var dist = math.abs(value - other);
			return dist.x < Epsilon && dist.y < Epsilon;
		}

		public static quaternion RotateTowards(this float3 from, float3 to)
		{
			return quaternion.AxisAngle(
				angle: math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1f, 1f)),
				axis: math.normalize(math.cross(from, to))
			);
		}

		public static (float3 forward, float3 right) FromUpToForwardRight(this float3 up)
		{
			var compare = up == math.up();
			float3 forward, right;
			if (compare.x & compare.y & compare.z)
			{
				forward = math.forward();
				right = math.right();
			}
			else
			{
				compare = up == math.right();
				if (compare.x & compare.y & compare.z)
				{
					forward = math.cross(math.forward(), up);
				}
				else
				{
					forward = math.cross(math.right(), up);
				}
				right = math.cross(up, forward);
			}

			return (forward, right);
		}
	}
}
