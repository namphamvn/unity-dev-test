using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Planetario.Geometry.Meshes
{
	[BurstCompile]
	public struct PlatformBuilderJob<T> : IJob, IDisposable
		where T : struct, IMeshPlatform
	{
		//inputs
		public T input;

		//outputs
		public MeshData output;
		public LocationNetwork locationNetwork;
		public NativeArray<float> objectScale;

		public void Execute()
		{
			input.ComputeMeshPlatform(ref output, ref locationNetwork);
		}

		public void Dispose()
		{
			output.Dispose();
			locationNetwork.Dispose();
			objectScale.Dispose();
		}
	}
}
