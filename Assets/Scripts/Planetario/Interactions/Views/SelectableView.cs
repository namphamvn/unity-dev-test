using System;
using Planetario.GameWorlds.Views;
using Planetario.Interactions.Models;
using Planetario.Shares.MaterialServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Planetario.Interactions.Views
{
	/// <summary>
	/// click to toggle selection on an entity
	/// </summary>
	public class SelectableView : BaseComponentView<Selectable>, IPointerClickHandler, IBeginDragHandler,
		IEndDragHandler
	{
		private const string MaterialKeyword = "_PL_TINT";
		private static readonly int TintColorParam = Shader.PropertyToID("_TintColor");

		public Renderer[] renderers;

		[NonSerialized] private bool _blockClick;

		private MaterialPropertyBlock _materialPropertyBlock;

		private Action _unsubscribe;

		private void OnDestroy()
		{
			_unsubscribe?.Invoke();
			_unsubscribe = null;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_blockClick = true;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_blockClick = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_blockClick)
			{
				return;
			}

			SendCommand(new ToggleSelectionCommand
			{
				entity = LogicEntity
			});
		}

		public override void ResetData()
		{
			_unsubscribe = LogicEntity.SubscribeOnDataChangedExactType<Selectable>(OnDataChanged);
		}

		private void OnDataChanged(Selectable selectable)
		{
			_materialPropertyBlock ??= new MaterialPropertyBlock();
			_materialPropertyBlock.SetColor(TintColorParam, selectable.isSelected ? Color.red : Color.white);
			foreach (var rend in renderers)
			{
				var newMat =
					EntityView.World.MaterialService.GetMaterial(new MaterialKey(rend.sharedMaterial,
						MaterialKeyword));
				MaterialService.AssignMaterial(rend, newMat);
				rend.SetPropertyBlock(_materialPropertyBlock);
			}
		}
	}
}
