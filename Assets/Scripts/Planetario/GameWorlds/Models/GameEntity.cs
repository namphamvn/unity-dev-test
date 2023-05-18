using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Unity.Entities;

namespace Planetario.GameWorlds.Models
{
	/// <summary>
	/// entity is the building block of the game
	/// entity can form a hierarchy
	/// entity can have multiple components, which can be changed at runtime
	/// </summary>
	[JsonObject(IsReference = true)]
	[Serializable]
	public class GameEntity : IChildrenEntities, IGameEntityInfo
	{
		private List<GameEntity> _children;

		[NonSerialized] private Dictionary<Type, IComponentData> _compMap;

		private List<IComponentData> _components;

		[NonSerialized] private GameEntity _parent;

		public GameEntity(Guid prefabGuid)
		{
			PrefabGuid = prefabGuid;
			_compMap = new Dictionary<Type, IComponentData>();
			_children = new List<GameEntity>();
		}

		public IEnumerable<GameEntity> Children => _children;

		public IGameEntityInfo Parent => _parent;
		public Guid PrefabGuid { get; }

		public IEnumerable<IGameEntityInfo> ChildrenInfo => _children;

		[field: NonSerialized] public event Action<IComponentData> OnDataChanged;

		[field: NonSerialized] public event Action OnDestroyed;

		[field: NonSerialized] public event Action<GameEntity> OnParentChanged;

		public TComp GetComponentDataWithExactType<TComp>() where TComp : IComponentData
		{
			return (TComp)_compMap[typeof(TComp)];
		}

		public bool HasComponentDataWithExactType<TComp>() where TComp : IComponentData
		{
			return _compMap.ContainsKey(typeof(TComp));
		}

		public bool TryGetComponentDataWithExactType<TComp>(out TComp data) where TComp : IComponentData
		{
			if (_compMap.TryGetValue(typeof(TComp), out var genericData))
			{
				data = (TComp)genericData;
				return true;
			}

			data = default;
			return false;
		}

		public TComp GetComponentData<TComp>() where TComp : IComponentData
		{
			foreach (var component in _compMap.Values)
			{
				if (component is TComp matched)
				{
					return matched;
				}
			}

			throw new Exception($"Component of type {typeof(TComp)} does not exist");
		}

		public IComponentData GetComponentData(Type compType)
		{
			foreach (var component in _compMap.Values)
			{
				if (compType == component.GetType())
				{
					return component;
				}
			}

			throw new Exception($"Component of type {compType} does not exist");
		}

		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			_components = new List<IComponentData>(_compMap.Values);
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			_compMap = _components.ToDictionary(value => value.GetType(), value => value);
			foreach (var child in _children)
			{
				child._parent = this;
			}
		}

		public void SetParent(GameEntity parent)
		{
			if (_parent == parent)
			{
				return;
			}

			if (_parent != null)
			{
				_parent._children.Remove(this);
			}

			var oldParent = _parent;
			_parent = parent;
			parent?._children.Add(this);
			OnParentChanged?.Invoke(oldParent);
		}

		public void RemoveEntity(GameEntity entity)
		{
			_children.Remove(entity);
		}

		public void SetComponentDataWithExactType<TComp>(TComp newValue) where TComp : IComponentData
		{
			_compMap[typeof(TComp)] = newValue;
			OnDataChanged?.Invoke(newValue);
		}

		public void SetComponentData<TComp>(TComp newValue) where TComp : IComponentData
		{
			foreach (var kv in _compMap)
			{
				if (kv.Value is TComp)
				{
					_compMap[kv.Key] = newValue;
					OnDataChanged?.Invoke(newValue);
					return;
				}
			}

			throw new Exception($"Component of type {typeof(TComp)} does not exist");
		}

		public void SetComponentData(IComponentData newValue)
		{
			var compType = newValue.GetType();
			foreach (var kv in _compMap)
			{
				if (kv.Value.GetType() == compType)
				{
					_compMap[kv.Key] = newValue;
					OnDataChanged?.Invoke(newValue);
					return;
				}
			}

			throw new Exception($"Component of type {compType} does not exist");
		}

		public void AddComponentData(IComponentData data)
		{
			_compMap.Add(data.GetType(), data);
		}

		public void Destroy()
		{
			this.DestroyChildren();
			OnDestroyed?.Invoke();
		}
	}
}
