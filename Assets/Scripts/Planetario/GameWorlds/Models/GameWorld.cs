using System;
using System.Collections.Generic;
using Planetario.GameWorlds.Services;
using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds.Models
{
	[Serializable]
	public class GameWorld : IChildrenEntities, IUndoState
	{
		private List<GameEntity> _entities;

		private List<ICommand>
			_undoCommands; //use a list and not a stack here due to serialization complication

		public GameWorld()
		{
			_entities = new List<GameEntity>();
			_undoCommands = new List<ICommand>();
		}

		public IEnumerable<GameEntity> Children => _entities;

		public bool HasUndo()
		{
			return _undoCommands.Count > 0;
		}

		/// <summary>
		///     This is for command which can create its undo before it is executed
		/// </summary>
		public void PushUndoCommand(IUndoableCommand command, GameWorldSystem worldSystem)
		{
			_undoCommands.Add(command.GetUndoCommand(worldSystem));
		}

		public ICommand PopUndoCommand()
		{
			var lastIndex = _undoCommands.Count - 1;
			var cmd = _undoCommands[lastIndex];
			_undoCommands.RemoveAt(lastIndex);
			return cmd;
		}

		[field: NonSerialized] public event Action<IGameEntityInfo> OnEntityAdded;

		public GameEntity AddEntity(Guid prefabGuid, GameEntity parent)
		{
			var entity = new GameEntity(prefabGuid);
			if (parent != null)
			{
				entity.SetParent(parent);
			}
			else
			{
				_entities.Add(entity);
			}

			return entity;
		}

		public void AddEntity(GameEntity entity, GameEntity parent)
		{
			if (parent == null)
			{
				_entities.Add(entity);
			}
			else
			{
				entity.SetParent(parent);
			}

			OnEntityAdded?.Invoke(entity);
		}

		public GameEntity Instantiate(GameEntity source, GameEntity parent)
		{
			var newEntity = PersistenceService.Instance.Clone(source);
			AddEntity(newEntity, parent);
			return newEntity;
		}

		public void Destroy()
		{
			this.DestroyChildren();
			_entities.Clear();
		}

		/// <summary>
		///     This is for command that can only create undo command after it is executed
		/// </summary>
		public void PushManualUndoCommand(ICommand command)
		{
			_undoCommands.Add(command);
		}
	}
}
