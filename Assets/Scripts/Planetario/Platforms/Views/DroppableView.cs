using System.Collections;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Geometry.Primitive;
using Planetario.MoveConstraints.Models;
using Planetario.MoveConstraints.Models.Constraints;
using Planetario.Platforms.Models;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Ray = UnityEngine.Ray;

namespace Planetario.Platforms.Views
{
	public class DroppableView : BaseComponentView<Droppable>, IBeginDragHandler,
		IDragHandler, IEndDragHandler
	{
		private States _currentState = States.Ready;

		private IDropArea _dropArea;
		private DraggableState _dropHoverState;
		private LinearConstraint _liftConstraint;
		private IConstraint _roamingPlane;

		private DraggableState _roamingState;
		private float _startScale;

		private DraggableState _startState;
		private float _startWorldScale;
		private float3 _touchLocalPos;

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (_currentState != States.Ready)
			{
				return;
			}

			var touchPosition = eventData.pointerCurrentRaycast.worldPosition;
			var myTransform = transform;
			_touchLocalPos = myTransform.worldToLocalMatrix.MultiplyPoint(touchPosition);
			_startState = new DraggableState
			{
				touchPosition = touchPosition,
				location = new Location(myTransform)
			};
			_startScale = myTransform.localScale.x;
			_startWorldScale = myTransform.lossyScale.x;
			_currentState = States.Lifting;
			_liftConstraint = new LinearConstraint(
				new Ray(myTransform.position, myTransform.TransformDirection(Data.liftDir)),
				0f, _startWorldScale * Data.liftDistance);
		}

