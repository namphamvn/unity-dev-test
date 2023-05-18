using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.ClippingPortals
{
	public interface IColliderClipper
	{
		bool IsPointClipped(in Ray ray, in float3 position, in float distance);
	}
}
