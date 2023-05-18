using System;
using System.Collections.Generic;
using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds.Models
{
	[Serializable]
	public struct CombinedCommand : IUndoableCommand
	{
		public List<ICommand> commands;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			var result = new CombinedCommand
			{
				commands = new List<ICommand>()
			};
			foreach (var childCmd in commands)
			{
				if (childCmd is IUndoableCommand undoableCommand)
				{
					result.commands.Add(undoableCommand.GetUndoCommand(worldSystem));
				}
			}

			return result;
		}
	}
}
