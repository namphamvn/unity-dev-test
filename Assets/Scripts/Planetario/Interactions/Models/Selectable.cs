using System;
using Unity.Entities;

namespace Planetario.Interactions.Models
{
	[Serializable]
	public struct Selectable : IComponentData
	{
		public bool isSelected;
	}
}
