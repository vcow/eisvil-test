using Plugins.vcow.WindowManager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StartScene
{
	[DisallowMultipleComponent]
	public class StartSceneLifetimeScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance<WindowManager.InstantiateWindowHook>(InstantiateWindowHook);
			builder.Register<WindowManager>(Lifetime.Singleton).AsImplementedInterfaces();
		}

		private void InstantiateWindowHook(IWindow window)
		{
			Container.Inject(window);
		}
	}
}