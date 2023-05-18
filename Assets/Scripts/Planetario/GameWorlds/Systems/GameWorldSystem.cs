using System.Collections.Generic;
using System.Linq;
using Planetario.GameWorlds.Models;

namespace Planetario.GameWorlds.Systems
{
	/// <summary>
	/// manage all active systems for <see cref="GameWorld"/>
	/// </summary>
	public class GameWorldSystem
	{
		private readonly List<BaseSystem> _systems;

		public GameWorldSystem(GameWorld world)
		{
			World = world;
			_systems = new List<BaseSystem>();
		}

		public GameWorld World { get; }

		public void AddSystem(BaseSystem system)
		{
			system.SetGameWorld(this);
			_systems.Add(system);
		}

		public T GetSystem<T>() where T : BaseSystem
		{
			foreach (var system in _systems)
			{
				if (system is T result)
				{
					return result;
				}
			}

			return null;
		}

		public void ReceiveCommand<TCommand>(TCommand command) where TCommand : ICommand
		{
			if (command is IUndoableCommand undoableCommand)
			{
				World.PushUndoCommand(undoableCommand, this);
			}

			foreach (var system in _systems)
			{
				if (system is ISystem<TCommand> commandSystem)
				{
					commandSystem.OnCommandReceived(command);
				}
			}
		}

		public void Undo()
		{
			if (World.HasUndo())
			{
				var cmd = World.PopUndoCommand();
				ExecuteCommand(cmd);
			}
		}

		public void ExecuteCommand(ICommand cmd)
		{
			var cmdType = cmd.GetType();
			var systemType = typeof(ISystem<>).MakeGenericType(cmdType);
			foreach (var system in _systems)
			{
				if (systemType.IsInstanceOfType(system))
				{
					var methodInfo = system.GetType().GetMethods()
						.First(method => method.Name == nameof(ISystem<ICommand>.OnCommandReceived) &&
						                 method.GetParameters()[0].ParameterType == cmdType);
					methodInfo.Invoke(system, new object[] { cmd });
				}
			}
		}
	}
}
