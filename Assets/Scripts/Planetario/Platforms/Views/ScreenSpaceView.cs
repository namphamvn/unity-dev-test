using Planetario.GameWorlds.Views;
using Planetario.Platforms.Models;
using UnityEngine;

namespace Planetario.Platforms.Views
{
	/// <summary>
	/// make it possible to use RectTransform for 3d object
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class ScreenSpaceView : BaseComponentView<ScreenSpace>
	{
		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				ResetData();
			}
		}

		public override void ResetData()
		{
			var canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = Camera.main;
			canvas.planeDistance = Data.planeDistance;
		}
	}
}
