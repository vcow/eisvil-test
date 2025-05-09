using Windows;
using GameScene.Quest;
using Plugins.vcow.ScreenLocker;
using ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

[DisallowMultipleComponent]
public sealed class ProjectLifetimeScope : LifetimeScope
{
	[SerializeField] private ScreenLockerSettings _screenLockerSettings;
	[SerializeField] private WindowManagerSettings _windowManagerSettings;
	[SerializeField] private QuestSettings _questSettings;

	protected override void Awake()
	{
		DontDestroyOnLoad(gameObject);
		base.Awake();
	}

	protected override void Configure(IContainerBuilder builder)
	{
		builder.RegisterInstance<IScreenLockerSettings>(_screenLockerSettings);
		builder.RegisterInstance<IQuestSettings>(_questSettings);
		builder.RegisterInstance<ScreenLockerManager.InstantiateScreenLockerHook>(InstantiateScreenLockerHook);
		builder.RegisterInstance(_windowManagerSettings.GetSettings());
		builder.Register<ScreenLockerManager>(Lifetime.Singleton).AsImplementedInterfaces();
	}

	private void InstantiateScreenLockerHook(ScreenLockerBase locker)
	{
	}

	private void OnValidate()
	{
		Assert.IsNotNull(_screenLockerSettings, "_screenLockerSettings != null");
		Assert.IsNotNull(_windowManagerSettings,  "_windowManagerSettings != null");
		Assert.IsNotNull(_questSettings, "_questSettings != null");
	}
}