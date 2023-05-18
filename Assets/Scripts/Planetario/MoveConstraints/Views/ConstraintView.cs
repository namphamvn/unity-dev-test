using Planetario.GameWorlds.Views;
using Planetario.MoveConstraints.Models.Constraints;

namespace Planetario.MoveConstraints.Views
{
	public abstract class ConstraintView<TConstraint> : BaseComponentView<TConstraint>, IConstraintView
		where TConstraint : unmanaged, IConstraint
	{
	}
}
