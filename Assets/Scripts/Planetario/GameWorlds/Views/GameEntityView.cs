using System;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Services;
using Unity.Entities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.GameWorlds.Views
{
	/// <summary>
	/// visualize an entity
	/// also acts as an Author, create Model from the prefab during scene load
	/// </summary>
	public class GameEntityView : MonoBehaviour, IAuthor
	{
		public string prefabGuid;
		public GameWorldView World { get; private set; }
		[field: NonSerialized] public IGameEntityInfo LogicEntity { get; private set; }

		public void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			Initialize();
			myEntity = World.World.AddEntity(new Guid(prefabGuid), parent);
			SetEntity(myEntity);
			foreach (var compView in GetComponents<BaseComponentView>())
			{
				compView.BakeModel(myEntity, parent);
				compView.ResetData();
			}

			this.BakeChildren(myEntity);
		}

		public void Initialize()
		{
			World = GetComponentInParent<GameWorldView>();
		}

		public TComp GetComponentDataWithExactType<TComp>()
			where TComp : IComponentData
		{
			return LogicEntity.GetComponentDataWithExactType<TComp>();
		}

		[ContextMenu("Set GUID From Prefab")]
		public void SetGuid()
		{
#if UNITY_EDITOR
			var path = AssetDatabase.GetAssetPath(this);
			prefabGuid = AssetDatabase.AssetPathToGUID(path);
			EditorUtility.SetDirty(this);
#endif
		}

		[ContextMenu("Print Data")]
		public void PrintData()
		{
			Debug.Log(PersistenceService.Instance.Serialize(LogicEntity));
		}

		public void SetEntity(IGameEntityInfo entity)
		{
			LogicEntity = entity;
			LogicEntity.OnDestroyed += OnDestroyed;
			LogicEntity.OnParentChanged += OnParentChanged;
			World.RegisterEntityView(this);
		}

		private void OnParentChanged(GameEntity oldParent)
		{
			var parentView = World.GetView(LogicEntity.Parent);
			transform.parent = parentView.transform;
		}

		private void OnDestroyed()
		{
			LogicEntity.OnParentChanged -= OnParentChanged;
			LogicEntity.OnDestroyed -= OnDestroyed;
			World.DeregisterEntityView(this);
			Destroy(gameObject);
		}
	}
}
