using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;

namespace Planetario.Interactions.Models
{
	[Serializable]
	public struct DeleteCommand : IUndoableCommand
	{
		public IGameEntityInfo entity;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			return new AddEntityCommand
			{
				entity = entity,
				parent = entity.Parent
			};
		}
	}
}
