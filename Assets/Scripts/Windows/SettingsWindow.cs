using System;
using Plugins.vcow.WindowManager.Template;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Windows
{
	public class SettingsWindow : ScaleAppearPopupWindowBase<DialogButtonType>
	{
		public const string Id = nameof(SettingsWindow);

		[SerializeField, Header("Language toggles")] private Toggle _en;
		[SerializeField] private Toggle _ru;

		private void Start()
		{
			var selLocaleCode = LocalizationSettings.SelectedLocale.Identifier.Code;
			switch (selLocaleCode)
			{
				case "en":
					_en.isOn = true;
					break;
				case "ru":
					_ru.isOn = true;
					break;
				default:
					throw new Exception($"There is no toggle for selected locale {selLocaleCode}.");
			}
		}

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

		public void OnEn(bool isOn)
		{
			if (isOn)
			{
				SetLanguage("en");
			}
		}

		public void OnRu(bool isOn)
		{
			if (isOn)
			{
				SetLanguage("ru");
			}
		}

		private void SetLanguage(string localeCode)
		{
			var locales = LocalizationSettings.AvailableLocales.Locales;
			foreach (var locale in locales)
			{
				if (locale.Identifier.Code == localeCode)
				{
					LocalizationSettings.SelectedLocale = locale;
					return;
				}
			}

			Debug.LogError($"Locale with code {localeCode} not found.");
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_en, "_en != null");
			Assert.IsNotNull(_ru, "_ru != null");
		}
	}
}