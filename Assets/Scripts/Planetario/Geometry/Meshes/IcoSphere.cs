using System;
using Planetario.Geometry.Primitive;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry.Meshes
{
	/// <summary>
	///     IcoSphere is a sphere that is either formed from triangles or hexagons / pentagons
	///     It starts from IcoSphereZeroOrder and use tessellation to have higher resolution
	///     An optional hexagonization will group triangles to form pentagons and hexagons
	/// </summary>
	[Serializable]
	public struct IcoSphere : IComponentData, ISphericalMeshPlatform
	{
		private static readonly float InnerRadiusConst = (1.0f + math.sqrt(5.0f)) / 4f;
		private static readonly float OuterRadiusConst = math.sqrt(10f + 2f * math.sqrt(5f)) / 4f;
		public float edgeLength;
		public int tessellationLevel;
		public bool hexagonize;


		public void ComputeMeshPlatform(ref MeshData output, ref LocationNetwork locationNetwork)
		{
			IcoSphereZeroOrder(output);

			if (tessellationLevel > 1)
			{
				//tesselation
				using var meshInput = new MeshData(output, Allocator.Temp, true);
				Tessellator.Compute(output, meshInput, tessellationLevel);

				output.NormaliseVertices(ComputeOuterRadius());
			}

			if (hexagonize && tessellationLevel >= 3)
			{
				Hexagonize(ref output, ref locationNetwork);
			}
			else
			{
				//calculate normal for vertices
				for (var i = 0; i < output.Normals.Length; i++)
				{
					output.Normals[i] = math.normalize(output.Vertices[i]);
				}

				//populate the locations
				for (var i = 0; i < output.Indices.Length / 3; i++)
				{
					var v0 = output.Vertices[output.Indices[i * 3]];
					var v1 = output.Vertices[output.Indices[i * 3 + 1]];
					var v2 = output.Vertices[output.Indices[i * 3 + 2]];
					var center = (v0 + v1 + v2) / 3f;
					var normal = math.normalize(center);
					var forward = math.normalize(math.cross(normal, v1 - v0));
					locationNetwork.locations.Add(new Location(center,
						quaternion.LookRotation(forward, normal)));
					locationNetwork.objScale = new LineSegment(v0, v1).DistanceTo(center) * 2;
				}
			}
		}

		public float PlatformRadius => ComputeOuterRadius();

		public void ComputePlatform(ref LocationNetwork locationNetwork)
		{
			var meshData = new MeshData(Allocator.Temp);
			ComputeMeshPlatform(ref meshData, ref locationNetwork);
			meshData.Dispose();
		}

		/// <summary>
		///     radius of sphere touches middle of each icoSphere edge
		/// </summary>
		public float ComputeInnerRadius()
		{
			return edgeLength * InnerRadiusConst;
		}

		/// <summary>
		///     radius of the icoSphere, from center to each vertices
		/// </summary>
		public float ComputeOuterRadius()
		{
			return edgeLength * OuterRadiusConst;
		}

		private void IcoSphereZeroOrder(MeshData output)
		{
			var halfEdge = edgeLength / 2f;
			var innerRadius = ComputeInnerRadius();
			output.AddVertex(new Vector3(-halfEdge, innerRadius, 0));
			output.AddVertex(new Vector3(halfEdge, innerRadius, 0));
			output.AddVertex(new Vector3(-halfEdge, -innerRadius, 0));
			output.AddVertex(new Vector3(halfEdge, -innerRadius, 0));

			output.AddVertex(new Vector3(0, -halfEdge, innerRadius));
			output.AddVertex(new Vector3(0, halfEdge, innerRadius));
			output.AddVertex(new Vector3(0, -halfEdge, -innerRadius));
			output.AddVertex(new Vector3(0, halfEdge, -innerRadius));

			output.AddVertex(new Vector3(innerRadius, 0, -halfEdge));
			output.AddVertex(new Vector3(innerRadius, 0, halfEdge));
			output.AddVertex(new Vector3(-innerRadius, 0, -halfEdge));
			output.AddVertex(new Vector3(-innerRadius, 0, halfEdge));

			// 5 faces around point 0
			output.AddTriangle(0, 11, 5);
			output.AddTriangle(0, 5, 1);
			output.AddTriangle(0, 1, 7);
			output.AddTriangle(0, 7, 10);
			output.AddTriangle(0, 10, 11);

			// 5 adjacent faces
			output.AddTriangle(1, 5, 9);
			output.AddTriangle(5, 11, 4);
			output.AddTriangle(11, 10, 2);
			output.AddTriangle(10, 7, 6);
			output.AddTriangle(7, 1, 8);

			// 5 faces around point 3
			output.AddTriangle(3, 9, 4);
			output.AddTriangle(3, 4, 2);
			output.AddTriangle(3, 2, 6);
			output.AddTriangle(3, 6, 8);
			output.AddTriangle(3, 8, 9);

			// 5 adjacent faces
			output.AddTriangle(4, 9, 5);
			output.AddTriangle(2, 4, 11);
			output.AddTriangle(6, 2, 10);
			output.AddTriangle(8, 6, 7);
			output.AddTriangle(9, 8, 1);
		}

		private static void Hexagonize(ref MeshData result, ref LocationNetwork locationNetwork)
		{
			//hexagonize
			var clone = new MeshData(result, Allocator.Temp, true);
			result.Clear();
			var topology = new Topology(clone, Allocator.Temp);
			TopologyBuilder.Compute(topology, clone);

			using var ringVertices = new NativeHashSet<int>(clone.Vertices.Length, Allocator.Temp);
			using var visitedFaces = new NativeHashSet<int>(clone.Indices.Length / 3, Allocator.Temp);
			using var vertexToCenter = new NativeHashMap<int, int>(clone.Vertices.Length, Allocator.Temp);

			for (var vid = 0; vid < clone.Vertices.Length; vid++)
			{
				var vertexDegree = topology.vertexToFaces.CountValuesForKey(vid);
				if (vertexDegree == 5 && !ringVertices.Contains(vid))
				{
					var valuesForKey = topology.vertexToVertices.GetValuesForKey(vid);
					valuesForKey.MoveNext();
					var firstRingVertex = valuesForKey.Current;
					locationNetwork.objScale =
						math.length(clone.Vertices[vid] - clone.Vertices[firstRingVertex]) * 2f;
					SearchForCenter(ref result, ref locationNetwork, vid);
				}
			}

			//find adjacency
			for (var currentCenter = 0; currentCenter < clone.Vertices.Length; currentCenter++)
			{
				if (ringVertices.Contains(currentCenter))
				{
					continue;
				}

				var locationA = vertexToCenter[currentCenter];
				foreach (var adjFace in topology.vertexToFaces.GetValuesForKey(currentCenter))
				{
					foreach (var neighborFace in topology.faceToFaces.GetValuesForKey(adjFace))
					{
						for (var i = 0; i < 3; i++)
						{
							var vid = clone.Indices[neighborFace * 3 + i];
							if (!ringVertices.Contains(vid))
							{
								var locationB = vertexToCenter[vid];
								if (locationA == locationB)
								{
									continue;
								}

								locationNetwork.adjacency.Add(locationA, locationB);
								break; //move to next neighbor
							}
						}
					}
				}
			}

			clone.Dispose();
			topology.Dispose();

			void SearchForCenter(ref MeshData result, ref LocationNetwork locationNetwork, int currentCenter)
			{
				if (vertexToCenter.ContainsKey(currentCenter))
				{
					return;
				}

				var center = float3.zero;
				var count = 0;
				var lastRingVertex = float3.zero;
				//mark ring vertices
				foreach (var neighbor in topology.vertexToVertices.GetValuesForKey(currentCenter))
				{
					ringVertices.Add(neighbor);
					count++;
					var ringVertex = (float3)clone.Vertices[neighbor];
					center += ringVertex;
					lastRingVertex = ringVertex;
				}

				//mark visited faces
				foreach (var face in topology.vertexToFaces.GetValuesForKey(currentCenter))
				{
					visitedFaces.Add(face);
				}

				//flatten center
				var flatCenter = center / count;
				clone.Vertices[currentCenter] = flatCenter;
				foreach (var face in topology.vertexToFaces.GetValuesForKey(currentCenter))
				{
					var v0 = clone.Vertices[clone.Indices[face * 3 + 0]];
					var v1 = clone.Vertices[clone.Indices[face * 3 + 1]];
					var v2 = clone.Vertices[clone.Indices[face * 3 + 2]];
					result.AddFace(v0, v1, v2);
				}

				var normal = math.normalize(flatCenter);
				var forward = math.normalize(lastRingVertex - flatCenter);
				var locationId = locationNetwork.locations.Length;
				vertexToCenter.Add(currentCenter, locationId);
				locationNetwork.locations.Add(new Location(flatCenter,
					quaternion.LookRotation(forward,
						normal)));

				//find next center
				foreach (var adjFace in topology.vertexToFaces.GetValuesForKey(currentCenter))
				{
					foreach (var neighborFace in topology.faceToFaces.GetValuesForKey(adjFace))
					{
						if (visitedFaces.Contains(neighborFace))
						{
							continue;
						}

						for (var i = 0; i < 3; i++)
						{
							var vid = clone.Indices[neighborFace * 3 + i];
							if (!ringVertices.Contains(vid))
							{
								SearchForCenter(ref result, ref locationNetwork, vid);
							}
						}
					}
				}
			}
		}
	}
}
