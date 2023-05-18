using System;
using Unity.Entities;

namespace Planetario.MoveConstraints.Models
{
	[Serializable]
	public struct LocalScale : IComponentData
	{
		public float value;
	}
}
