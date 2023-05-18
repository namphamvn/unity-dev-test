using System;
using System.Collections.Generic;
using Planetario.GameWorlds.Views;
using UnityEngine;

namespace Planetario.GameWorlds.Services
{
	public class GameEntityViewCatalogService : MonoBehaviour
	{
		public GameEntityView[] prefabs;

		private Dictionary<Guid, GameEntityView> _map;
		public static GameEntityViewCatalogService Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
			_map = new Dictionary<Guid, GameEntityView>();
			foreach (var prefab in prefabs)
			{
				_map.Add(new Guid(prefab.prefabGuid), prefab);
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public GameEntityView GetPrefab(Guid guid)
		{
			return _map[guid];
		}
	}
}
