using System.Collections.Generic;
using Unity.Mathematics;

namespace Planetario.Geometry
{
	public interface IPoints
	{
		IEnumerable<float3> GetPoints();
	}
}
