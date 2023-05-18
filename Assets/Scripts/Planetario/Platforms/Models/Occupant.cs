using System;
using Unity.Entities;

namespace Planetario.Platforms.Models
{
	/// <summary>
	/// make an entity occupy a location in a platform
	/// which prevents other entity from occupying the same
	/// </summary>
	[Serializable]
	public struct Occupant : IComponentData
	{
		public int location;
	}
}
