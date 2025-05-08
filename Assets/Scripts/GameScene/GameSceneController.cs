using Plugins.vcow.ScreenLocker;
using UnityEngine;
using VContainer;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneController : MonoBehaviour
	{
		[Inject] private readonly IScreenLockerManager _screenLockerManager;

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
	}
}