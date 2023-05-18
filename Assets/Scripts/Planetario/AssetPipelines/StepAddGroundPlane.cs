using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Planetario.AssetPipelines
{
	public class StepAddGroundPlane : BaseProcessor
	{
		public BaseProcessor input; //Must contain BoxCollider
		public GameObject footprintPrefab;
		public float scale = 1.1f;
		public bool equalSize;
		public float size = 30f;
		public float footprintLightmapScale = 0.5f;
		public string suffix = "_ground";

		protected override IEnumerable<GameObject> ComputeOutput()
		{
			var inputs = input.GetOutputs();
			var outputs = new List<GameObject>();
			foreach (var input in inputs)
			{
				var inputTransform = input.transform;
				var oldPos = inputTransform.position;
				var boxCollider = input.GetComponent<BoxCollider>();
				var plane = Instantiate(footprintPrefab, inputTransform.parent);
				plane.gameObject.SetActive(true);
				var localBottomCenter = boxCollider.center + 0.5f * boxCollider.size.y * Vector3.down;
				var worldBottomCenter = input.transform.TransformPoint(localBottomCenter);

				var planeTransform = plane.transform;
				var boxSize = boxCollider.size;
				var boxMaxSize = Mathf.Max(Mathf.Max(boxSize.x, boxSize.y), boxSize.z);
				planeTransform.localScale = scale * boxMaxSize * 0.1f * Vector3.one;
				planeTransform.position = worldBottomCenter;
//				planeTransform.rotation = Quaternion.Euler(90, 0, 0);
				plane.name = input.name + suffix;
				SetLightMapScale(plane.GetComponent<Renderer>(), footprintLightmapScale);

				inputTransform.SetParent(planeTransform, true);
				planeTransform.position = oldPos;

				if (equalSize)
				{
					planeTransform.localScale *= size / boxMaxSize;
				}

				outputs.Add(plane);
			}

			return outputs;
		}

		private static void SetLightMapScale(Renderer rend, float val)
		{
#if UNITY_EDITOR
			//set ground light map scale
			var so = new SerializedObject(rend);
			so.FindProperty("m_ScaleInLightmap").floatValue = val;
			so.ApplyModifiedProperties();
#endif
		}
	}
}
