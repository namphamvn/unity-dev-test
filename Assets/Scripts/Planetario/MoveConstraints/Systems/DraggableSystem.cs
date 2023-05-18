using Planetario.GameWorlds;
using Planetario.GameWorlds.Systems;
using Planetario.MoveConstraints.Models;
using Planetario.MoveConstraints.Models.Constraints;

namespace Planetario.MoveConstraints.Systems
{
	public class DraggableSystem : BaseSystem, ISystem<DragCommand>
	{
		public void OnCommandReceived(DragCommand command)
		{
			var parent = command.entity.Parent;
			var constraint = parent.GetComponentData<IConstraint>();
			var dragState = command.dragState;
			dragState.currentRay = command.endRay;
			var constraintState = command.entity.GetComponentData<IConstraintState>();
			var newState = constraint.ComputeNewCoord(constraintState, dragState);
			GetEntity(command.entity).SetComponentData(newState);
		}
	}
}
