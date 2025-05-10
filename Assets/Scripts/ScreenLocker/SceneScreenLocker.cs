using DG.Tweening;
using Plugins.vcow.ScreenLocker;
using Plugins.vcow.TouchHelper;
using UnityEngine;

namespace ScreenLocker
{
	[DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
	public class SceneScreenLocker : ScreenLockerBase
	{
		public const string Key = nameof(SceneScreenLocker);

		private int? _locker;
		private Tween _tween;

		private CanvasGroup _canvasGroup;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0f;
			_canvasGroup.interactable = false;
		}

		public override void Activate(object[] args = null, bool immediately = false)
		{
			if (State is ScreenLockerState.Active or ScreenLockerState.ToActive)
			{
				return;
			}

			_tween?.Kill();
			_canvasGroup.interactable = true;

			if (immediately)
			{
				_tween = null;
				_canvasGroup.alpha = 1f;
				State = ScreenLockerState.Active;
			}
			else
			{
				State = ScreenLockerState.ToActive;
				const float activateDurationSec = 1f;
				_tween = _canvasGroup.DOFade(1f, activateDurationSec).SetEase(Ease.Linear)
					.OnComplete(() =>
					{
						_tween = null;
						State = ScreenLockerState.Active;
					});
			}
		}

		public override void Deactivate(bool immediately = false)
		{
			if (State is ScreenLockerState.Inactive or ScreenLockerState.ToInactive)
			{
				return;
			}

			_tween?.Kill();

			if (immediately)
			{
				_tween = null;
				_canvasGroup.alpha = 0f;
				_canvasGroup.interactable = false;
				if (_locker.HasValue)
				{
					TouchHelper.Unlock(_locker.Value);
					_locker = null;
				}

				State = ScreenLockerState.Inactive;
			}
			else
			{
				State = ScreenLockerState.ToInactive;
				const float deactivateDurationSec = 1f;
				_tween = _canvasGroup.DOFade(0f, deactivateDurationSec).SetEase(Ease.Linear)
					.OnComplete(() =>
					{
						_tween = null;
						_canvasGroup.interactable = false;
						if (_locker.HasValue)
						{
							TouchHelper.Unlock(_locker.Value);
							_locker = null;
						}

						State = ScreenLockerState.Inactive;
					});
			}
		}

		public override void Force()
		{
			if (_tween != null)
			{
				_tween.Kill(true);
			}
		}

		protected override void OnDestroy()
		{
			_tween?.Kill();
			if (_locker.HasValue)
			{
				TouchHelper.Unlock(_locker.Value);
				_locker = null;
			}

			base.OnDestroy();
		}

		public override string LockerKey => Key;
	}
}