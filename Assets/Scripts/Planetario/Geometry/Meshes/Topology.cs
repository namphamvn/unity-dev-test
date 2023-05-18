using System;
using Unity.Collections;

namespace Planetario.Geometry.Meshes
{
	public struct Topology : IDisposable
	{
		/// <summary>
		///     one vertexId to multiple faceId
		/// </summary>
		public NativeMultiHashMap<int, int> vertexToFaces;

		/// <summary>
		///     one vertexId to its vertex neighbours
		/// </summary>
		public NativeMultiHashMap<int, int> vertexToVertices;

		/// <summary>
		///     one faceId to its face neighbours
		/// </summary>
		public NativeMultiHashMap<int, int> faceToFaces;

		public Topology(MeshData input, Allocator allocator)
		{
			vertexToFaces = new NativeMultiHashMap<int, int>(input.Vertices.Length, allocator);
			vertexToVertices = new NativeMultiHashMap<int, int>(input.Vertices.Length, allocator);
			faceToFaces = new NativeMultiHashMap<int, int>(input.Indices.Length / 3, allocator);
		}

		public void Dispose()
		{
			vertexToFaces.Dispose();
			vertexToVertices.Dispose();
			faceToFaces.Dispose();
		}
	}
}
