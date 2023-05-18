namespace Planetario.Geometry.Meshes
{
	public interface IMeshPlatform : IPlatform
	{
		void ComputeMeshPlatform(ref MeshData output, ref LocationNetwork locationNetwork);
	}
}
