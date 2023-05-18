using Planetario.Geometry.Primitive;
using Unity.Entities;

namespace Planetario.Geometry.Meshes
{
	public interface IPlatformLocations : IComponentData
	{
		Location GetLocation(int locationId);
		int GetLocationCount();
		float GetObjectScale();
	}
}
