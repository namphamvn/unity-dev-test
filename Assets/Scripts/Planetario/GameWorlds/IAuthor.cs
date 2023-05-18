using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Views;
using Planetario.Shares;
using UnityEngine;

namespace Planetario.GameWorlds
{
	/// <summary>
	/// An Author creates the Model / Data. It is used for scene and prefab design
	/// In this project, I took the shortcut that View is also acts as an Author
	/// </summary>
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
