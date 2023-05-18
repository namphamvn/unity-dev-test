using Unity.Entities;

namespace Planetario.GameWorlds
{
	public interface IView<TComp> : IView
	{
		TComp Data { get; }
	}

	public interface IView
	{
		IComponentData GenericData { get; }
	}
}
