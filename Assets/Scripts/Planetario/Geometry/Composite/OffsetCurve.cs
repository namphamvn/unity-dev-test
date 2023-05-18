using Planetario.Geometry.Extensions;
using Unity.Mathematics;

namespace Planetario.Geometry.Composite
{
	public struct OffsetCurve : ICurve3D
	{
		public ICurve3D Source { get; }
		public float Offset { get; }
		public float3 Up { get; }

		public OffsetCurve(ICurve3D source, float offset, float3 up)
		{
			Source = source;
			Offset = offset;
			Up = up;
		}

		public float3 Point(float coord)
		{
			return Source.Offset(coord, Up, Offset);
		}

		public float3 Tangent(float coord)
		{
			return Source.Tangent(coord);
		}

		public float Length()
		{
			return this.ApproximateLength(VectorExtensions.ComputeLengthSegmentCount);
		}

		ICurve3D ICurve3D.OffsetCurve(float offset, float3 up)
		{
			return new OffsetCurve(this, offset, up);
		}
	}
}
