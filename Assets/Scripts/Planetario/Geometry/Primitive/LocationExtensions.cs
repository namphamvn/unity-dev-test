using UnityEngine;

namespace Planetario.Geometry.Primitive
{
	public static class LocationExtensions
	{
		public static Location GetRelativeLocation(Transform draggable, Transform join)
		{
			return new Location(
				join.InverseTransformPoint(draggable.position),
				Quaternion.Inverse(join.rotation) * draggable.rotation
			);
		}
	}
}
