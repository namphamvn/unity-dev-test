using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;

namespace Planetario.Interactions.Models
{
	[Serializable]
	public struct ToggleSelectionCommand : IUndoableCommand
	{
		public IGameEntityInfo entity;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			return this;
		}
	}
}
