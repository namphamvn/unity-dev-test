using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry
{
	public static class GizmosExtensions
	{
		public static IEnumerable<(float3 start, float3 end)> GetGizmoLines(this IPoints geometry)
		{
			using var points = geometry.GetPoints().GetEnumerator();
			points.MoveNext();
			var first = points.Current;
			var prev = first;
			float3 current = default;
			while (points.MoveNext())
			{
				current = points.Current;
				yield return (prev, current);
				prev = current;
			}

			yield return (current, first);
		}

		public static void DrawGizmo(this IPoints geometry, Color color)
		{
			Gizmos.color = color;
			foreach (var segment in geometry.GetGizmoLines())
			{
				Gizmos.DrawLine(segment.start, segment.end);
			}
		}
	}
}
