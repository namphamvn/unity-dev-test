using Planetario.Geometry.Primitive;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.MoveConstraints.Models.Constraints
{
	public abstract class MoveConstraint : MonoBehaviour
	{
		public virtual void Initialize()
		{
		}

		public abstract Location ComputeConstraint(in DraggableState state, Ray currentRay);
	}
}
