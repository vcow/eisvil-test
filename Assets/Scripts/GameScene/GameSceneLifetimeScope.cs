using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Logic;
using GameScene.Quest;
using GameScene.Quest.Controller;
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
			builder.Register<SceneContext>(Lifetime.Singleton);

			builder.Register<IReadOnlyList<QuestControllerBase>>(resolver =>
					resolver.Resolve<IQuestSettings>().QuestData.Select(pair =>
						{
							var sceneContext = resolver.Resolve<SceneContext>();
							return pair.Key switch
							{
								QuestTriggerType.MinTimeInGame => (QuestControllerBase)new MinTimeInGameQuestController(pair.Value, sceneContext),
								QuestTriggerType.RedEnemiesKilled => new RedEnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext),
								QuestTriggerType.BlueEnemiesKilled => new BlueEnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext),
								QuestTriggerType.TotalEnemiesKilled => new EnemiesKilledQuestController(Mathf.FloorToInt(pair.Value), sceneContext),
								_ => throw new NotSupportedException($"The quest {pair.Key} isn't supported.")
							};
						})
						.ToArray()
				, Lifetime.Singleton);
		}
	}
}