using Planetario.Geometry.Meshes;
using Planetario.Shares;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Planetario.Platforms.Views
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public abstract class MeshPlatformView<T> : PlatformView<T>
		where T : unmanaged, IComponentData, IMeshPlatform
	{
		private void Start()
		{
			//use start instead of awake so that the shape param can be initialized
			Compute();
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				Compute();
			}
		}

		private void Compute()
		{
			using var locations = new LocationNetwork(10, Allocator.TempJob);
			using var meshOutput = new MeshData(Allocator.TempJob);
			using var objectScale = new NativeArray<float>(1, Allocator.TempJob);
			var job = new PlatformBuilderJob<T>
			{
				input = Data,
				output = meshOutput,
				locationNetwork = locations,
				objectScale = objectScale
			};
			var jobHandle = job.Schedule();
			jobHandle.Complete();

			var mesh = meshOutput.ToMesh();

			mesh.name = "generated_mesh";
			RuntimeEditorCode.SetMesh(gameObject.GetComponent<MeshFilter>(), mesh);

			SetCollider();
		}

		protected virtual void SetCollider()
		{
		}
	}
}
