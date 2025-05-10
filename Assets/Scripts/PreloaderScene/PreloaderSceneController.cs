using Cysharp.Threading.Tasks;
using Plugins.vcow.ScreenLocker;
using R3;
using ScreenLocker;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace PreloaderScene
{
	[DisallowMultipleComponent]
	public class PreloaderSceneController : MonoBehaviour
	{
		[Inject] private readonly IScreenLockerManager _screenLockerManager;

		private void Start()
		{
			_screenLockerManager.Lock(GameScreenLocker.Key, Init);
		}

		private async void Init()
		{
			ObservableSystem.DefaultFrameProvider = UnityFrameProvider.Update;
			await UniTask.WaitForSeconds(1f);
			// TODO: Add other initialization tasks here

			await SceneManager.LoadSceneAsync(Const.StartScene);
		}
	}
}