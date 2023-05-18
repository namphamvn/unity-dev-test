using System;
using Unity.Collections;
using UnityEngine;

namespace Planetario.Geometry.Meshes
{
	public readonly struct Tessellator
	{
		private struct EdgePoint : IEquatable<EdgePoint>
		{
			public readonly int Index;
			public readonly int Vertex0;
			public readonly int Vertex1;

			public EdgePoint(int index, int v0, int v1)
			{
				Index = index;
				Vertex0 = v0;
				Vertex1 = v1;
			}

			public bool Equals(EdgePoint other)
			{
				return Index == other.Index && Vertex0 == other.Vertex0 && Vertex1 == other.Vertex1;
			}

			public override bool Equals(object obj)
			{
				return obj is EdgePoint other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = Index;
					hashCode = (hashCode * 397) ^ Vertex0;
					hashCode = (hashCode * 397) ^ Vertex1;
					return hashCode;
				}
			}
		}

		public static void Compute(MeshData result, MeshData input, int tessellateLevel)
		{
			var oldFaces = input.Indices;
			result.Indices.Clear();
			var newFaces = result.Indices;
			var capacity = input.Vertices.Length * tessellateLevel;
			using var edgePointCache = new NativeHashMap<EdgePoint, int>(capacity, Allocator.Temp);

			var midPointCacheSize = tessellateLevel * tessellateLevel;
			var middlePointCache = new NativeArray<int>(midPointCacheSize, Allocator.Temp);

			for (var i = 0; i < oldFaces.Length / 3; i++)
			{
				var v0 = oldFaces[i * 3];
				var v1 = oldFaces[i * 3 + 1];
				var v2 = oldFaces[i * 3 + 2];
				TessellateFace(tessellateLevel, v0, v1, v2);
			}

			middlePointCache.Dispose();

			void TessellateFace(int n, int v0, int v1, int v2)
			{
				var vertices = input.Vertices;

				Vector3 GridPos(int x, int y)
				{
					var baseX = (vertices[v1] - vertices[v0]) * x / n;
					var baseY = (vertices[v2] - vertices[v0]) * y / n;
					return vertices[v0] + baseX + baseY;
				}

				void ClearMidPointCache()
				{
					for (var i = 0; i < n * n; i++)
					{
						middlePointCache[i] = -1;
					}
				}

				//return a vertex, or create a new one, base on triangle coordinate x,y
				int GetVertex(int x, int y)
				{
					if (x == 0 && y == 0)
					{
						return v0;
					}

					if (x == n && y == 0)
					{
						return v1;
					}

					if (x == 0 && y == n)
					{
						return v2;
					}

					var onEdge = false;
					var middle = -1;
					var p0 = -1;
					var p1 = -1;
					if (x == 0)
					{
						onEdge = true;
						middle = y;
						p0 = v0;
						p1 = v2;
					}
					else if (y == 0)
					{
						onEdge = true;
						middle = x;
						p0 = v0;
						p1 = v1;
					}
					else if (x + y == n)
					{
						onEdge = true;
						middle = x;
						p0 = v2;
						p1 = v1;
					}

					if (onEdge)
					{
						var key1 = new EdgePoint(middle, p0, p1);
						var key2 = new EdgePoint(n - middle, p1, p0);
						var key = p0 < p1 ? key1 : key2;

						int ret;
						if (edgePointCache.TryGetValue(key, out ret))
						{
							return ret;
						}

						ret = result.AddVertex(GridPos(x, y));
						edgePointCache.Add(key, ret);
						return ret;
					}
					else
					{
						int ret;
						var key = n * x + y;
						var cachedVal = middlePointCache[key];
						if (cachedVal == -1)
						{
							ret = result.AddVertex(GridPos(x, y));
							middlePointCache[key] = ret;
							return ret;
						}

						return cachedVal;
					}
				}

				void AddTriangle(int a, int b, int c)
				{
					newFaces.Add(a);
					newFaces.Add(b);
					newFaces.Add(c);
				}

				ClearMidPointCache();
				for (var x = 0; x <= n; x++)
				{
					for (var y = 0; y <= n; y++)
					{
						//triangle that is the lower half of the grid cell
						if (x + y + 1 <= n)
						{
							AddTriangle(GetVertex(x, y), GetVertex(x + 1, y), GetVertex(x, y + 1));
						}

						//triangle that is the upper half of the grid cell
						if (x + y + 2 <= n)
						{
							AddTriangle(GetVertex(x + 1, y), GetVertex(x + 1, y + 1), GetVertex(x, y + 1));
						}
					}
				}
			}
		}
	}
}
