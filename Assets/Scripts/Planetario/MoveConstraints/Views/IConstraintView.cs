using Planetario.GameWorlds;
using Planetario.MoveConstraints.Models.Constraints;

namespace Planetario.MoveConstraints.Views
{
	public interface IConstraintView : IView
	{
	}

	public static class ConstraintViewExtension
	{
		public static IConstraint GetConstraintData(this IConstraintView view)
		{
			return (IConstraint)view.GenericData;
		}
	}
}
