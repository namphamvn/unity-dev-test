using System.IO;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Planetario.GameWorlds.Views
{
	public class PersistenceView : MonoBehaviour
	{
		[SerializeField] private Button _saveButton;
		[SerializeField] private Button _loadButton;
		[SerializeField] private GameWorldView _world;

		private void Awake()
		{
			_saveButton.onClick.AddListener(Save);
			_loadButton.onClick.AddListener(Load);
		}

		private static string GetSavePath()
		{
#if UNITY_EDITOR
			return Application.dataPath;
#else
			return Application.persistentDataPath;
#endif
		}

		private void Save()
		{
			var world = _world.World;
			var jsonWorld = PersistenceService.Instance.Serialize(world);
			File.WriteAllText(GetSaveFile(), jsonWorld);
		}

		private static string GetSaveFile()
		{
			return Path.Combine(GetSavePath(), "game-state.json");
		}

		private void Load()
		{
			var json = File.ReadAllText(GetSaveFile());
			var world = PersistenceService.Instance.Deserialize<GameWorld>(json);
			_world.LoadData(world);
		}
	}
}
