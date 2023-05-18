using System;
using Planetario.GameWorlds.Systems;
using Unity.Entities;

namespace Planetario.GameWorlds.Models
{
	/// <summary>
	/// a command to set a component data to an entity
	/// </summary>
	[Serializable]
	public struct SetComponentDataCommand : IUndoableCommand
	{
		public IComponentData componentData;
		public IGameEntityInfo entity;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			return new SetComponentDataCommand
			{
				entity = entity,
				componentData = entity.GetComponentData(componentData.GetType())
			};
		}
	}
}
