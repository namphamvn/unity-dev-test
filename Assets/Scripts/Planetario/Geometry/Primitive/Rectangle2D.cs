using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct Rectangle2D : IComponentData, IPoints2D
	{
		/// <summary>
		///     one corner of the rectangle
		/// </summary>
		public float2 origin;

		/// <summary>
		///     vector represents the width of the rectangle
		/// </summary>
		public float2 width;

		/// <summary>
		///     the height, on the perpendicular direction with the width vector
		/// </summary>
		public float height;

		public IEnumerable<float2> GetPoints()
		{
			var heightVector = math.normalize(width.yx) * height;

			yield return origin;
			yield return origin + width;
			yield return origin + width + heightVector;
			yield return origin + heightVector;
		}

		public float2 GetPoint(float2 coord)
		{
			var heightVector = math.normalize(width.yx) * height;
			return origin + width * coord.x + heightVector * coord.y;
		}
	}
}
