using System;
using System.Collections.Generic;
using System.Linq;

namespace Planetario.Shares
{
	public static class ReflectionUtils
	{
		public static IEnumerable<Type> GetDerivedTypes(this Type baseType)
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => baseType.IsAssignableFrom(type) && type != baseType);
		}
	}
}
