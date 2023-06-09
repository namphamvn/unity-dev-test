﻿using Planetario.GameWorlds.Models;

namespace Planetario.GameWorlds.Systems
{
	/// <summary>
	/// base class for all systems
	/// </summary>
	public abstract class BaseSystem : ISystem
	{
		protected GameWorldSystem WorldSystem;

		public void SetGameWorld(GameWorldSystem worldSystem)
		{
			WorldSystem = worldSystem;
		}

		protected GameEntity GetEntity(IGameEntityInfo entityInfo)
		{
			// This should be reimplemented using an Entity ID system
			return (GameEntity)entityInfo;
		}
	}
}
