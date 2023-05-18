using System;
using Planetario.GameWorlds.Models;
using Unity.Entities;

namespace Planetario.GameWorlds.Views
{
	public static class GameEntityInfoExtensions
	{
		public static Action SubscribeOnDataChanged<TComp>(this IGameEntityInfo entity,
			Action<TComp> onDataChanged)
			where TComp : IComponentData
		{
			onDataChanged(entity.GetComponentData<TComp>());
			entity.OnDataChanged += OnDataChanged;

			void OnDataChanged(IComponentData obj)
			{
				if (obj is TComp componentData)
				{
					onDataChanged(componentData);
				}
			}

			return () => entity.OnDataChanged -= OnDataChanged;
		}

		public static Action SubscribeOnDataChangedExactType<TComp>(this IGameEntityInfo entity,
			Action<TComp> onDataChanged)
			where TComp : struct, IComponentData
		{
			onDataChanged(entity.GetComponentDataWithExactType<TComp>());
			entity.OnDataChanged += OnDataChanged;

			void OnDataChanged(IComponentData obj)
			{
				if (obj is TComp componentData)
				{
					onDataChanged(componentData);
				}
			}

			return () => entity.OnDataChanged -= OnDataChanged;
		}
	}
}
