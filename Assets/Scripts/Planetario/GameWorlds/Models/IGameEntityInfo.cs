using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Entities;

namespace Planetario.GameWorlds.Models
{
	/// <summary>
	/// read-only version of <see cref="GameEntity"/>
	/// this is used by the View classes
	/// </summary>
	[JsonObject(IsReference = true)]
	public interface IGameEntityInfo
	{
		IGameEntityInfo Parent { get; }
		Guid PrefabGuid { get; }
		IEnumerable<IGameEntityInfo> ChildrenInfo { get; }
		event Action<IComponentData> OnDataChanged;
		event Action OnDestroyed;
		event Action<GameEntity> OnParentChanged;
		TComp GetComponentDataWithExactType<TComp>() where TComp : IComponentData;
		bool HasComponentDataWithExactType<TComp>() where TComp : IComponentData;
		bool TryGetComponentDataWithExactType<TComp>(out TComp data) where TComp : IComponentData;
		TComp GetComponentData<TComp>() where TComp : IComponentData;
		IComponentData GetComponentData(Type compType);
	}
}
