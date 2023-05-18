using Planetario.Shares;

namespace Planetario.Geometry.Meshes
{
	public struct TopologyBuilder
	{
		public static void Compute(Topology output, MeshData input)
		{
			//compute all faces connected to each vertex
			var indices = input.Indices;
			var vertexToFaces = output.vertexToFaces;
			var vertexToVertices = output.vertexToVertices;
			for (var faceId = 0; faceId < indices.Length / 3; faceId++)
			{
				var v0 = indices[faceId * 3];
				var v1 = indices[faceId * 3 + 1];
				var v2 = indices[faceId * 3 + 2];

				vertexToFaces.AddUniqueValueToKey(v0, faceId);
				vertexToFaces.AddUniqueValueToKey(v1, faceId);
				vertexToFaces.AddUniqueValueToKey(v2, faceId);

				vertexToVertices.AddUniqueValueToKey(v0, v1);
				vertexToVertices.AddUniqueValueToKey(v0, v2);
				vertexToVertices.AddUniqueValueToKey(v1, v0);
				vertexToVertices.AddUniqueValueToKey(v1, v2);
				vertexToVertices.AddUniqueValueToKey(v2, v0);
				vertexToVertices.AddUniqueValueToKey(v2, v1);
			}

			//compute adjacent faces to each face
			var faceToFaces = output.faceToFaces;
			for (var i = 0; i < input.Vertices.Length; i++)
			{
				foreach (var firstFace in vertexToFaces.GetValuesForKey(i))
				{
					foreach (var secondFace in vertexToFaces.GetValuesForKey(i))
					{
						if (firstFace == secondFace)
						{
							continue;
						}

						var sameVertex = 0;
						for (var v1 = 0; v1 < 3; v1++)
						{
							var ver1 = indices[firstFace * 3 + v1];
							for (var v2 = 0; v2 < 3; v2++)
							{
								var ver2 = indices[secondFace * 3 + v2];
								if (ver1 == ver2)
								{
									sameVertex++;
								}
							}
						}

						if (sameVertex >= 2)
						{
							//these adjacency are unique, no need to check for duplicates
							faceToFaces.Add(firstFace, secondFace);
							faceToFaces.Add(secondFace, firstFace);
						}
					}
				}
			}
		}
	}
}
