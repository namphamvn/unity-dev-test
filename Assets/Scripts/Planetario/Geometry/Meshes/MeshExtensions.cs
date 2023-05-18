using Planetario.Geometry.Extensions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry.Meshes
{
	public static class MeshExtensions
	{
		public static MeshData ComputeCurveExtrude<T>(float curveLength, T curve,
			float segmentLength, float segmentWidth) where T : ICurve3D
		{
			var mesh = new MeshData(Allocator.Temp);
			var segmentCount = (int)(curveLength / segmentLength);
			var prevL = -1;
			var prevR = -1;
			for (var i = 0; i <= segmentCount; i++)
			{
				var t = (float)i / segmentCount;
				var p = curve.Point(t);
				var tangent = curve.Tangent(t);

				var up = new float3(0, 1, 0);
				var normal = curve.Normal(t, up);
				var halfWidth = normal * segmentWidth * 0.5f;
				var l0 = p + halfWidth;
				var r0 = p - halfWidth;
				var newL = mesh.AddVertex(l0, Vector3.up, Color.white, Vector4.zero);
				var newR = mesh.AddVertex(r0, Vector3.up, Color.white, Vector4.zero);

				if (i > 0)
				{
					mesh.AddTriangle(newL, prevR, prevL);
					mesh.AddTriangle(newR, prevR, newL);
				}

				prevL = newL;
				prevR = newR;
			}

			return mesh;
		}

		public static MeshData ComputeSurfaceMesh(float2[] points, Transform transform)
		{
			var triangulator = new Triangulator(points);
			var data = new MeshData(Allocator.Temp);
			for (var i = 0; i < points.Length; i++)
			{
				data.AddVertex(transform.InverseTransformPoint(points[i].To3D()));
			}

			var indices = triangulator.Triangulate();
			for (var i = 0; i < indices.Length; i++)
			{
				data.Indices.Add(indices[i]);
			}

			return data;
		}

		public static MeshData ComputeStripMesh(ICurve3D startCurve, ICurve3D endCurve,
			int segmentCount, Transform transform)
		{
			var data = new MeshData(Allocator.Temp);
			var prev0 = AddVertex(startCurve, 0);
			var prev1 = AddVertex(endCurve, 0);

			for (var i = 1; i <= segmentCount; i++)
			{
				var coord = (float)i / segmentCount;
				var cur0 = AddVertex(startCurve, coord);
				var cur1 = AddVertex(endCurve, coord);

				data.AddTriangle(cur1, prev1, prev0);
				data.AddTriangle(prev0, cur0, cur1);

				prev0 = cur0;
				prev1 = cur1;
			}

			int AddVertex(ICurve3D curve, float coord)
			{
				return data.AddVertex(LocalPoint(curve, coord), startCurve.TrueUp(coord, math.up()));
			}

			Vector3 LocalPoint(ICurve3D curve, float coord)
			{
				var wPos = curve.Point(coord);
				return transform?.InverseTransformPoint(wPos) ?? wPos;
			}

			return data;
		}


		/// <summary>
		///     Set all vertices to be in a sphere from the the origin (0, 0, 0)
		/// </summary>
		public static void NormaliseVertices(this MeshData input, float radius)
		{
			for (var i = 0; i < input.Vertices.Length; i++)
			{
				input.Vertices[i] = math.normalize(input.Vertices[i]) * radius;
			}
		}
	}
}
