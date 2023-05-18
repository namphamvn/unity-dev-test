namespace Planetario.Geometry.EvenBeziers
{
	public interface IEqualBezierParams : ICurveParams
	{
		float CurveRatio { get; }
		float EndToStartRatio { get; }
	}
}
