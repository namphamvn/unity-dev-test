using Planetario.GameWorlds.Views;
using UnityEngine;

namespace Planetario.GameWorlds.Authors
{
	/// <summary>
	/// kickstarts the "baking" process, converting prefabs and scenes into Model
	/// </summary>
	public class GameWorldAuthor : MonoBehaviour
	{
		public GameWorldView world;

		/// <summary>
		///     This must be executed before all other events in all Views
		///     This is achieved with Script Execution Order
		/// </summary>
		private void Awake()
		{
			world.BakeModel(null, null);
			Destroy(gameObject);
		}
	}
}
