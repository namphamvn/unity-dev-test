using System;
using UnityEngine;

namespace Planetario.Shares.MaterialServices
{
	[Serializable]
	public struct KeywordKey : IEquatable<KeywordKey>
	{
		public Shader shader;
		public string keyword;

		public KeywordKey(Shader shader, string keyword)
		{
			this.shader = shader;
			this.keyword = keyword;
		}

		public bool Equals(KeywordKey other)
		{
			return Equals(shader, other.shader) && keyword == other.keyword;
		}

		public override bool Equals(object obj)
		{
			return obj is KeywordKey other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(shader, keyword);
		}

		public override string ToString()
		{
			return $"{keyword}-{shader.name}";
		}
	}
}
