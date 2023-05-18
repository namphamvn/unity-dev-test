using System.Collections.Generic;
using Planetario.Shares;
using Planetario.Shares.MaterialServices;
using Unity.Mathematics;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.ClippingPortals
{
	public abstract class MaterialKeywordSetterForChildren : MonoBehaviour
	{
		public Collider[] excludedColliders;
		private Collider[] _colliders;
		private HashSet<Collider> _excludedColliders;
		private MaterialPropertyBlock _materialPropertyBlock;

		private Renderer[] _renderers;

		protected virtual void Awake()
		{
			_excludedColliders = new HashSet<Collider>(excludedColliders);
			ScanChildren();
			_materialPropertyBlock = new MaterialPropertyBlock();
		}

		private void Update()
		{
			UpdatePropertyBlock(_materialPropertyBlock);
			foreach (var rend in _renderers)
			{
				rend.SetPropertyBlock(_materialPropertyBlock);
				RuntimeEditorCode.SetDirty(rend);
			}
		}

		protected virtual void OnEnable()
		{
			foreach (var rend in _renderers)
			{
				if (!rend.gameObject.activeSelf)
				{
					continue;
				}

				if (rend.sharedMaterial.shader != MaterialService.Instance.urpCombineShader)
				{
					continue;
				}

				var materialKey = GetMaterialKey();
				materialKey.source = rend.sharedMaterial;
				var newMat = MaterialService.Instance.GetMaterial(materialKey);
				MaterialService.AssignMaterial(rend, newMat);
			}

			foreach (var myCollider in _colliders)
			{
				var clippedCollider = myCollider.gameObject.GetComponent<ClippedCollider>();
				if (clippedCollider == null)
				{
					clippedCollider = myCollider.gameObject.AddComponent<ClippedCollider>();
				}

				clippedCollider.clipper = this;
			}
		}

		private void OnTransformChildrenChanged()
		{
			ScanChildren();
			if (enabled)
			{
				OnEnable();
			}
		}

		private void ScanChildren()
		{
			_renderers = gameObject.GetComponentsInChildren<Renderer>();
			if (excludedColliders == null || excludedColliders.Length == 0)
			{
				_colliders = gameObject.GetComponentsInChildren<Collider>();
			}
			else
			{
				var list = new List<Collider>(gameObject.GetComponentsInChildren<Collider>());
				for (var i = list.Count - 1; i >= 0; i--)
				{
					if (_excludedColliders.Contains(list[i]))
					{
						list.RemoveAt(i);
					}
				}

				_colliders = list.ToArray();
			}
		}

		protected abstract MaterialKey GetMaterialKey();
		protected abstract void UpdatePropertyBlock(MaterialPropertyBlock block);

		public abstract bool IsChildClipped(in Ray ray, in float3 position, in float distance);

		protected static void AssignRenderQueue(Renderer rend, int newQueue)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				rend.material.renderQueue = newQueue;
			}
			else
			{
				rend.sharedMaterial.renderQueue = newQueue;
				RuntimeEditorCode.SetDirty(rend.sharedMaterial);
			}
#else
			rend.material.renderQueue = newQueue;
#endif
		}

		[ContextMenu("Preview in Editor")]
		private void PreviewInEditor()
		{
			Awake();
			OnEnable();
			Update();
			RuntimeEditorCode.SetDirty(this);
			RuntimeEditorCode.SetSceneDirty(gameObject);
			//RuntimeEditorCode.SaveAllAssets();
		}

		[ContextMenu("Restore in Editor", true)]
		private bool ValidateRestoreInEditor()
		{
			return _renderers != null;
		}


		[ContextMenu("Restore in Editor")]
		private void RestoreInEditor()
		{
			foreach (var rend in _renderers)
			{
				if (!rend.gameObject.activeSelf)
				{
					continue;
				}

				if (rend.sharedMaterial.shader != MaterialService.Instance.urpCombineShader)
				{
					continue;
				}

				var newMat = MaterialService.Instance.GetMaterialSource(rend.sharedMaterial);
				MaterialService.AssignMaterial(rend, newMat);
			}
		}
	}
}
