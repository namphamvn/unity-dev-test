using System;
using Planetario.Geometry.Extensions;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models.Constraints
{
	/// <summary>
	///     Constraint the movement on a spherical surface
	///     The distance between the object and this transform origin is constant
	/// </summary>
	[Serializable]
	public struct SphericalConstraint : IConstraint<SphericalConstraint.State>
	{
		public float3 center;

		public SphericalConstraint(float3 center)
		{
			this.center = center;
		}

		public readonly Location ComputeConstraint(in DraggableState state, Ray currentRay)
		{
			var touchDir = state.touchPosition - center;
			var sphere = new Sphere(center, math.length(touchDir));
			var rayPos = sphere.Raycast(currentRay, out var enter)
				? currentRay.GetPoint(enter)
				: currentRay.ClosestPointTo(center);

			var rayDir = rayPos - center;
			var rotation = touchDir.RotateTowards(rayDir);
			var startDir = state.location.position - center;
			return new Location(center + math.rotate(rotation, startDir),
				math.mul(rotation, state.location.rotation));
		}

		public readonly State Initialize(in Location relativeLocation)
		{
			return new State
			{
				startLocation = new Location(relativeLocation.position, quaternion.identity),
				coord = relativeLocation.rotation
			};
		}

		public IConstraintState ComputeNewCoord(IConstraintState dragStartState, in DragState dragState)
		{
			return ComputeCoord((State)dragStartState, dragState);
		}

		public Location ApplyCoord(IConstraintState constraintState)
		{
			return ApplyCoord((State)constraintState);
		}

		public readonly State ComputeCoord(in State constraintState, in DragState dragState)
		{
			var touchDir = dragState.startTouchPosition - center;
			var sphere = new Sphere(center, math.length(touchDir));
			var rayPos = sphere.Raycast(dragState.currentRay, out var enter)
				? dragState.currentRay.GetPoint(enter)
				: dragState.currentRay.ClosestPointTo(center);

			var rayDir = rayPos - center;
			var rotation = touchDir.RotateTowards(rayDir);
			if (math.isnan(rotation.value.x))
			{
				//happen when touchDir and rayDir are identical
				rotation = quaternion.identity;
			}

			var result = constraintState;
			result.coord = math.mul(rotation, constraintState.coord);
			return result;
		}

		public readonly Location ApplyCoord(in State constraintState)
		{
			var startDir = constraintState.startLocation.position - center;
			return new Location(center + math.rotate(constraintState.coord, startDir),
				math.mul(constraintState.coord, constraintState.startLocation.rotation));
		}

		public struct State : IConstraintState
		{
			public Location startLocation;
			public quaternion coord;
		}
	}
}
