using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepGridArrange : BaseProcessor
	{
		public BaseProcessor input;
		public float size;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			var inputs = input.GetOutputs().ToList();
			var sqrt = Mathf.CeilToInt(Mathf.Sqrt(inputs.Count()));
			var index = 0;
			foreach (var obj in inputs)
			{
				var x = index / sqrt;
				var z = index % sqrt;
				obj.transform.SetParent(transform);
				obj.transform.localPosition = new Vector3(x, 0, z) * size;
				index++;
			}

			return inputs;
		}
	}
}
