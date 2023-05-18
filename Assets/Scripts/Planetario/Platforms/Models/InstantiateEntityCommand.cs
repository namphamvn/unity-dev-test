using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct InstantiateEntityCommand : ICommand
	{
		public int locationId;
		public IGameEntityInfo platform;
		public IGameEntityInfo source;
	}
}
