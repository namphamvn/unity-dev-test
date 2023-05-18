using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepGenerateLightUV : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
#if UNITY_EDITOR
			foreach (var input in inputs)
			{
				var meshFilters = input.GetComponentsInChildren<MeshFilter>();
				foreach (var meshFilter in meshFilters)
				{
					if (meshFilter == null)
					{
						continue;
					}

					var path = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
					var importer = AssetImporter.GetAtPath(path) as ModelImporter;
					if (importer == null)
					{
						continue;
					}

					if (!importer.generateSecondaryUV)
					{
						importer.generateSecondaryUV = true;
						AssetDatabase.ImportAsset(path);
					}
				}
			}
#endif
			return inputs;
		}
	}
}
