using System;
using Unity.Entities;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct Occupant : IComponentData
	{
		public int location;
	}
}
