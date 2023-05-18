namespace Planetario.Geometry
{
	public interface ICurve<out T>
	{
		T Point(float coord);
		T Tangent(float coord);
		float Length();
	}
}
