using System;
using Planetario.Geometry.Meshes;
using Planetario.Geometry.Primitive;
using Unity.Collections;
using UnityEngine;

namespace Planetario.Platforms.Views
{
	public class IcoSphereView : MeshPlatformView<IcoSphere>
	{
		[NonSerialized] private bool _cached;
		private LocationNetwork _locationNetwork;

		private void OnDestroy()
		{
			if (_cached)
			{
				_locationNetwork.Dispose();
				_cached = false;
			}
		}

		protected override void SetCollider()
		{
			var collider = gameObject.GetComponent<SphereCollider>();
			if (collider == null)
			{
				collider = gameObject.AddComponent<SphereCollider>();
			}

			collider.radius = Data.PlatformRadius;
		}

		protected override IRaycastable GetRayBlocker()
		{
			var radius = Data.PlatformRadius;
			return new Sphere(transform.position, radius);
		}

		public override IPlatformLocations GetPlatformLocations()
		{
			if (!Application.isPlaying)
			{
				_locationNetwork = new LocationNetwork(5, Allocator.Temp);
				Data.ComputePlatform(ref _locationNetwork);
			}
			else if (!_cached)
			{
				_locationNetwork = new LocationNetwork(5, Allocator.Persistent);
				Data.ComputePlatform(ref _locationNetwork);
				_cached = true;
			}

			return _locationNetwork;
		}
	}
}
