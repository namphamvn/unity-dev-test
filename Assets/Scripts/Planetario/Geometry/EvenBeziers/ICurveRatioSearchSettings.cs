namespace Planetario.Geometry.EvenBeziers
{
	public interface ICurveRatioSearchSettings
	{
		float DeltaEpsilon { get; }
		float DeltaStepPercentage { get; }
		int MaxAttempt { get; }
	}
}
