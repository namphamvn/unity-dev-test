using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Planetario.GameWorlds.Models;
using Unity.Entities;

namespace Planetario.GameWorlds.Services
{
	public class PersistenceService
	{
		private static PersistenceService _instance;
		private readonly JsonSerializerSettings _jsonSetting;

		public PersistenceService()
		{
			_jsonSetting = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ContractResolver = new PrivateFieldsContractResolver()
			};

			AotHelper.EnsureList<int>(); //this is to fix deserialization of HashSet<int> on device
			AotHelper.EnsureList<GameEntity>();
			AotHelper.EnsureList<IComponentData>();
			AotHelper.EnsureList<ICommand>();
		}

		public static PersistenceService Instance => _instance ??= new PersistenceService();

		public string Serialize<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSetting);
		}

		public T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, _jsonSetting);
		}

		public T Clone<T>(T obj)
		{
			return Deserialize<T>(Serialize(obj));
		}

		private class PrivateFieldsContractResolver : DefaultContractResolver
		{
			protected override List<MemberInfo> GetSerializableMembers(Type objectType)
			{
				var results =
					objectType.GetFields(BindingFlags.Instance | BindingFlags.Public |
					                     BindingFlags.NonPublic);
				var returnValue = results
					.Where(info => info.GetCustomAttribute<NonSerializedAttribute>() == null)
					.Cast<MemberInfo>()
					.ToList();
				return returnValue;
			}

			protected override JsonProperty CreateProperty(MemberInfo member,
				MemberSerialization memberSerialization)
			{
				var property = base.CreateProperty(member, memberSerialization);

				if (member is FieldInfo fieldInfo)
				{
					property.Readable = true;
					property.Writable = true;
				}

				return property;
			}
		}
	}
}
