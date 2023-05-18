using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.ClippingPortals
{
	public class ClippedCollider : MonoBehaviour, IColliderClipper
	{
		public MaterialKeywordSetterForChildren clipper;

		public bool IsPointClipped(in Ray ray, in float3 position, in float distance)
		{
			return clipper.IsChildClipped(ray, position, distance);
		}
	}
}
