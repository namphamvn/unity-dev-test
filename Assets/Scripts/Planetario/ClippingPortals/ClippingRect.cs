using Planetario.Geometry.Primitive;
using Planetario.Shares.MaterialServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.ClippingPortals
{
	[RequireComponent(typeof(Renderer))]
	public class ClippingRect : MaterialKeywordSetterForChildren
	{
		private static readonly int PlaneOriginParam = Shader.PropertyToID("_ClipVector0");
		private static readonly int PlaneRightParam = Shader.PropertyToID("_ClipVector1");
		private static readonly int PlaneForwardParam = Shader.PropertyToID("_ClipVector2");
		private static readonly int RectSizeParam = Shader.PropertyToID("_ClipVector3");
		public Transform clippingPlane;
		public Vector2 rectExtends;
		public Collider[] rectColliders;

		private Renderer _renderer;

		protected override void Awake()
		{
			base.Awake();
			_renderer = GetComponent<Renderer>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			AssignRenderQueue(_renderer, (int)RenderQueue.Geometry - 1);
			foreach (var rectCollider in rectColliders)
			{
				var clipper = rectCollider.GetComponent<ClippingRectCollider>();
				if (clipper == null)
				{
					clipper = rectCollider.gameObject.AddComponent<ClippingRectCollider>();
				}

				clipper.clippingRect = this;
			}
		}

		protected override MaterialKey GetMaterialKey()
		{
			return new MaterialKey(null, "_PL_CLIP_MODE_RECT", (int)RenderQueue.Geometry - 2);
		}

		protected override void UpdatePropertyBlock(MaterialPropertyBlock block)
		{
			block.SetVector(PlaneOriginParam, clippingPlane.position);
			block.SetVector(PlaneRightParam, clippingPlane.right);
			block.SetVector(PlaneForwardParam, clippingPlane.forward);
			block.SetVector(RectSizeParam, rectExtends);
		}

		public override bool IsChildClipped(in Ray ray, in float3 position, in float distance)
		{
			var cartesianPlane = new CartesianPlane(clippingPlane);
			var coord = cartesianPlane.GetCoord3d(position);
			var up = clippingPlane.up;
			if (coord.y > 0f)
			{
				//positive side, clip things outside the extend area
				return math.abs(coord.x) > rectExtends.x ||
				       math.abs(coord.z) > rectExtends.y;
			}

			if (math.dot(ray.direction, up) > 0f)
			{
				//portal not visible
				return true;
			}

			//negative side, clip things outside of the rect, from camera perspective
			if (cartesianPlane.Raycast(ray, out var enter))
			{
				var intersectPoint = ray.GetPoint(enter);
				var intersectCoord = cartesianPlane.GetCoord(intersectPoint);
				return math.abs(intersectCoord.x) > rectExtends.x ||
				       math.abs(intersectCoord.y) > rectExtends.y;
			}

			//the plane is perpendicular with the camera, it can't be seen
			//this rayHit is on the negative side, so it is clipped
			return true;
		}

		public bool AmIClipped(in Ray ray, in float3 position, in float distance)
		{
			var cartesianPlane = new CartesianPlane(clippingPlane);
			var up = clippingPlane.up;
			if (math.dot(ray.direction, up) < 0f)
			{
				//the portal is visible
				if (cartesianPlane.Raycast(ray, out var enter))
				{
					var intersectPoint = ray.GetPoint(enter);
					var intersectCoord = cartesianPlane.GetCoord(intersectPoint);
					//clip if inside the portal
					//so that the ray will go to the children
					return math.abs(intersectCoord.x) <= rectExtends.x &&
					       math.abs(intersectCoord.y) <= rectExtends.y;
				}

				//the plane is perpendicular with the camera, it can't be seen
				//which mean no ray can get to the children, so no clipping
				return false;
			}

			//the portal is not visible, no clipping
			//which means it will block ray to all children
			return false;
		}
	}
}
