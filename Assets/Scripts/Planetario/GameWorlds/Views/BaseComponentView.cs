using Planetario.GameWorlds.Models;
using Unity.Entities;
using UnityEngine;

namespace Planetario.GameWorlds.Views
{
	/// <summary>
	/// base class for visualizing a component
	/// this view is not restricted to only read or listen to the "default" component that it is interested in
	/// a view can read or listen to multiple components
	/// </summary>
	public abstract class BaseComponentView : MonoBehaviour, IView, IAuthor
	{
		public abstract void BakeModel(GameEntity myEntity, GameEntity parent);
		public abstract IComponentData GenericData { get; }

		public abstract void Initialize();

		public virtual void ResetData()
		{
		}
	}

	[RequireComponent(typeof(GameEntityView))]
	public abstract class BaseComponentView<TData> : BaseComponentView, IView<TData>
		where TData : IComponentData
	{
		public TData authoredData;
		public GameEntityView EntityView { get; private set; }
		public IGameEntityInfo LogicEntity => EntityView.LogicEntity;

		public virtual TData Data
		{
			get
			{
#if UNITY_EDITOR
				if (Application.isPlaying)
				{
					return EntityView.GetComponentDataWithExactType<TData>();
				}

				return authoredData;
#else
				return EntityView.GetComponentDataWithExactType<TData>();
#endif
			}
		}

		public override IComponentData GenericData => Data;

		public sealed override void Initialize()
		{
			EntityView = GetComponent<GameEntityView>();
		}

		protected void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
		{
			EntityView.World.WorldSystem.ReceiveCommand(command);
		}

		public override void BakeModel(GameEntity myEntity, GameEntity parent)
		{
			Initialize();
			myEntity.AddComponentData(authoredData);
		}
	}
}
