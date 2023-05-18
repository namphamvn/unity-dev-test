using System;
using UnityEngine;

namespace Planetario.Shares.MaterialServices
{
	[Serializable]
	public struct MaterialKey : IEquatable<MaterialKey>
	{
		public Material source;
		public string keyword;

		/// <summary>
		///     -1 mean take the render queue from the shader
		/// </summary>
		public int renderQueue;

		public MaterialKey(Material source, string keyword, int renderQueue = -1)
		{
			this.source = source;
			this.keyword = keyword;
			this.renderQueue = renderQueue;
		}

		public bool Equals(MaterialKey other)
		{
			return Equals(source, other.source) && keyword == other.keyword &&
			       renderQueue == other.renderQueue;
		}

		public override bool Equals(object obj)
		{
			return obj is MaterialKey other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(source, keyword, renderQueue);
		}

		public override string ToString()
		{
			return $"{source.name}-{keyword}-{renderQueue}";
		}
	}
}
