using System;
using Unity.Entities;

namespace Planetario.Interactions.Models
{
	/// <summary>
	/// whether an entity is selectable
	/// after selected, further commands can be performed on them, such as Deletion
	/// </summary>
	[Serializable]
	public struct Selectable : IComponentData
	{
		public bool isSelected;
	}
}
