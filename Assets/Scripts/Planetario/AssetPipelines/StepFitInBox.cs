using System.Collections.Generic;
using Planetario.Shares;
using UnityEngine;

namespace Planetario.AssetPipelines
{
	/// <summary>
	///     Put the object inside a container with a pivot at the center of the box collider
	/// </summary>
	public class StepFitInBox : BaseProcessor
	{
		[Tooltip("Must contain BoxCollider")] public BaseProcessor input;

		[Tooltip("Target scale of the object")]
		public SerializableNullable<float> size;

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			var myOutputs = new List<GameObject>();
			foreach (var myInput in inputs)
			{
				var inputTransform = myInput.transform;
				var oldPos = inputTransform.position;

				var container = new GameObject(myInput.name);
				var containerTransform = container.transform;
				containerTransform.SetParent(inputTransform.parent);

				var boxCollider = myInput.GetComponent<BoxCollider>();
				var localBottomCenter = boxCollider.center + 0.5f * boxCollider.size.y * Vector3.down;
				var worldBottomCenter = inputTransform.TransformPoint(localBottomCenter);
				containerTransform.position = worldBottomCenter;

				inputTransform.SetParent(containerTransform, true);
				containerTransform.position = oldPos;

				if (size.HasValue)
				{
					var boxSize = boxCollider.size;
					var boxMaxSize = Mathf.Max(Mathf.Max(boxSize.x, boxSize.y), boxSize.z);
					var newScale = size.Value / boxMaxSize;
					inputTransform.localScale *= newScale;
					inputTransform.localPosition *= newScale;
				}

				myOutputs.Add(container);
			}

			return myOutputs;
		}
	}
}
