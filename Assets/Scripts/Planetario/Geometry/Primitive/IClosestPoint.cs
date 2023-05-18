using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public interface IClosestPoint
	{
		float3 ClosestPointTo(float3 point);
	}
}
