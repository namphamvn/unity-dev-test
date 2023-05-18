using Unity.Mathematics;

namespace Planetario.Shares
{
	public static class MatrixExtensions
	{
		/// <summary>Transforms a 3D point by a 4x4 transformation matrix.</summary>
		/// <param name="m">A transformation matrix</param>
		/// <param name="p">A 3D position</param>
		/// <returns>
		///     A vector containing the transformed point.
		/// </returns>
		public static float3 TransformPoint(in this float4x4 m, in float3 p)
		{
			return math.mul(m, new float4(p, 1)).xyz;
		}

		/// <summary>Transforms a 3D direction by a 4x4 transformation matrix.</summary>
		/// <param name="m">A transformation matrix</param>
		/// <param name="d">A vector representing a direction in 3D space. This vector does not need to be normalized.</param>
		/// <returns>
		///     A vector containing the transformed direction. This vector will not necessarily be unit-length.
		/// </returns>
		public static float3 TransformDirection(in this float4x4 m, in float3 d)
		{
			return math.rotate(m, d);
		}
	}
}
