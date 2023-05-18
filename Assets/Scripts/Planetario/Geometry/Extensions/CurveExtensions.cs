using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Planetario.Geometry.Extensions
{
	public static class CurveExtensions
	{
		/// <summary>
		///     Approximate the length by divide the curve into
		///     <param name="segmentCount">
		///         and sum their lengths
		///     </param>
		/// </summary>
		public static float ApproximateLength<T>(this T curve, int segmentCount) where T : ICurve3D
		{
			float totalLen = 0;
			var lastPos = curve.Point(0f);
			for (var i = 1; i <= segmentCount; i++)
			{
				var curPos = curve.Point((float)i / segmentCount);
				totalLen += math.length(curPos - lastPos);
				lastPos = curPos;
			}

			return totalLen;
		}

		/// <summary>
		///     Normal vector of the curve at coordinate
		///     <param name="coord"></param>
		/// </summary>
		public static float3 Normal(this ICurve3D curve, float coord, float3 up)
		{
			return math.normalize(math.cross(curve.Tangent(coord), up));
		}

		/// <summary>
		///     This is different than the up vector passed into <see cref="Normal" /> method
		///     This is the up vector that are both perpendicular to Tangent and Normal
		/// </summary>
		public static float3 TrueUp(this ICurve3D curve, float coord, float3 up)
		{
			var tangent = curve.Tangent(coord);
			var normal = math.cross(tangent, up);
			return math.normalize(math.cross(normal, tangent));
		}

		/// <summary>
		///     Compute offset position
		/// </summary>
		public static float3 Offset<T>(this T curve, float coord, float3 up, float offset) where T : ICurve3D
		{
			return curve.Point(coord) + curve.Normal(coord, up) * offset;
		}

		public static IEnumerable<T> Points<T>(this ICurve<T> curve, int segmentCount)
		{
			for (var i = 0; i <= segmentCount; i++)
			{
				var t = (float)i / segmentCount;
				yield return curve.Point(t);
			}
		}

		public static IEnumerable<float3> Offsets<T>(this T curve, int segmentCount, float3 up, float offset)
			where T : ICurve3D
		{
			for (var i = 0; i <= segmentCount; i++)
			{
				var t = (float)i / segmentCount;
				yield return curve.Offset(t, up, offset);
			}
		}

		public static IEnumerable<(T, T)> Segments<T>(this IEnumerable<T> points)
		{
			var first = false;
			T prev = default;
			foreach (var point in points)
			{
				if (first)
				{
					yield return (prev, point);
					prev = point;
				}
				else
				{
					prev = point;
					first = true;
				}
			}
		}

		public static float Deviance<T>(this IEnumerable<(T, T)> segments)
		{
			var min = float.MaxValue;
			var max = float.MinValue;

			foreach (var (prev, cur) in segments)
			{
				float length;
				if (cur is float2 cur2D && prev is float2 prev2D)
				{
					length = math.length(cur2D - prev2D);
				}
				else if (cur is float3 cur3D && prev is float3 prev3D)
				{
					length = math.length(cur3D - prev3D);
				}
				else
				{
					throw new NotSupportedException("Only support float2 and float3");
				}

				if (length < min)
				{
					min = length;
				}

				if (length > max)
				{
					max = length;
				}
			}

			return (max - min) / min;
		}
	}
}
