using System;
using GameScene.Logic;
using R3;
using UnityEngine;

namespace GameScene.Quest.Controller
{
	public sealed class MinTimeInGameQuestController : QuestControllerBase
	{
		private readonly ReactiveProperty<bool> _isCompleted;
		private readonly ReactiveProperty<float> _progress;

		public override ReadOnlyReactiveProperty<bool> IsCompleted => _isCompleted;
		public override ReadOnlyReactiveProperty<float> Progress => _progress;

		public MinTimeInGameQuestController(float timeSec, SceneContext sceneContext)
			: base(QuestTriggerType.MinTimeInGame, new object[] { timeSec })
		{
			_isCompleted = new ReactiveProperty<bool>(false).AddTo(_disposables);
			_progress = new ReactiveProperty<float>(0f).AddTo(_disposables);

			var initialTime = Time.time;
			IDisposable h = null;
			h = Observable.EveryUpdate().Subscribe(_ =>
			{
				var timesPass = Time.time - initialTime;
				_progress.Value = Mathf.Clamp01(timesPass / timeSec);
				if (_progress.Value >= 1f)
				{
					_disposables.Remove(h);
					_isCompleted.Value = true;
				}
			}).AddTo(_disposables);
		}
	}
}