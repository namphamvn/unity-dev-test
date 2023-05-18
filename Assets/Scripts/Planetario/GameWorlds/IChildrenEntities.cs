using System.Collections.Generic;
using Planetario.GameWorlds.Models;

namespace Planetario.GameWorlds
{
	public interface IChildrenEntities
	{
		public IEnumerable<GameEntity> Children { get; }
	}

	public static class ChildrenEntitiesExtension
	{
		public static void DestroyChildren(this IChildrenEntities me)
		{
			foreach (var child in me.Children)
			{
				child.Destroy();
			}
		}

		public static IEnumerable<GameEntity> GetAllEntities(this IChildrenEntities me)
		{
			foreach (var child in me.Children)
			{
				yield return child;
				foreach (var recur in child.GetAllEntities())
				{
					yield return recur;
				}
			}
		}
	}
}
