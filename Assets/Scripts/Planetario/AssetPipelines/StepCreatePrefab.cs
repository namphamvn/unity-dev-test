using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepCreatePrefab : BaseProcessor
	{
		public BaseProcessor input;
		public string path = "Assets/GeneratedPrefabs";
		public string prefix = "gen_";
		public string suffix;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
#if UNITY_EDITOR
			var outputs = new List<GameObject>();
			var inputs = input.GetOutputs();
			foreach (var input in inputs)
			{
				var path = $"{this.path}/{prefix}{input.name}{suffix}.prefab";
				var output = PrefabUtility.SaveAsPrefabAsset(input, path, out var success);
				if (!success)
				{
					throw new Exception($"Saving prefab {input.name} to path {path} failed");
				}

				outputs.Add(output);
			}

			return outputs;
#else
			return input.GetOutputs();
#endif
		}
	}
}
