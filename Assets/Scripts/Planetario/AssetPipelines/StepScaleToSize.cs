using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	/// <summary>
	///     Scale the object to fit inside a box of a specific size
	/// </summary>
	public class StepScaleToSize : BaseProcessor
	{
		[Tooltip("Must contain BoxCollider")] public BaseProcessor input;

		public float size = 1f;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs().ToArray();
			foreach (var myInput in inputs)
			{
				var inputTransform = myInput.transform;
				var boxCollider = myInput.GetComponent<BoxCollider>();
				var boxSize = boxCollider.size;
				var boxMaxSize = Mathf.Max(Mathf.Max(boxSize.x, boxSize.y), boxSize.z);
				inputTransform.localScale *= size / boxMaxSize;
			}

			return inputs;
		}
	}
}
