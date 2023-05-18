using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;

namespace Planetario.Interactions.Models
{
	[Serializable]
	public struct AddEntityCommand : ICommand
	{
		public IGameEntityInfo entity;
		public IGameEntityInfo parent;
	}
}
