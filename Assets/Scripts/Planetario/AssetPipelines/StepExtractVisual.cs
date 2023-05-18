using System.Collections.Generic;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	/// <summary>
	///     Extract the visual part from AssetEntry
	/// </summary>
	public class StepExtractVisual : BaseProcessor
	{
		[Tooltip("Must contain AssetEntry")] public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			var outputs = new List<GameObject>();
			foreach (var input in inputs)
			{
				var entry = input.GetComponent<StepAssetEntry>();
				var output = Instantiate(entry.visual);
				output.name = input.name;
				outputs.Add(output);
			}

			return outputs;
		}
	}
}
