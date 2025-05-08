using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PreloaderScene
{
	[DisallowMultipleComponent]
	public class PreloaderSceneLifetimeScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
		}
	}
}