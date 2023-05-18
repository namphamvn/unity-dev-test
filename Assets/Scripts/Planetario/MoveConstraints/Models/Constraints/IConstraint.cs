using Planetario.Geometry.Primitive;
using Unity.Entities;

namespace Planetario.MoveConstraints.Models.Constraints
{
	public interface IConstraint : IComponentData
	{
		Location ComputeConstraint(in DraggableState state, Ray currentRay);
		IConstraintState ComputeNewCoord(IConstraintState dragStartState, in DragState dragState);
		Location ApplyCoord(IConstraintState constraintState);
	}

	public interface IConstraint<TState> : IConstraint
		where TState : unmanaged, IConstraintState
	{
		TState Initialize(in Location relativeLocation);

		/// <summary>
		///     This method is not used, but is here so that it is easier
		///     to job-ify later on
		/// </summary>
		TState ComputeCoord(in TState constraintState, in DragState dragState);

		Location ApplyCoord(in TState constraintState);
	}
}
