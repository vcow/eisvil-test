using System;
using GameScene.Logic;
using R3;
using UnityEngine;

namespace GameScene.Quest.Controller
{
	public abstract class EnemiesKilledQuestControllerBase : QuestControllerBase
	{
		private readonly ReactiveProperty<bool> _isCompleted;
		private readonly ReactiveProperty<float> _progress;
		private readonly ReactiveProperty<string> _label;

		public override ReadOnlyReactiveProperty<bool> IsCompleted => _isCompleted;
		public override ReadOnlyReactiveProperty<float> Progress => _progress;
		public override ReadOnlyReactiveProperty<string> Label => _label;

		protected EnemiesKilledQuestControllerBase(int numEnemies, SceneContext sceneContext, EnemyType enemyType,
			QuestTriggerType triggerType) : base(triggerType, new object[] { numEnemies })
		{
			_isCompleted = new ReactiveProperty<bool>(false).AddTo(_disposables);
			_label = new ReactiveProperty<string>(string.Empty).AddTo(_disposables);

			var deadEnemiesObservable = sceneContext.GetDeadEnemiesObservable(enemyType);
			_progress = new ReactiveProperty<float>((float)deadEnemiesObservable.CurrentValue / numEnemies).AddTo(_disposables);

			IDisposable h = null;
			h = deadEnemiesObservable.Subscribe(i =>
				{
					_label.Value = $"{i}/{numEnemies}";
					_progress.Value = Mathf.Clamp01((float)i / numEnemies);
					if (i >= numEnemies)
					{
						_disposables.Remove(h);
						_isCompleted.Value = true;
					}
				})
				.AddTo(_disposables);
		}
	}
}