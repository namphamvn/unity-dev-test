using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepRemoveBoxCollider : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs().ToList();
			foreach (var iInput in inputs)
			{
				var colliders = iInput.GetComponentsInChildren<Collider>();
				foreach (var iCollider in colliders)
				{
					DestroyImmediate(iCollider);
				}
			}

			return inputs;
		}
	}
}
