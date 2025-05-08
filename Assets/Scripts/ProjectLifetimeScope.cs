using Windows;
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

	protected override void Awake()
	{
		DontDestroyOnLoad(gameObject);
		base.Awake();
	}

	protected override void Configure(IContainerBuilder builder)
	{
		builder.RegisterInstance<IScreenLockerSettings>(_screenLockerSettings);
		builder.RegisterInstance<ScreenLockerManager.InstantiateScreenLockerHook>(InstantiateScreenLockerHook);
		builder.RegisterInstance(_windowManagerSettings.GetSettings());
		builder.Register<ScreenLockerManager>(Lifetime.Singleton).AsImplementedInterfaces();
	}

	private void InstantiateScreenLockerHook(ScreenLockerBase locker)
	{
	}

	private void OnValidate()
	{
		Assert.IsNotNull(_screenLockerSettings);
		Assert.IsNotNull(_windowManagerSettings);
	}
}