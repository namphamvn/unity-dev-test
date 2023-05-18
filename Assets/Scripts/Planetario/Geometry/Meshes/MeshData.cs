using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry.Meshes
{
	public struct MeshData : IDisposable
	{
		public NativeList<int> Indices;
		public NativeList<Vector3> Vertices;
		public NativeList<Vector3> Normals;
		public NativeList<Color> Colors;
		public NativeList<Vector4> UVs;

		public MeshData(Allocator allocator = Allocator.Temp)
		{
			Indices = new NativeList<int>(allocator);
			Vertices = new NativeList<Vector3>(allocator);
			Colors = new NativeList<Color>(allocator);
			Normals = new NativeList<Vector3>(allocator);
			UVs = new NativeList<Vector4>(allocator);
		}

		public MeshData(in MeshData source, Allocator allocator = Allocator.Temp, bool includeIndices = false)
		{
			Indices = includeIndices ? CopyFrom(source.Indices, allocator) : new NativeList<int>(allocator);
			Vertices = CopyFrom(source.Vertices, allocator);
			Colors = CopyFrom(source.Colors, allocator);
			Normals = CopyFrom(source.Normals, allocator);
			UVs = CopyFrom(source.UVs, allocator);
		}

		private static NativeList<T> CopyFrom<T>(NativeList<T> source, Allocator allocator = Allocator.Temp)
			where T : unmanaged
		{
			var result = new NativeList<T>(source.Length, allocator);
			for (var i = 0; i < source.Length; i++)
			{
				result.Add(source[i]);
			}

			//result.CopyFrom(source.AsArray()); //this is buggy, it triggers Dispose immediately
			return result;
		}

		public void Dispose()
		{
			Indices.Dispose();
			Vertices.Dispose();
			Normals.Dispose();
			Colors.Dispose();
			UVs.Dispose();
		}

		public void Clear()
		{
			Indices.Clear();
			Vertices.Clear();
			Normals.Clear();
			Colors.Clear();
			UVs.Clear();
		}

		public int AddVertex(in Vector3 pos)
		{
			return AddVertex(pos, Vector3.up, Color.red, Vector4.zero);
		}

		public int AddVertex(in Vector3 pos, in Vector3 normal)
		{
			return AddVertex(pos, normal, Color.red, Vector4.zero);
		}

		public int AddVertex(in Vector3 pos, in Vector3 normal, in Color color)
		{
			return AddVertex(pos, normal, color, Vector4.zero);
		}

		public int AddVertex(in Vector3 pos, in Vector3 normal, in Color color, in Vector4 uv)
		{
			Vertices.Add(pos);
			Normals.Add(normal);
			Colors.Add(color);
			UVs.Add(uv);
			return Vertices.Length - 1;
		}

		public void AddTriangle(int a, int b, int c)
		{
			Indices.Add(a);
			Indices.Add(b);
			Indices.Add(c);
		}

		public void AddFace(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			var normal = math.normalize(math.cross(v1 - v0, v2 - v1));
			var i0 = AddVertex(v0, normal);
			var i1 = AddVertex(v1, normal);
			var i2 = AddVertex(v2, normal);
			AddTriangle(i0, i1, i2);
		}

		private static List<T> ToList<T>(NativeList<T> source) where T : unmanaged
		{
			var result = new List<T>(source.Length);
			for (var i = 0; i < source.Length; i++)
			{
				result.Add(source[i]);
			}

			return result;
		}

		public Mesh ToMesh()
		{
			var mesh = new Mesh();
			mesh.SetVertices(ToList(Vertices));
			mesh.SetNormals(ToList(Normals));
			mesh.SetColors(ToList(Colors));
			mesh.SetUVs(0, ToList(UVs));
			mesh.SetTriangles(ToList(Indices), 0);
			mesh.RecalculateBounds();
			return mesh;
		}
	}
}
