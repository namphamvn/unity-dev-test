using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Platforms.Models
{
	/// <summary>
	/// make entity able to be drag and drop on <see cref="IPlatform"/>
	/// </summary>
	[Serializable]
	public struct Droppable : IComponentData
	{
		public float3 liftDir;
		public float liftDistance;
		public float returnDuration;
		public bool cloneOnDrop;
	}
}
