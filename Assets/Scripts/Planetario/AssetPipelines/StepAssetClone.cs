using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepAssetClone : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs().ToList();
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			var outputs = new List<GameObject>();
			foreach (var input in inputs)
			{
				var newObj = Instantiate(input);
				newObj.name = input.name;
				newObj.transform.SetParent(transform, false);
				outputs.Add(newObj);
			}

			return outputs;
		}
	}
}
