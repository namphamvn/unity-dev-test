using System;
using UnityEngine;

namespace Planetario.Shares
{
	/// <summary>
	///     A Unity-serializable version of Nullable_T or T?
	///     Also has a custom property drawer
	/// </summary>
	[Serializable]
	public struct SerializableNullable<T> where T : struct
	{
		[SerializeField] private T v;

		[SerializeField] private bool hasValue;

		public SerializableNullable(bool hasValue, T v)
		{
			this.v = v;
			this.hasValue = hasValue;
		}

		private SerializableNullable(T v)
		{
			this.v = v;
			hasValue = true;
		}

		public T Value
		{
			get
			{
				if (!HasValue)
				{
					throw new InvalidOperationException(
						"Serializable nullable object must have a value.");
				}

				return v;
			}
		}

		public bool HasValue => hasValue;

		public static implicit operator SerializableNullable<T>(T value)
		{
			return new SerializableNullable<T>(value);
		}

		public static implicit operator SerializableNullable<T>(T? value)
		{
			return value.HasValue ? new SerializableNullable<T>(value.Value) : new SerializableNullable<T>();
		}

		public static implicit operator T?(SerializableNullable<T> value)
		{
			return value.HasValue ? value.Value : null;
		}
	}
}
