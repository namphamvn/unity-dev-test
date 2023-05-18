using Unity.Entities;

namespace Planetario.GameWorlds
{
	/// <summary>
	/// A View visualize the Model. It could read info from the Model and listen to events
	/// </summary>
	/// <typeparam name="TComp">Type of the component that is attached to an entity</typeparam>
	public interface IView<TComp> : IView
	{
		TComp Data { get; }
	}

	public interface IView
	{
		IComponentData GenericData { get; }
	}
}
