using System.Collections.Generic;
using Planetario.GameWorlds.Authors;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Services;
using Planetario.GameWorlds.Systems;
using Planetario.Shares.MaterialServices;
using UnityEngine;

namespace Planetario.GameWorlds.Views
{
	/// <summary>
	///     Every child GameObject is part of the game world
	///     which is serializable
	///     Convert pre-made Views child objects into Models
	///     Create and run game simulation
	/// </summary>
	public class GameWorldView : MonoBehaviour, IAuthor
	{
		[SerializeField] private GameEntityViewCatalogService _catalogService;
		[SerializeField] private MaterialService _materialService;
		private Dictionary<IGameEntityInfo, GameEntityView> _dataToView;
		public GameWorld World { get; private set; }
		public GameWorldSystem WorldSystem { get; private set; }
		public MaterialService MaterialService => _materialService;

		public void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			World = new GameWorld();
			Initialize();
			this.BakeChildren(null);
		}

		private void Initialize()
		{
			RegisterSystems();
			_dataToView = new Dictionary<IGameEntityInfo, GameEntityView>();
			World.OnEntityAdded += OnEntityAdded;
		}

		private void OnEntityAdded(IGameEntityInfo newEntity)
		{
			var parentView = GetView(newEntity.Parent);
			LoadEntity(newEntity, parentView.transform);
		}

		private void RegisterSystems()
		{
			WorldSystem = new GameWorldSystem(World);
			foreach (var system in GetComponents<SystemAuthor>())
			{
				system.RegisterSystem(WorldSystem);
			}
		}

		public void LoadData(GameWorld data)
		{
			World.Destroy();
			World = data;
			Initialize();

			foreach (var entity in World.Children)
			{
				LoadEntity(entity, transform);
			}
		}

		private void LoadEntity(IGameEntityInfo entity, Transform parent)
		{
			var prefab = _catalogService.GetPrefab(entity.PrefabGuid);
			var entityView = Instantiate(prefab, parent);
			entityView.Initialize();
			entityView.SetEntity(entity);
			foreach (var compView in entityView.GetComponents<BaseComponentView>())
			{
				compView.Initialize();
				compView.ResetData();
			}

			foreach (var child in entity.ChildrenInfo)
			{
				LoadEntity(child, entityView.transform);
			}
		}

		public void RegisterEntityView(GameEntityView entityView)
		{
			_dataToView.Add(entityView.LogicEntity, entityView);
		}

		public void DeregisterEntityView(GameEntityView entityView)
		{
			_dataToView.Remove(entityView.LogicEntity);
		}

		public GameEntityView GetView(IGameEntityInfo entity)
		{
			return _dataToView[entity];
		}
	}
}
