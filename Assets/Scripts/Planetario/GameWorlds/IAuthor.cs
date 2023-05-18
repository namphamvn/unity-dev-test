using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Shares;
using UnityEngine;

namespace Planetario.GameWorlds
{
	public interface IAuthor
	{
		void BakeModel(GameEntity myEntity, GameEntity parent);
	}

	public static class AuthorExtensions
	{
		public static void BakeChildren<TAuthor>(this TAuthor author, GameEntity parent)
			where TAuthor : Component, IAuthor
		{
			foreach (var gameEntityView in author.transform.GetChildComponents<GameEntityView>())
			{
				gameEntityView.BakeModel(null, parent);
			}
		}
	}
}
