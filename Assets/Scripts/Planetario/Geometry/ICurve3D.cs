using Unity.Mathematics;

namespace Planetario.Geometry
{
	public interface ICurve3D : ICurve<float3>
	{
		ICurve3D OffsetCurve(float offset, float3 up);
	}
}
