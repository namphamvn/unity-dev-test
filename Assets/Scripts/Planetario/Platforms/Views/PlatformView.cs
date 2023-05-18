using System.Collections.Generic;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Geometry.Meshes;
using Planetario.Geometry.Primitive;
using Planetario.MoveConstraints.Models;
using Planetario.Platforms.Models;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.Platforms.Views
{
	public abstract class PlatformView<T> : BaseComponentView<T>, IDropArea, IPlatformView
		where T : unmanaged, IComponentData, IPlatform
	{
		[SerializeField] private bool blockingDrop;

		public bool EnterArea(in Ray dragRay, out float distance)
		{
			if (blockingDrop)
			{
				distance = -1f;
				return false;
			}

			if (GetRayBlocker().Raycast(dragRay, out var enter))
			{
				distance = enter;
				return true;
			}

			distance = -1f;
			return false;
		}

		public bool ExitArea(in Ray dragRay)
		{
			return !EnterArea(in dragRay, out var _);
		}

		public Location ComputeHoverConstraint(in DraggableState state, Ray currentRay)
		{
			return ComputeWorldLocation(currentRay).location;
		}

		public bool ComputeDropLocation(in Ray dragRay,
			out Location dropLocation)
		{
			dropLocation = ComputeWorldLocation(dragRay).location;
			return true;
		}

		public bool ComputeDropLocation(in Ray dragRay, out int dropLocationId, out Location dropLocation)
		{
			(dropLocationId, dropLocation) = ComputeWorldLocation(dragRay);
			return !LogicEntity.GetComponentDataWithExactType<OccupancyState>().IsOccupied(dropLocationId);
		}

		public float ObjectScale()
		{
			return GetPlatformLocations().GetObjectScale();
		}

		public Transform GetTransform()
		{
			return transform;
		}

		public abstract IPlatformLocations GetPlatformLocations();
		protected abstract IRaycastable GetRayBlocker();

		private (int locationId, Location location) ComputeWorldLocation(Ray currentRay)
		{
			if (GetRayBlocker().Raycast(currentRay, out var enter))
			{
				var rayPos = currentRay.GetPoint(enter);
				(int locationId, Location location) found = (-1, default);
				var minDistanceSq = float.MaxValue;
				foreach (var item in GetWorldLocations())
				{
					var distanceSq = math.distancesq(item.location.position, rayPos);
					if (minDistanceSq > distanceSq)
					{
						minDistanceSq = distanceSq;
						found = item;
					}
				}

				return found;
			}

			return default;
		}

		public IEnumerable<(int locationId, Location location)> GetWorldLocations()
		{
			var aspect = GetPlatformLocations();

			for (var i = 0; i < aspect.GetLocationCount(); i++)
			{
				var localLocation = aspect.GetLocation(i);
				var myTransform = transform;
				yield return (i, new Location(myTransform.TransformPoint(localLocation.position),
					math.mul(myTransform.rotation, localLocation.rotation)));
			}
		}

		public override void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			base.BakeModel(myEntity, parent);
			myEntity.AddComponentData(new OccupancyState());
		}
	}
}
