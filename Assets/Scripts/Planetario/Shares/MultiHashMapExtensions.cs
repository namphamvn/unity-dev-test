using System;
using Unity.Collections;

namespace Planetario.Shares
{
	public static class MultiHashMapExtensions
	{
		public static void AddUniqueValueToKey<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> map,
			TKey key, TValue value)
			where TKey : unmanaged, IEquatable<TKey>
			where TValue : unmanaged, IEquatable<TValue>
		{
			if (map.ContainsKey(key))
			{
				var found = false;
				foreach (var face in map.GetValuesForKey(key))
				{
					if (face.Equals(value))
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					map.Add(key, value);
				}
			}
			else
			{
				map.Add(key, value);
			}
		}
	}
}
