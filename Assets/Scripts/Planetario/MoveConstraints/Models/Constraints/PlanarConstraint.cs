using System;
using Planetario.Geometry.Primitive;
using Planetario.Shares;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models.Constraints
{
	[Serializable]
	public struct PlanarConstraint : IConstraint<PlanarConstraint.State>
	{
		public CartesianPlane plane;
		public SerializableNullable<float2> minLimit;
		public SerializableNullable<float2> maxLimit;

		public Location ComputeConstraint(in DraggableState state, Ray currentRay)
		{
			if (plane.Raycast(currentRay, out var enter))
			{
				var intersectPoint = currentRay.GetPoint(enter);
				return new Location(intersectPoint - state.touchPosition + state.location.position,
					state.location.rotation);
			}

			return default;
		}

		public IConstraintState ComputeNewCoord(IConstraintState dragStartState, in DragState dragState)
		{
			return ComputeCoord((State)dragStartState, dragState);
		}

		public Location ApplyCoord(IConstraintState constraintState)
		{
			return ApplyCoord((State)constraintState);
		}

		public State Initialize(in Location relativeLocation)
		{
			var pos = plane.GetCoord3d(relativeLocation.position);
			return new State
			{
				startRotation = relativeLocation.rotation,
				y = pos.y,
				coordXZ = pos.xz
			};
		}

		public State ComputeCoord(in State constraintState, in DragState dragState)
		{
			var planeAtTouchPos = new Plane(plane.GetUp(), dragState.startTouchPosition);
			if (planeAtTouchPos.Raycast(dragState.currentRay, out var enter))
			{
				var intersectPoint = dragState.currentRay.GetPoint(enter);
				var delta = plane.GetCoord(intersectPoint) - plane.GetCoord(dragState.startTouchPosition);

				var result = constraintState;
				result.coordXZ += delta;
				if (minLimit.HasValue)
				{
					result.coordXZ = math.max(result.coordXZ, minLimit.Value);
				}
				if (maxLimit.HasValue)
				{
					result.coordXZ = math.min(result.coordXZ, maxLimit.Value);
				}
				return result;
			}

			return constraintState;
		}

		public Location ApplyCoord(in State constraintState)
		{
			return new Location(plane.GetPoint3d(constraintState.coordXZ, constraintState.y),
				constraintState.startRotation);
		}

		public struct State : IConstraintState
		{
			public quaternion startRotation;
			public float y;
			public float2 coordXZ;
		}
	}
}
