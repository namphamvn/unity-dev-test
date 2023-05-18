using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Planetario.ClippingPortals
{
	/// <summary>
	///     Base largely on PhysicsRaycaster
	///     Filter out all the hit that is clipped by ClipPlane or ClipRect
	/// </summary>
	public class ClippingPhysicsRaycaster : PhysicsRaycaster
	{
		private RaycastHit[] _hits;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			var ray = new Ray();
			var displayIndex = 0;
			float distanceToClipPlane = 0;
			if (!ComputeRayAndDistance(eventData, ref ray, ref displayIndex, ref distanceToClipPlane))
			{
				return;
			}

			var hitCount = 0;

			if (m_MaxRayIntersections == 0)
			{
				_hits = Physics.RaycastAll(ray, distanceToClipPlane, finalEventMask);
				hitCount = _hits.Length;
			}
			else
			{
				if (m_LastMaxRayIntersections != m_MaxRayIntersections)
				{
					_hits = new RaycastHit[m_MaxRayIntersections];
					m_LastMaxRayIntersections = m_MaxRayIntersections;
				}

				hitCount = Physics.RaycastNonAlloc(ray, _hits, distanceToClipPlane, finalEventMask);
			}

			if (hitCount != 0)
			{
				if (hitCount > 1)
				{
					Array.Sort(_hits, 0, hitCount, RaycastHitComparer.Instance);
				}

				for (int b = 0, bMax = hitCount; b < bMax; ++b)
				{
					var raycastHit = _hits[b];
					var clippedCollider = raycastHit.collider.gameObject.GetComponent<IColliderClipper>();
					if (clippedCollider != null &&
					    clippedCollider.IsPointClipped(ray, raycastHit.point, raycastHit.distance))
					{
						continue;
					}

					var result = new RaycastResult
					{
						gameObject = raycastHit.collider.gameObject,
						module = this,
						distance = raycastHit.distance,
						worldPosition = raycastHit.point,
						worldNormal = raycastHit.normal,
						screenPosition = eventData.position,
						displayIndex = displayIndex,
						index = resultAppendList.Count,
						sortingLayer = 0,
						sortingOrder = 0
					};
					resultAppendList.Add(result);
				}
			}
		}

		private class RaycastHitComparer : IComparer<RaycastHit>
		{
			public static readonly RaycastHitComparer Instance = new();

			public int Compare(RaycastHit x, RaycastHit y)
			{
				return x.distance.CompareTo(y.distance);
			}
		}
	}
}
