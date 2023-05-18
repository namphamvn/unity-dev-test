using Planetario.Geometry.Meshes;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;
using UnityEngine;
using Grid = Planetario.Geometry.Meshes.Grid;

namespace Planetario.Platforms.Views
{
	public class GridView : PlatformView<Grid>
	{
		private void OnDrawGizmosSelected()
		{
			var plane = new CartesianPlane(transform);
			var extends = Data.GetExtends();
			var positions = new Vector3[]
			{
				plane.GetPoint(new float2(-extends.x, -extends.y)),
				plane.GetPoint(new float2(-extends.x, +extends.y)),
				plane.GetPoint(new float2(+extends.x, +extends.y)),
				plane.GetPoint(new float2(+extends.x, -extends.y))
			};
			Gizmos.color = Color.red;
			for (var i = 0; i < 4; i++)
			{
				Gizmos.DrawLine(positions[i], positions[(i + 1) % 4]);
			}
		}

		protected override IRaycastable GetRayBlocker()
		{
			return new Rectangle
			{
				plane = new CartesianPlane(transform),
				extends = new float2(Data.cellCount.x * Data.cellSize, Data.cellCount.y * Data.cellSize) *
				          0.5f
			};
		}

		public override IPlatformLocations GetPlatformLocations()
		{
			return new GridLocations(Data);
		}
	}
}
