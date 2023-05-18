using System.Collections.Generic;
using UnityEngine;

namespace Planetario.Shares
{
	public static class TransformExtensions
	{
		public static IEnumerable<T> GetChildComponents<T>(this Transform transform)
			where T : Component
		{
			for (var i = 0; i < transform.childCount; i++)
			{
				var childComp = transform.GetChild(i).GetComponent<T>();
				if (childComp != null)
				{
					yield return childComp;
				}
			}
		}
	}
}
