using System.Collections.Generic;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	/// <summary>
	///     Scale the object
	/// </summary>
	public class StepScale : BaseProcessor
	{
		[Tooltip("Must contain BoxCollider")] public BaseProcessor input;

		public float scale = 1f;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			foreach (var input in inputs)
			{
				var inputTransform = input.transform;
				inputTransform.localScale *= scale;
			}

			return inputs;
		}
	}
}
