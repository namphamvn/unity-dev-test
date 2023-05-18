using System;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Platforms.Models;
using UnityEngine;

namespace Planetario.Platforms.Views
{
	public class OccupantView : BaseComponentView<Occupant>
	{
		private Action _unsubscribe;

		private void OnDestroy()
		{
			_unsubscribe?.Invoke();
			_unsubscribe = null;
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				OnDataChanged(authoredData);
			}
		}

		public override void ResetData()
		{
			_unsubscribe = LogicEntity.SubscribeOnDataChangedExactType<Occupant>(OnDataChanged);
		}

		public override void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			base.BakeModel(myEntity, parent);
			parent.GetComponentDataWithExactType<OccupancyState>()
				.SetOccupant(myEntity, Data.location, parent);
		}

		private void OnDataChanged(Occupant occupant)
		{
			var myTransform = transform;
			if (myTransform.parent == null)
			{
				return; //could be in prefab mode, or not under a GameWorld
			}

			var platform = myTransform.parent.GetComponent<IPlatformView>();
			if (platform == null)
			{
				return; //this happens when using in StepAddComponents
			}

			var platformLocations = platform.GetPlatformLocations();
			var location = platformLocations.GetLocation(occupant.location);
			myTransform.localPosition = location.position;
			myTransform.localRotation = location.rotation;
			myTransform.localScale = Vector3.one * platformLocations.GetObjectScale();
		}
	}
}
