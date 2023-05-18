using Planetario.Geometry.Primitive;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models.Constraints
{
	public struct DragState : IComponentData
	{
		public float3 startTouchPosition;
		public Ray currentRay;
	}
}
