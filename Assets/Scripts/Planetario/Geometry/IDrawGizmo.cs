using System.Collections.Generic;
using Unity.Mathematics;

namespace Planetario.Geometry
{
	public interface IDrawGizmo
	{
		IEnumerable<(float3, float3)> GetGizmoLines();
	}
}
