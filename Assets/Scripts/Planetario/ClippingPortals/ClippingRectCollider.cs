using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.ClippingPortals
{
	public class ClippingRectCollider : MonoBehaviour, IColliderClipper
	{
		public ClippingRect clippingRect;

		public bool IsPointClipped(in Ray ray, in float3 position, in float distance)
		{
			return clippingRect.AmIClipped(ray, position, distance);
		}
	}
}
