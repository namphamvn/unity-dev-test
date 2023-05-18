using UnityEngine;

namespace Planetario.Geometry.EvenBeziers
{
	//TODO combine with IRoadCurve
	public interface ICurveParams
	{
		Transform Start { get; }
		Transform End { get; }
	}
}
