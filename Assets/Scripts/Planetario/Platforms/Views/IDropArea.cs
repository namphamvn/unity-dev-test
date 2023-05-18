using Planetario.GameWorlds.Models;
using Planetario.Geometry.Primitive;
using Planetario.MoveConstraints.Models;
using UnityEngine;
using Ray = Planetario.Geometry.Primitive.Ray;

namespace Planetario.Platforms.Views
{
	public interface IDropArea
	{
		IGameEntityInfo LogicEntity { get; }
		bool EnterArea(in Ray dragRay, out float distance);
		bool ExitArea(in Ray dragRay);
		bool ComputeDropLocation(in Ray dragRay, out Location dropLocation);
		bool ComputeDropLocation(in Ray dragRay, out int dropLocationId, out Location dropLocation);
		Location ComputeHoverConstraint(in DraggableState state, Ray currentRay);
		float ObjectScale();
		Transform GetTransform();
	}

	public static class DropAreaExtensions
	{
		public static float AreaWorldScale(this IDropArea area)
		{
			return area.GetTransform().lossyScale.x;
		}
	}
}
