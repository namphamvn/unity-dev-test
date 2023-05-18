using System;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Geometry.Primitive;
using Planetario.MoveConstraints.Models;
using Planetario.MoveConstraints.Models.Constraints;
using Planetario.Shares;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.MoveConstraints.Views
{
	/// <summary>
	///     Receive drag events and send drag commands
	/// </summary>
	public class DraggableView : BaseComponentView<IConstraintState>, IBeginDragHandler, IDragHandler,
		IEndDragHandler
	{
		private IConstraintState _startConstraintState;

		private DragState _startDragState;
		private Action _unsubscribe;

		private void OnDestroy()
		{
			_unsubscribe?.Invoke();
			_unsubscribe = null;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			var touchPosition = eventData.pointerCurrentRaycast.worldPosition;
			_startDragState = new DragState
			{
				startTouchPosition = GetParentInvMatrix().TransformPoint(touchPosition),
				currentRay = ComputeLocalRay(eventData)
			};
			_startConstraintState = LogicEntity.GetComponentData<IConstraintState>();
		}

		public void OnDrag(PointerEventData eventData)
		{
			var constraint = GetParentConstraint();
			var dragState = _startDragState;
			dragState.currentRay = ComputeLocalRay(eventData);
			var currentState = constraint.ComputeNewCoord(_startConstraintState, dragState);
			OnDataChanged(currentState);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			SendCommand(new DragCommand
			{
				entity = LogicEntity,
				dragState = _startDragState,
				endRay = ComputeLocalRay(eventData)
			});
		}

		public override void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			Initialize();

			var myTransform = transform;
			var constraintView = myTransform.parent.GetComponent<IConstraintView>();
			var constraint = constraintView.GetConstraintData();
			var relativeLocation = new Location(myTransform.localPosition, myTransform.localRotation);
			var method = constraint.GetType()
				.GetMethod(nameof(IConstraint<SphericalConstraint.State>.Initialize));
			var state = (IConstraintState)method.Invoke(constraint, new object[] { relativeLocation });

			myEntity.AddComponentData(state);
			myEntity.AddComponentData(new LocalScale
			{
				value = myTransform.localScale.x
			});
		}

		private void OnDataChanged(IConstraintState constraintState)
		{
			var constraint = GetParentConstraint();
			var newLocation = constraint.ApplyCoord(constraintState);
			var myTransform = transform;
			myTransform.localPosition = newLocation.position;
			myTransform.localRotation = newLocation.rotation;
			myTransform.localScale =
				LogicEntity.GetComponentDataWithExactType<LocalScale>().value * Vector3.one;
		}

		private IConstraint GetParentConstraint()
		{
			var parent = LogicEntity.Parent;
			var constraint = parent.GetComponentData<IConstraint>();
			return constraint;
		}

		public override void ResetData()
		{
			_unsubscribe = LogicEntity.SubscribeOnDataChanged<IConstraintState>(OnDataChanged);
		}

		private Ray ComputeLocalRay(PointerEventData eventData)
		{
			var inversed = GetParentInvMatrix();
			var camera = Camera.main;
			var ray = camera.ScreenPointToRay(eventData.position);
			var localRay = new Ray(inversed.TransformPoint(ray.origin),
				inversed.TransformDirection(ray.direction));
			return localRay;
		}

		private float4x4 GetParentInvMatrix()
		{
			return transform.parent.worldToLocalMatrix;
		}
	}
}
