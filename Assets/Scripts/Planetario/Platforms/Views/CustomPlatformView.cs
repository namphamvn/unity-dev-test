using Planetario.Geometry.Meshes;
using Planetario.Geometry.Primitive;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.Platforms.Views
{
	public class CustomPlatformView : PlatformView<CustomPlatform>, IRaycastable, IPlatformLocations
	{
		[SerializeField] private Collider _collider;

		[SerializeField] private Transform[] _locations;

		[SerializeField] private float _objectScale = 1f;

		public Location GetLocation(int locationId)
		{
			if (locationId >= _locations.Length)
			{
				locationId = _locations.Length - 1;
			}

			var location = _locations[locationId];
			return new Location
			{
				position = location.localPosition,
				rotation = location.localRotation
			};
		}

		public int GetLocationCount()
		{
			return _locations.Length;
		}

		public float GetObjectScale()
		{
			return _objectScale;
		}

		public bool Raycast(Ray ray, out float enter)
		{
			var unityRay = new UnityEngine.Ray(ray.origin, ray.direction);
			if (_collider.Raycast(unityRay, out var rayHit, float.MaxValue))
			{
				enter = rayHit.distance;
				return true;
			}

			enter = default;
			return false;
		}

		protected override IRaycastable GetRayBlocker()
		{
			return this;
		}

		public override IPlatformLocations GetPlatformLocations()
		{
			return this;
		}
	}
}
