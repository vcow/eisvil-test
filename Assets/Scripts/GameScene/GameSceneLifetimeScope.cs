using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneLifetimeScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
		}
	}
}