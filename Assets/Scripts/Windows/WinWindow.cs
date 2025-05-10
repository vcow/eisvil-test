using Plugins.vcow.ScreenLocker;
using Plugins.vcow.TouchHelper;
using Plugins.vcow.WindowManager.Template;
using ScreenLocker;
using UnityEngine.SceneManagement;
using VContainer;

namespace Windows
{
	public class WinWindow : ScaleAppearPopupWindowBase<DialogButtonType>
	{
		public const string Id = nameof(WinWindow);

		[Inject] private readonly IScreenLockerManager _screenLockerManager;

		private int? _locker;

		protected override string GetWindowId()
		{
			return Id;
		}

		protected override void DoSetArgs(object[] args)
		{
		}

		public void OnOk()
		{
			Close();
		}

		private void Start()
		{
			_locker = TouchHelper.Lock();
		}

		protected override void OnDestroy()
		{
			if (_locker.HasValue)
			{
				TouchHelper.Unlock(_locker.Value);
				_locker = null;
			}

			base.OnDestroy();

			_screenLockerManager.Lock(SceneScreenLocker.Key, () =>
				SceneManager.LoadSceneAsync(Const.StartScene));
		}
	}
}