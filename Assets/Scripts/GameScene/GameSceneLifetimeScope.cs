using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Logic;
using GameScene.Quest;
using GameScene.Quest.Controller;
using Plugins.vcow.WindowManager;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneLifetimeScope : LifetimeScope
	{
		private readonly CompositeDisposable _disposables = new();

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance<WindowManager.InstantiateWindowHook>(InstantiateWindowHook);
			builder.Register<WindowManager>(Lifetime.Singleton).AsImplementedInterfaces();

			builder.Register<SceneContext>(Lifetime.Singleton);

			builder.Register<IReadOnlyList<QuestControllerBase>>(resolver =>
					resolver.Resolve<IQuestSettings>().QuestData.Select(pair =>
						{
							var sceneContext = resolver.Resolve<SceneContext>();
							return pair.Key switch
							{
								QuestTriggerType.MinTimeInGame =>
									(QuestControllerBase)new MinTimeInGameQuestController(pair.Value, sceneContext).AddTo(_disposables),
								QuestTriggerType.RedEnemiesKilled =>
									new RedEnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext).AddTo(_disposables),
								QuestTriggerType.BlueEnemiesKilled =>
									new BlueEnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext).AddTo(_disposables),
								QuestTriggerType.TotalEnemiesKilled =>
									new EnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext).AddTo(_disposables),
								_ => throw new NotSupportedException($"The quest {pair.Key} isn't supported.")
							};
						})
						.ToArray()
				, Lifetime.Singleton);
		}

		private void InstantiateWindowHook(IWindow window)
		{
			Container.Inject(window);
		}

		protected override void OnDestroy()
		{
			_disposables.Dispose();
			base.OnDestroy();
		}
	}
}