		public void OnDrag(PointerEventData eventData)
		{
			var ray = CalculateRay(eventData);
			var myTransform = transform;
			Location newPos;
			switch (_currentState)
			{
				case States.Lifting:
					LinearConstraint.ConstraintResult result;
					(newPos, result) = _liftConstraint.ComputeConstraintWithLimit(_startState, ray);
					SetNewPos(_startScale);
					if (result == LinearConstraint.ConstraintResult.ExceedMax)
					{
						GoToRoaming();
					}

					break;
				case States.Roaming:
					newPos = _roamingPlane.ComputeConstraint(_roamingState, ray);
					SetNewPos(_startScale);
					var closestArea = FindClosestDropArea(ray);

					if (closestArea != null)
					{
						_currentState = States.FoundDropArea;
						SetDropArea(closestArea);
					}

					break;
				case States.FoundDropArea:
					newPos = _dropArea.ComputeHoverConstraint(_dropHoverState, ray);
					var localScaleOnDropArea = GetLocalScaleOnDropArea();
					newPos.position += math.rotate(newPos.rotation, Data.liftDir) *
					                   Data.liftDistance * 0.5f * _dropArea.AreaWorldScale();
					SetNewPos(localScaleOnDropArea);
					var newArea = FindClosestDropArea(ray);
					if (newArea != null && newArea != _dropArea)
					{
						_dropArea = newArea;
						SetDropArea(newArea);
					}
					else if (_dropArea.ExitArea(ray))
					{
						GoToRoaming();
					}

					break;
			}

			void SetNewPos(float localScale)
			{
				if (!math.isnan(newPos.position.x))
				{
					myTransform.position = newPos.position;
					myTransform.rotation = newPos.rotation;
					myTransform.localScale = Vector3.one * localScale;
				}
			}

			void GoToRoaming()
			{
				_currentState = States.Roaming;
				myTransform.localScale = Vector3.one * _startScale;
				_roamingPlane = ScreenSpaceConstraint.GetConstraint(_startState.location.position);
				_roamingState = new DraggableState
				{
					touchPosition = myTransform.localToWorldMatrix.MultiplyPoint(_touchLocalPos),
					location = new Location(myTransform)
				};
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			switch (_currentState)
			{
				case States.Lifting:
				case States.Roaming:
					GoToReturning();
					break;
				case States.FoundDropArea:
					var ray = CalculateRay(eventData);
					if (_dropArea.ComputeDropLocation(ray, out var dropLocationId, out var dropLocation))
					{
						_currentState = States.Dropping;
						StartCoroutine(MoveToLocationAndDrop(_dropArea.LogicEntity, dropLocationId,
							dropLocation,
							GetLocalScaleOnDropArea(), Data.returnDuration));
					}
					else
					{
						GoToReturning();
					}

					break;
			}

			void GoToReturning()
			{
				_currentState = States.Returning;
				StartCoroutine(MoveToLocation(_startState.location, _startScale, Data.returnDuration));
			}
		}

		private float GetLocalScaleOnDropArea()
		{
			var objectWorldScaleOnArea = _dropArea.AreaWorldScale() * _dropArea.ObjectScale();
			return objectWorldScaleOnArea / transform.parent.lossyScale.x;
		}

		private void SetDropArea(IDropArea dropArea)
		{
			var myTransform = transform;
			myTransform.localScale = Vector3.one * dropArea.ObjectScale();
			_dropArea = dropArea;
			_dropHoverState = new DraggableState
			{
				touchPosition = myTransform.localToWorldMatrix.MultiplyPoint(_touchLocalPos),
				location = new Location(myTransform)
			};
		}

		private IDropArea FindClosestDropArea(Ray ray)
		{
			IDropArea foundArea = null;
			var foundDistance = float.PositiveInfinity;
			var rayHits = Physics.RaycastAll(ray, float.MaxValue);
			foreach (var rayHit in rayHits)
			{
				var dropArea = rayHit.transform.GetComponentInParent<IDropArea>();
				if (dropArea != null && !IsMyChild(dropArea) && dropArea.EnterArea(ray, out var distance))
				{
					if (distance < foundDistance)
					{
						foundArea = dropArea;
						foundDistance = distance;
					}
				}
			}

			return foundArea;
		}

		private bool IsMyChild(IDropArea dropArea)
		{
			var current = dropArea.GetTransform();
			do
			{
				if (current == transform)
				{
					return true;
				}

				current = current.parent;
			} while (current != null);

			return false;
		}

		private static Ray CalculateRay(PointerEventData eventData)
		{
			var mainCamera = Camera.main;
			var camRay = mainCamera.ScreenPointToRay(eventData.position);
			var ray = new Ray(camRay.origin, camRay.direction);
			return ray;
		}

		private IEnumerator MoveToLocationAndDrop(IGameEntityInfo platform, int locationId, Location location,
			float localScale, float duration)
		{
			yield return StartCoroutine(MoveToLocation(location, localScale, duration));
			if (Data.cloneOnDrop)
			{
				//snap back
				var myTransform = transform;
				myTransform.position = _startState.location.position;
				myTransform.rotation = _startState.location.rotation;
				myTransform.localScale = Vector3.one * _startScale;
				_currentState = States.Ready;
				SendCommand(new InstantiateEntityCommand
				{
					source = LogicEntity,
					platform = platform,
					locationId = locationId
				});
			}
			else
			{
				SendCommand(new DropCommand
				{
					entity = LogicEntity,
					platform = platform,
					locationId = locationId
				});
			}
		}

		private IEnumerator MoveToLocation(Location location, float localScale, float duration)
		{
			var elapsed = 0f;
			var myTransform = transform;
			var startPos = myTransform.position;
			var startRot = myTransform.rotation;
			var startScale = myTransform.localScale.x;
			while (elapsed < duration)
			{
				var ratio = elapsed / duration;
				myTransform.position =
					math.lerp(startPos, location.position, ratio);
				myTransform.rotation = math.slerp(startRot, location.rotation, ratio);
				myTransform.localScale = Vector3.one * math.lerp(startScale, localScale, ratio);
				elapsed += Time.deltaTime;
				yield return null;
			}

			myTransform.position = location.position;
			myTransform.rotation = location.rotation;
			myTransform.localScale = Vector3.one * localScale;
			_currentState = States.Ready;
		}

		private enum States
		{
			Ready, //before any dragging occur
			Lifting, //lifting from original position
			Roaming, //looking for drop area
			FoundDropArea, //snapped to a drop area
			Dropping, //dropping to a spot in the drop area
			Returning //returning back to original position
		}
	}
}
