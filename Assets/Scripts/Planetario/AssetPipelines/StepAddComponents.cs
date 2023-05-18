using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepAddComponents : BaseProcessor
	{
		public BaseProcessor input;
		public Component[] components;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs().ToList();
#if UNITY_EDITOR
			foreach (var input in inputs)
			{
				foreach (var comp in components)
				{
					var outComp = input.AddComponent(comp.GetType());
					EditorUtility.CopySerialized(comp, outComp);
				}
			}
#endif
			return inputs;
		}
	}
}
