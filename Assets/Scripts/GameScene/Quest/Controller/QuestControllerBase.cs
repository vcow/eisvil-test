using System;
using R3;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace GameScene.Quest.Controller
{
	public abstract class QuestControllerBase : IDisposable
	{
		private readonly string _descriptionKey;
		private readonly ReactiveProperty<string> _description = new();
		private readonly object[] _descriptionArgs;

		// ReSharper disable once InconsistentNaming
		protected readonly CompositeDisposable _disposables = new();

		public ReadOnlyReactiveProperty<string> Description => _description;
		public abstract ReadOnlyReactiveProperty<bool> IsCompleted { get; }
		public abstract ReadOnlyReactiveProperty<float> Progress { get; }
		public abstract ReadOnlyReactiveProperty<string> Label { get; }

		protected QuestControllerBase(QuestTriggerType triggerType, object[] descriptionArgs)
		{
			_descriptionKey = $"{triggerType}_description";
			_descriptionArgs = descriptionArgs;

			Observable.FromEvent<Action<Locale>, Locale>(
					h => locale => h(locale),
					h => LocalizationSettings.SelectedLocaleChanged += h,
					h => LocalizationSettings.SelectedLocaleChanged -= h)
				.Subscribe(_ => ValidateDescriptionText())
				.AddTo(_disposables);
			ValidateDescriptionText();
		}

		private async void ValidateDescriptionText()
		{
			var text = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_descriptionKey).Task;
			_description.Value = string.IsNullOrEmpty(text) ? string.Empty : string.Format(text, _descriptionArgs);
		}

		public virtual void Dispose()
		{
			_disposables.Dispose();
		}
	}
}