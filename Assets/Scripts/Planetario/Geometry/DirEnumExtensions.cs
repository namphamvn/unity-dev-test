using Unity.Mathematics;
using UnityEngine;

namespace Planetario.Geometry
{
	public static class DirEnumExtensions
	{
		private static readonly float RADIAN90 = math.radians(90f);
		private static readonly float RADIAN180 = math.radians(180f);
		private static readonly float RADIAN270 = math.radians(270f);
		private static readonly float3 UP = DirEnum.UpY.ToFloat3();

		public static float3 ToFloat3(this DirEnum dir)
		{
			switch (dir)
			{
				case DirEnum.ForwardZ:
					return new float3(0f, 0f, 1f);
				case DirEnum.UpY:
					return new float3(0f, 1f, 0f);
				case DirEnum.RightX:
					return new float3(1f, 0f, 0f);
				case DirEnum.InvForwardZ:
					return new float3(0f, 0f, -1f);
				case DirEnum.InvUpY:
					return new float3(0f, -1f, 0f);
				case DirEnum.InvRightX:
					return new float3(-1f, 0f, 0f);
				default:
					return float3.zero;
			}
		}

		// public static float3 ToFloat3(this DirEnum dir, in LocalToWorld localToWorld)
		// {
		// 	switch (dir)
		// 	{
		// 		case DirEnum.Forward:
		// 			return localToWorld.Forward;
		// 		case DirEnum.Up:
		// 			return localToWorld.Up;
		// 		case DirEnum.Right:
		// 			return localToWorld.Right;
		// 		case DirEnum.RevForward:
		// 			return -localToWorld.Forward;
		// 		case DirEnum.RevUp:
		// 			return -localToWorld.Up;
		// 		case DirEnum.RevRight:
		// 			return -localToWorld.Right;
		// 		default:
		// 			return float3.zero;
		// 	}
		// }

		public static float3 ToFloat3(this DirEnum dir, in Transform transform)
		{
			switch (dir)
			{
				case DirEnum.ForwardZ:
					return transform.forward;
				case DirEnum.UpY:
					return transform.up;
				case DirEnum.RightX:
					return transform.right;
				case DirEnum.InvForwardZ:
					return -transform.forward;
				case DirEnum.InvUpY:
					return -transform.up;
				case DirEnum.InvRightX:
					return -transform.right;
				default:
					return float3.zero;
			}
		}

		public static quaternion ToForwardRotation(this DirEnum dir)
		{
			switch (dir)
			{
				case DirEnum.ForwardZ:
					return quaternion.identity;
				case DirEnum.UpY:
					return quaternion.identity; //invalid!
				case DirEnum.RightX:
					return quaternion.RotateY(RADIAN90);
				case DirEnum.InvForwardZ:
					return quaternion.RotateY(RADIAN180);
				case DirEnum.InvUpY:
					return quaternion.identity; //invalid!
				case DirEnum.InvRightX:
					return quaternion.RotateY(RADIAN270);
				default:
					return quaternion.identity; //invalid!
			}
		}

		public static quaternion ToRotation(this DirEnum dir)
		{
			return quaternion.LookRotation(dir.ToFloat3(), UP);
		}

		public static DirEnum RotateClockwise(this DirEnum dir)
		{
			switch (dir)
			{
				case DirEnum.ForwardZ:
					return DirEnum.RightX;
				case DirEnum.UpY:
					return DirEnum.UpY; //invalid!
				case DirEnum.RightX:
					return DirEnum.InvForwardZ;
				case DirEnum.InvForwardZ:
					return DirEnum.InvRightX;
				case DirEnum.InvUpY:
					return DirEnum.InvUpY; //invalid!
				case DirEnum.InvRightX:
					return DirEnum.ForwardZ;
				default:
					return DirEnum.ForwardZ; //invalid!
			}
		}
	}
}
