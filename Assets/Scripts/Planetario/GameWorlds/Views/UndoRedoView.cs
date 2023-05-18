using UnityEngine;
using UnityEngine.UI;

namespace Planetario.GameWorlds.Views
{
	public class UndoRedoView : MonoBehaviour
	{
		[SerializeField] private Button _undoButton;
		[SerializeField] private GameWorldView _world;

		private void Awake()
		{
			_undoButton.onClick.AddListener(Undo);
		}

		private void Undo()
		{
			_world.WorldSystem.Undo();
		}
	}
}
