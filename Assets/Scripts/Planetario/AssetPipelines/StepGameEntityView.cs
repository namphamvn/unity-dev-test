using System.Collections.Generic;
using Planetario.GameWorlds.Views;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepGameEntityView : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			foreach (var input in inputs)
			{
				var view = input.GetComponent<GameEntityView>();
				view.SetGuid();
			}

			return inputs;
		}
	}
}
