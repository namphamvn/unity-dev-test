namespace Planetario.Geometry.Primitive
{
	public interface IRaycastable
	{
		bool Raycast(Ray ray, out float enter);
	}
}
