using Windows;
using Plugins.vcow.ScreenLocker;
using Plugins.vcow.WindowManager;
using ScreenLocker;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace StartScene
{
	[DisallowMultipleComponent]
	public class StartSceneController : MonoBehaviour
	{
		[Inject] private readonly IScreenLockerManager _screenLockerManager;
		[Inject] private readonly IWindowManager _windowManager;

		private void Start()
		{
			if (_screenLockerManager.IsLocked)
			{
				_screenLockerManager.Unlock(completeCallback: _ => OnStart());
			}
			else
			{
				OnStart();
			}
		}

		private void OnStart()
		{
		}

		public void OnPlay()
		{
			_screenLockerManager.Lock(SceneScreenLocker.Key, () => SceneManager.LoadSceneAsync(Const.GameScene));
		}

		public void OnSettings()
		{
			_windowManager.ShowWindow(SettingsWindow.Id);
		}
	}
}