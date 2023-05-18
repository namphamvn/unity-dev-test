using System;
using System.Collections.Generic;
using Planetario.Shares;
using UnityEngine;

namespace Planetario.GameWorlds.Services
{
	public class ComponentTypeService : MonoBehaviour
	{
		private readonly Dictionary<Type, Type> _dataToView;
		private readonly Dictionary<Type, Type> _viewToData;

		public ComponentTypeService()
		{
			_dataToView = new Dictionary<Type, Type>();
			_viewToData = new Dictionary<Type, Type>();

			foreach (var viewType in typeof(IView).GetDerivedTypes())
			{
				if (viewType.IsInterface || viewType.IsAbstract || viewType.BaseType == null)
				{
					continue;
				}

				Type dataType = null;
				foreach (var interfaceType in viewType.GetInterfaces())
				{
					if (typeof(IView).IsAssignableFrom(interfaceType) && interfaceType.IsGenericType)
					{
						dataType = interfaceType.GetGenericArguments()[0];
						break;
					}
				}

				if (dataType != null)
				{
					_dataToView.Add(dataType, viewType);
					_viewToData.Add(viewType, dataType);
				}
			}
		}

		public Type GetDataType(Type viewType)
		{
			return _viewToData[viewType];
		}
	}
}
