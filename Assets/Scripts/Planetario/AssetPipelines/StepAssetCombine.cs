using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepAssetCombine : BaseProcessor
	{
		public BaseProcessor[] inputs;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			return inputs.SelectMany(input => input.GetOutputs());
		}
	}
}
