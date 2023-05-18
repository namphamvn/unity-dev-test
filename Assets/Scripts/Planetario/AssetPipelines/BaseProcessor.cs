using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Planetario.AssetPipelines
{
	public abstract class BaseProcessor : MonoBehaviour
	{
		public List<GameObject> outputs;
		protected abstract IEnumerable<GameObject> ComputeOutput();

		[ContextMenu("ComputeOutput")]
		public void Output()
		{
			//clean up outputs in the scene
			var objects = SceneManager.GetActiveScene().GetRootGameObjects();
			var all = objects.SelectMany(obj => obj.GetComponentsInChildren<BaseProcessor>());
			foreach (var every in all)
			{
				every.outputs = null;
			}

			//compute output
			GetOutputs();
		}

		public IEnumerable<GameObject> GetOutputs()
		{
			return outputs ??= ComputeOutput().ToList();
		}
	}
}
