using System.Collections.Generic;
using Planetario.Interactions.Views;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepSelectableView : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			foreach (var input in inputs)
			{
				var view = input.AddComponent<SelectableView>();
				view.renderers = view.GetComponentsInChildren<Renderer>();
			}

			return inputs;
		}
	}
}
