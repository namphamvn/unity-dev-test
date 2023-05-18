using Planetario.GameWorlds.Systems;
using Planetario.GameWorlds.Views;
using UnityEngine;

namespace Planetario.GameWorlds.Authors
{
	/// <summary>
	/// base class for a system author
	/// which will add a system to the game world
	/// </summary>
	[RequireComponent(typeof(GameWorldView))]
	public abstract class SystemAuthor<TSystem> : SystemAuthor where TSystem : BaseSystem, new()
	{
		public override void RegisterSystem(GameWorldSystem worldSystem)
		{
			worldSystem.AddSystem(new TSystem());
		}
	}

	public abstract class SystemAuthor : MonoBehaviour
	{
		public abstract void RegisterSystem(GameWorldSystem worldSystem);
	}
}
