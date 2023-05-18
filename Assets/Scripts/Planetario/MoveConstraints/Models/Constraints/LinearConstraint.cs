using System;
using Planetario.Geometry.Primitive;
using Planetario.Shares;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models.Constraints
{
	/// <summary>
	///     Constraint movement on an infinite line with optional limit
	///     The limit represent the limit point along the ray
	/// </summary>
	[Serializable]
	public struct LinearConstraint : IConstraint<LinearConstraint.State>
	{
		public enum ConstraintResult
		{
			WithinConstraint,
			ExceedMin,
			ExceedMax
		}

		public Ray constraintRay;
		public SerializableNullable<float> minLimit;
		public SerializableNullable<float> maxLimit;

		public LinearConstraint(in Ray constraintRay, in SerializableNullable<float> minLimit,
			in SerializableNullable<float> maxLimit)
		{
			this.constraintRay = constraintRay;
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
		}

		public Location ComputeConstraint(in DraggableState state, Ray currentRay)
		{
			var (newPos, _) = ComputeConstraintWithLimit(state, currentRay);
			return newPos;
		}

		public Location ApplyCoord(IConstraintState constraintState)
		{
			return ApplyCoord((State)constraintState);
		}

		public IConstraintState ComputeNewCoord(IConstraintState dragStartState, in DragState dragState)
		{
			return ComputeCoord((State)dragStartState, dragState);
		}

		public State Initialize(in Location relativeLocation)
		{
			return new State
			{
				posAtZeroCoord = new Ray(relativeLocation.position, constraintRay.direction)
					.ClosestPointTo(constraintRay.origin),
				startRotation = relativeLocation.rotation,
				coord = constraintRay.ClosestCoordTo(relativeLocation.position)
			};
		}

		public State ComputeCoord(in State constraintState, in DragState dragState)
		{
			var dragRay = new Ray(dragState.startTouchPosition, constraintRay.direction);
			var delta = dragRay.ClosestCoordTo(dragState.currentRay);
			var result = constraintState;
			result.coord += delta;
			if (minLimit.HasValue)
			{
				result.coord = math.max(result.coord, minLimit.Value);
			}

			if (maxLimit.HasValue)
			{
				result.coord = math.min(result.coord, maxLimit.Value);
			}

			return result;
		}

		public Location ApplyCoord(in State constraintState)
		{
			return new Location(
				constraintState.posAtZeroCoord + constraintState.coord * constraintRay.direction,
				constraintState.startRotation);
		}

		public (Location newPos, ConstraintResult result) ComputeConstraintWithLimit(in DraggableState state,
			Ray currentRay)
		{
			var onAxis = new Ray(state.touchPosition, constraintRay.direction).ClosestPointTo(currentRay);

			var newPos = onAxis - state.touchPosition + state.location.position;
			var result = ConstraintResult.WithinConstraint;
			var constraintDir = constraintRay.direction;
			if (minLimit.HasValue || maxLimit.HasValue)
			{
				var constraintOrigin = constraintRay.origin;
				var objectRay = new Ray(state.location.position, constraintDir);

				bool CapPosToPlane(Plane plane)
				{
					if (!plane.IsOnPositiveSide(newPos))
					{
						plane.Raycast(objectRay, out var enter);
						newPos = objectRay.GetPoint(enter);
						return true;
					}

					return false;
				}

				if (minLimit.HasValue && CapPosToPlane(new Plane(constraintDir,
					    constraintOrigin + constraintDir * minLimit.Value)))
				{
					result = ConstraintResult.ExceedMin;
				}

				if (maxLimit.HasValue && CapPosToPlane(new Plane(-constraintDir,
					    constraintOrigin + constraintDir * maxLimit.Value)))
				{
					result = ConstraintResult.ExceedMax;
				}
			}

			return (new Location(newPos, state.location.rotation), result);
		}

		public struct State : IConstraintState
		{
			public quaternion startRotation;
			public float3 posAtZeroCoord;
			public float coord;
		}
	}
}
