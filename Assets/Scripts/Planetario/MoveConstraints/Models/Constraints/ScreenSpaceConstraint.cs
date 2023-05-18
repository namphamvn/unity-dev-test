using Planetario.Geometry.Primitive;
using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.MoveConstraints.Models.Constraints
{
	/// <summary>
	///     Restrict the movement to screen space
	///     i.e. the plane parallel with the screen, from the camera perspective
	///     the origin of the plane is the same as the object origin
	/// </summary>
	public class ScreenSpaceConstraint : MoveConstraint
	{
		public override Location ComputeConstraint(in DraggableState state, Ray currentRay)
		{
			var planarConstraint = GetConstraint(state.touchPosition);
			return planarConstraint.ComputeConstraint(in state, currentRay);
		}

		public static PlanarConstraint GetConstraint(float3 position)
		{
			var camTransform = Camera.main.transform;
			// var normal = -camTransform.forward;
			var right = camTransform.right;
			var forward = camTransform.up;
			return new PlanarConstraint
			{
				plane = new CartesianPlane(position, right, forward)
			};
		}
	}
}
