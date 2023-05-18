using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Planetario.Shares.MaterialServices
{
	public class MaterialService : MonoBehaviour, ISerializationCallbackReceiver
	{
		public Shader urpCombineShader;
		[SerializeField] private List<Entry> materials;
		[SerializeField] private List<KeywordKey> keywordKeys;

		private readonly Dictionary<MaterialKey, Material> _cache = new();
		private readonly Dictionary<KeywordKey, LocalKeyword> _keywords = new();
		private readonly Dictionary<Material, MaterialKey> _reverseCache = new();

		public static MaterialService Instance { get; private set; }
		public IEnumerable<Material> Materials => _cache.Values;
		public IEnumerable<KeywordKey> Keywords => _keywords.Keys;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public void OnBeforeSerialize()
		{
			materials = new List<Entry>(_cache.Count);
			foreach (var item in _cache)
			{
				materials.Add(new Entry
				{
					key = item.Key,
					generatedMaterial = item.Value
				});
			}

			keywordKeys = new List<KeywordKey>(_keywords.Keys);
		}

		public void OnAfterDeserialize()
		{
			foreach (var key in keywordKeys)
			{
				GetKeyword(key);
			}

			foreach (var item in materials)
			{
				_cache.Add(item.key, item.generatedMaterial);
				_reverseCache.Add(item.generatedMaterial, item.key);

				//this is to restore renderQueue because this info is lost
				//whenever a scene was saved or loaded
				item.generatedMaterial.renderQueue = item.key.renderQueue;
			}
		}

		public Material GetMaterial(MaterialKey key)
		{
			if (_reverseCache.TryGetValue(key.source, out var existingKey))
			{
				key.source = existingKey.source;
			}

			if (!_cache.TryGetValue(key, out var result))
			{
				var keyword = GetKeyword(new KeywordKey(key.source.shader, key.keyword));
				result = new Material(key.source);
				result.name = key.ToString();
				result.SetKeyword(keyword, true);
				result.renderQueue = key.renderQueue;
				RuntimeEditorCode.SetDirty(result);

				_cache.Add(key, result);
				_reverseCache.Add(result, key);
				RuntimeEditorCode.SetDirty(this);
			}

			return result;
		}

		public static void AssignMaterial(Renderer rend, Material newMat)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				rend.material = newMat;
			}
			else
			{
				rend.sharedMaterial = newMat;
			}
#else
			rend.material = newMat;
#endif
			RuntimeEditorCode.SetDirty(rend);
		}

		private LocalKeyword GetKeyword(KeywordKey key)
		{
			if (!_keywords.TryGetValue(key, out var keyword))
			{
				keyword = new LocalKeyword(key.shader, key.keyword);
				_keywords.Add(key, keyword);
				RuntimeEditorCode.SetDirty(this);
			}

			return keyword;
		}

		public Material GetMaterialSource(Material mat)
		{
			if (_reverseCache.TryGetValue(mat, out var sourceKey))
			{
				return sourceKey.source;
			}

			throw new Exception($"This material [{mat.name}] is not generated by {nameof(MaterialService)}");
		}

		/// <summary>
		///     this is to restore renderQueue because this info is lost
		///     whenever a scene was saved or loaded
		/// </summary>
		[ContextMenu("Restore Materials")]
		public void RestoreRenderQueue()
		{
			foreach (var item in _cache)
			{
				item.Value.renderQueue = item.Key.renderQueue;
			}
		}

		[Serializable]
		public struct Entry
		{
			public MaterialKey key;
			public Material generatedMaterial;
		}
	}
}
