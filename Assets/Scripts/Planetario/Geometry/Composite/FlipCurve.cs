using Unity.Mathematics;

namespace Planetario.Geometry.Composite
{
	public struct FlipCurve : ICurve3D
	{
		public ICurve3D Source { get; }

		public FlipCurve(ICurve3D source)
		{
			Source = source;
		}

		public float3 Point(float coord)
		{
			return Source.Point(1 - coord);
		}

		public float3 Tangent(float coord)
		{
			return -Source.Tangent(1 - coord);
		}

		public float Length()
		{
			return Source.Length();
		}

		ICurve3D ICurve3D.OffsetCurve(float offset, float3 up)
		{
			return new OffsetCurve(this, offset, up);
		}
	}
}
