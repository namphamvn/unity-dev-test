using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepMakeLightStatic : BaseProcessor
	{
		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
#if UNITY_EDITOR
			foreach (var input in inputs)
			{
				foreach (var renderer in input.GetComponentsInChildren<Renderer>())
				{
					GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,
						StaticEditorFlags.ContributeGI);
				}
			}
#endif
			return inputs;
		}
	}
}
