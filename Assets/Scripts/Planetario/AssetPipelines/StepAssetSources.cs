using System.Collections.Generic;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepAssetSources : BaseProcessor
	{
		public GameObject[] objects;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var computeOutput = new List<GameObject>();
			foreach (var prefab in objects)
			{
				var obj = Instantiate(prefab, transform);
				obj.name = prefab.name;
				computeOutput.Add(obj);
			}

			return computeOutput;
		}
	}
}
