using Planetario.Shares.MaterialServices;
using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.ClippingPortals
{
	public class ClippingPlane : MaterialKeywordSetterForChildren
	{
		private static readonly int ClipPlaneParam = Shader.PropertyToID("_ClipVector0");
		public Transform clippingPlane;

		protected override MaterialKey GetMaterialKey()
		{
			return new MaterialKey(null, "_PL_CLIP_MODE_PLANE");
		}

		protected override void UpdatePropertyBlock(MaterialPropertyBlock block)
		{
			var plane = new Plane(clippingPlane.up, clippingPlane.position);
			block.SetVector(ClipPlaneParam,
				new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
		}

		public override bool IsChildClipped(in Ray ray, in float3 position, in float distance)
		{
			var dir = position - (float3)clippingPlane.transform.position;
			var coordY = math.dot(dir, clippingPlane.up);
			return coordY < 0f;
		}
	}
}
