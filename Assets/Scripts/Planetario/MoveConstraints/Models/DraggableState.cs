using Planetario.Geometry.Primitive;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.MoveConstraints.Models
{
	/// <summary>
	///     state created when a dragging operation starts
	/// </summary>
	public struct DraggableState : IComponentData
	{
		/// <summary>
		///     world position of where the drag starts
		///     for mobile, it's the touch position
		///     for vr, it's where the laser pointer intersect with the draggable object
		/// </summary>
		public float3 touchPosition;

		/// <summary>
		///     the starting position and rotation of the draggable object
		/// </summary>
		public Location location;
	}
}
