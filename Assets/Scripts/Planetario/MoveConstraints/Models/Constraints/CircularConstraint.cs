using System;
using Planetario.Geometry.Extensions;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models.Constraints
{
	/// <summary>
	///     Constraint the movement to a circle
	///     The circle has the origin of this transform and a normal defined as a param
	/// </summary>
	[Serializable]
	public struct CircularConstraint : IConstraint<CircularConstraint.State>
	{
		public float3 normal;
		public float3 center;

		public CircularConstraint(float3 normal, float3 center)
		{
			this.normal = normal;
			this.center = center;
		}

		public Location ComputeConstraint(in DraggableState state, Ray currentRay)
		{
			var plane = new Plane(normal, state.touchPosition);
			if (plane.Raycast(currentRay, out var enter))
			{
				var hingeCenter = plane.ClosestPointTo(center);
				var touchDir = state.touchPosition - hingeCenter;

				var rayPos = currentRay.GetPoint(enter);
				var rayDir = rayPos - hingeCenter;

				var rotation = touchDir.RotateTowards(rayDir);
				if (math.isnan(rotation.value.x))
				{
					//happen when touchDir and rayDir are parallel
					return state.location;
				}

				var startDir = state.location.position - hingeCenter;
				var newPos = hingeCenter + math.rotate(rotation, startDir);
				return new Location(newPos,
					math.mul(rotation, state.location.rotation));
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
			var centerDir = relativeLocation.position - center;
			var hingeCenter = center + normal * math.dot(centerDir, normal);
			var radiusVector = relativeLocation.position - hingeCenter;
			var radius = math.length(radiusVector);
			var hingeToPos = radiusVector / radius;
			if (radius == 0f)
			{
				(_, hingeToPos) = normal.FromUpToForwardRight();
			}

			return new State
			{
				hingeCenter = hingeCenter,
				startRotation = relativeLocation.rotation,
				startRight = hingeToPos,
				radius = radius,
				angle = 0f
			};
		}

		public State ComputeCoord(in State constraintState, in DragState dragState)
		{
			var plane = new Plane(normal, dragState.startTouchPosition);
			if (plane.Raycast(dragState.currentRay, out var enter))
			{
				var hingeCenter = plane.ClosestPointTo(center);
				var touchDir = dragState.startTouchPosition - hingeCenter;

				var rayPos = dragState.currentRay.GetPoint(enter);
				var rayDir = rayPos - hingeCenter;

				var rotation = touchDir.RotateTowards(rayDir);
				if (math.isnan(rotation.value.x))
				{
					//happen when touchDir and rayDir are parallel
					return constraintState;
				}

				var startDirection = constraintState.ComputeDirection(this);
				var newDirection = math.rotate(rotation, startDirection);
				var result = constraintState;
				result.angle = constraintState.ComputeAngle(newDirection, this);
				return result;
			}

			return constraintState;
		}

		public Location ApplyCoord(in State constraintState)
		{
			return constraintState.ComputeLocation(this);
		}

		public struct State : IConstraintState
		{
			/// <summary>
			///     project start position onto the normal axis
			/// </summary>
			public float3 hingeCenter;

			public quaternion startRotation;
			public float angle;

			/// <summary>
			///     direction from hingeCenter to start position
			///     This vector should always be perpendicular with the normal
			/// </summary>
			public float3 startRight;

			/// <summary>
			///     distance from start position to hinge center
			/// </summary>
			public float radius;

			/// <summary>
			///     vector from hingeCenter to position
			/// </summary>
			public readonly float3 ComputeDirection(in CircularConstraint constraint)
			{
				var forward = math.cross(startRight, constraint.normal);
				return math.cos(angle) * startRight + math.sin(angle) * forward;
			}

			public readonly float ComputeAngle(in float3 direction, in CircularConstraint constraint)
			{
				var forward = math.cross(startRight, constraint.normal);
				var x = math.dot(direction, startRight);
				var y = math.dot(direction, forward);
				return math.atan2(y, x);
			}

			public readonly Location ComputeLocation(in CircularConstraint constraint)
			{
				var direction = ComputeDirection(constraint);
				return new Location(hingeCenter + direction * radius,
					math.mul(quaternion.AxisAngle(constraint.normal, -angle), startRotation));
			}
		}
	}
}
