using UnityEngine;
using UnityEngine.UI;

namespace Planetario.GameWorlds.Views
{
	/// <summary>
	/// A button to send a fixed predetermined command
	/// </summary>
	public abstract class CommandButtonView<TCommand> : MonoBehaviour where TCommand : ICommand
	{
		[SerializeField] private TCommand _command;
		[SerializeField] private Button _button;
		[SerializeField] private GameWorldView _world;

		private void Awake()
		{
			_button.onClick.AddListener(PerformCommand);
		}

		private void PerformCommand()
		{
			_world.WorldSystem.ReceiveCommand(_command);
		}
	}
}
