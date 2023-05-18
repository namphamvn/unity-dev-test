using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct Droppable : IComponentData
	{
		public float3 liftDir;
		public float liftDistance;
		public float returnDuration;
		public bool cloneOnDrop;
	}
}
