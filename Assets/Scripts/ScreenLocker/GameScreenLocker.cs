using DG.Tweening;
using Plugins.vcow.ScreenLocker;
using Plugins.vcow.TouchHelper;
using UnityEngine;
using UnityEngine.Assertions;

namespace ScreenLocker
{
	[DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
	public class GameScreenLocker : ScreenLockerBase
	{
		public const string Key = nameof(GameScreenLocker);

		private int? _locker;
		private Tween _tween;

		public override void Activate(object[] args = null, bool immediately = false)
		{
			Assert.IsFalse(_locker.HasValue);
			_locker = TouchHelper.Lock();
			State = ScreenLockerState.Active;
		}

		public override void Deactivate(bool immediately = false)
		{
			var canvasGroup = GetComponent<CanvasGroup>();

			const float deactivateDurationSec = 1f;

			Assert.IsNull(_tween);
			State = ScreenLockerState.ToInactive;
			_tween = canvasGroup.DOFade(0, deactivateDurationSec).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					_tween = null;
					if (_locker.HasValue)
					{
						TouchHelper.Unlock(_locker.Value);
						_locker = null;
					}

					State = ScreenLockerState.Inactive;
				});
		}

		public override void Force()
		{
			if (State == ScreenLockerState.ToInactive && _tween != null)
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