using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepMaterialReplace : BaseProcessor
	{
		public BaseProcessor input;
		public Shader newShader;
		public string materialPath = "Assets/GeneratedPrefabs/Materials";
		public string materialPrefix = "gen_";

		private readonly Dictionary<Material, Material> _materialCache = new();

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			_materialCache.Clear();
			foreach (var myInput in inputs)
			{
				foreach (var render in myInput.GetComponentsInChildren<Renderer>())
				{
					Material[] newMats = null;
					for (var index = 0; index < render.sharedMaterials.Length; index++)
					{
						var mat = render.sharedMaterials[index];
						if (mat.shader == newShader)
						{
							continue;
						}

						newMats ??= new Material[render.sharedMaterials.Length];
						newMats[index] = GetMaterial(mat);
					}

					if (newMats != null)
					{
						render.materials = newMats;
					}
				}
			}
#if UNITY_EDITOR
			foreach (var newMat in _materialCache.Values)
			{
				AssetDatabase.CreateAsset(newMat,
					Path.Combine(materialPath, materialPrefix + newMat.name + ".mat"));
			}
#endif
			return inputs;
		}

		private Material GetMaterial(Material source)
		{
			if (!_materialCache.TryGetValue(source, out var newMat))
			{
				newMat = new Material(source)
				{
					shader = newShader
				};
				_materialCache.Add(source, newMat);
			}

			return newMat;
		}
	}
}
