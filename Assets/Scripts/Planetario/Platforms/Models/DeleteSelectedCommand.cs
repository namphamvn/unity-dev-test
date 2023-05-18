using System;
using System.Collections.Generic;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;
using Planetario.Interactions.Models;
using Planetario.Platforms.Systems;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct DeleteSelectedCommand : IUndoableCommand
	{
		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			var entities = worldSystem.GetSystem<AdditionDeletionSystem>().GetAllDeletableSelectedEntities();
			var result = new CombinedCommand
			{
				commands = new List<ICommand>()
			};
			foreach (var entity in entities)
			{
				result.commands.Add(new AddEntityCommand
				{
					entity = entity,
					parent = entity.Parent
				});
			}

			return result;
		}
	}
}
