using System.Collections.Generic;
using Unity.Mathematics;

namespace Planetario.Geometry
{
	public interface ICurve2D : ICurve<float2>
	{
		ICurve2D OffsetCurve(float offset);
		IEnumerable<float2> Intersect(ICurve2D other);
		IEnumerable<float> IntersectCoord(ICurve2D other);
	}
}
