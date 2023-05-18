using System;
using Unity.Entities;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct ScreenSpace : IComponentData
	{
		public float planeDistance;
	}
}
