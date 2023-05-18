using Planetario.Geometry.Extensions;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.Geometry.EvenBeziers
{
	public class EvenBezier
	{
		private const int SegmentCount = 50;
		private readonly IEqualBezierParams _params;
		private readonly ICurveRatioSearchSettings _searchSettings;

		public EvenBezier(IEqualBezierParams parameters, ICurveRatioSearchSettings searchSettings)
		{
			_params = parameters;
			_searchSettings = searchSettings;
		}

		public CubicBezier ComputeSpline(float curveRatio)
		{
			return new CubicBezier(_params.Start.position, _params.Start.forward,
				_params.End.position, _params.End.forward, curveRatio, _params.EndToStartRatio);
		}

		public float ComputeCurveRatio(float curveRatio)
		{
			(float ratio, float delta) cur = (curveRatio, ComputeDelta(curveRatio));
			(float ratio, float delta) prev = (curveRatio * 2, ComputeDelta(curveRatio * 2));
			// float min = ComputeDeviance(cur.ratio);
			var attempt = 0;
			while (attempt < _searchSettings.MaxAttempt)
			{
				if (math.abs(cur.delta) < _searchSettings.DeltaEpsilon)
				{
					break;
				}

				if (cur.delta > 0)
				{
					if (prev.delta > 0)
					{
						var temp = cur;
						cur.ratio = math.min(cur.ratio, prev.ratio) - math.abs(cur.ratio - prev.ratio);
						cur.delta = ComputeDelta(cur.ratio);
						prev = temp;
					}
					else
					{
						var temp = cur;
						cur.ratio = (cur.ratio + prev.ratio) / 2f;
						cur.delta = ComputeDelta(cur.ratio);
						prev = temp;
					}
				}
				else
				{
					if (prev.delta > 0)
					{
						var temp = cur;
						cur.ratio = (cur.ratio + prev.ratio) / 2f;
						cur.delta = ComputeDelta(cur.ratio);
						prev = temp;
					}
					else
					{
						var temp = cur;
						cur.ratio = math.max(cur.ratio, prev.ratio) + math.abs(cur.ratio - prev.ratio);
						cur.delta = ComputeDelta(cur.ratio);
						prev = temp;
					}
				}

				attempt++;
			}

			//
			// if (!AutoComputeRatio)
			// {
			// 	Debug.Log($"Found {cur.ratio}, delta = {cur.delta}, after {attempt} attempts");
			// }
			return cur.ratio;
		}

		private float ComputeDelta(float curveRatio)
		{
			var d1 = ComputeDeviance(curveRatio);
			var d2 = ComputeDeviance(curveRatio * _searchSettings.DeltaStepPercentage);
			var delta = d2 - d1;
			return delta;
		}

		private float ComputeDeviance(float curveRatio)
		{
			var spline = ComputeSpline(curveRatio);
			return spline.Points(SegmentCount).Segments().Deviance();
		}
	}
}
