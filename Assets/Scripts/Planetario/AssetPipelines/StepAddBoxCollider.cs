using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	public class StepAddBoxCollider : BaseProcessor
	{
		private static readonly Vector3[] _corners =
		{
			new Vector3(1, 1, 1),
			new Vector3(-1, 1, 1),
			new Vector3(1, -1, 1),
			new Vector3(1, 1, -1),
			new Vector3(-1, -1, 1),
			new Vector3(-1, 1, -1),
			new Vector3(1, -1, -1),
			new Vector3(-1, -1, -1)
		};

		public BaseProcessor input;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs().ToList();
			foreach (var input in inputs)
			{
				var inputTransform = input.transform;
				var commonBound = new Bounds(Vector3.zero, Vector3.zero);
				var meshFilters = input.GetComponentsInChildren<MeshFilter>();
				foreach (var meshFilter in meshFilters)
				{
					var localBound = meshFilter.sharedMesh.bounds;
					foreach (var corner in _corners)
					{
						var localPoint = localBound.center + Vector3.Scale(localBound.extents, corner);
						var worldPoint = meshFilter.transform.TransformPoint(localPoint);
						commonBound.Encapsulate(inputTransform.InverseTransformPoint(worldPoint));
					}
				}

				var boxCollider = input.AddComponent<BoxCollider>();
				boxCollider.center = commonBound.center;
				boxCollider.size = commonBound.size;
			}

			return inputs;
		}
	}
}
