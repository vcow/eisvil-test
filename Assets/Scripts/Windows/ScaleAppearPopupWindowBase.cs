using DG.Tweening;
using Plugins.vcow.WindowManager;
using Plugins.vcow.WindowManager.Template;
using UnityEngine;
using UnityEngine.UI;

namespace Windows
{
	[DisallowMultipleComponent, RequireComponent(typeof(RawImage), typeof(CanvasGroup))]
	public abstract class ScaleAppearPopupWindowBase<T> : PopupWindowBase<T>
	{
		private Tween _tween;
		private Color _initialBlendColor;

		private void Awake()
		{
			_initialBlendColor = Blend.color;
		}

		protected override void DoActivate(bool immediately)
		{
			if (State is WindowState.Active or WindowState.ToActive)
			{
				return;
			}

			_tween?.Kill();

			var canvasGroup = GetComponent<CanvasGroup>();
			if (immediately)
			{
				_tween = null;
				canvasGroup.interactable = true;
				Popup.localScale = Vector3.one;
				Blend.color = _initialBlendColor;
				State = WindowState.Active;
			}
			else
			{
				canvasGroup.interactable = false;
				Popup.localScale = Vector3.one * 0.01f;
				Blend.color = new Color(_initialBlendColor.r, _initialBlendColor.g, _initialBlendColor.b, 0f);
				const float activateDurationSec = 1f;

				State = WindowState.ToActive;
				_tween = DOTween.Sequence()
					.Append(Blend.DOFade(_initialBlendColor.a, activateDurationSec * 0.5f).SetEase(Ease.Linear))
					.Join(Popup.DOScale(Vector3.one, activateDurationSec).SetEase(Ease.OutBack))
					.OnComplete(() =>
					{
						_tween = null;
						canvasGroup.interactable = true;
						State = WindowState.Active;
					});
			}
		}

		protected override void DoDeactivate(bool immediately)
		{
			if (State is WindowState.Inactive or WindowState.ToInactive)
			{
				return;
			}

			_tween?.Kill();

			var canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.interactable = false;
			var blendDestinationColor = new Color(_initialBlendColor.r, _initialBlendColor.g, _initialBlendColor.b, 0f);
			if (immediately)
			{
				_tween = null;
				Popup.localScale = Vector3.one * 0.01f;
				Blend.color = blendDestinationColor;
				State = WindowState.Inactive;
			}
			else
			{
				Popup.localScale = Vector3.one;
				Blend.color = _initialBlendColor;
				const float deactivateDurationSec = 1f;

				State = WindowState.ToInactive;
				_tween = DOTween.Sequence()
					.Append(Blend.DOFade(0f, deactivateDurationSec).SetEase(Ease.Linear))
					.Join(Popup.DOScale(Vector3.one * 0.01f, deactivateDurationSec).SetEase(Ease.InBack))
					.OnComplete(() =>
					{
						_tween = null;
						State = WindowState.Inactive;
					});
			}
		}

		protected override void OnDestroy()
		{
			_tween?.Kill(true);
			base.OnDestroy();
		}
	}
}