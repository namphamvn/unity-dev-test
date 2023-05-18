using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;
using Planetario.Geometry.Primitive;
using Planetario.MoveConstraints.Models.Constraints;

namespace Planetario.MoveConstraints.Models
{
	[Serializable]
	public struct DragCommand : IUndoableCommand
	{
		public Ray endRay;
		public DragState dragState;
		public IGameEntityInfo entity;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			return new SetComponentDataCommand
			{
				entity = entity,
				componentData = entity.GetComponentData<IConstraintState>()
			};
		}
	}
}
